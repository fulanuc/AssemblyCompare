using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.ConVar;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002A5 RID: 677
	public class CombatDirector : MonoBehaviour
	{
		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000DCD RID: 3533 RVA: 0x0000AB85 File Offset: 0x00008D85
		private WeightedSelection<DirectorCard> monsterCards
		{
			get
			{
				return ClassicStageInfo.instance.monsterSelection;
			}
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000DCE RID: 3534 RVA: 0x0000AB91 File Offset: 0x00008D91
		// (set) Token: 0x06000DCF RID: 3535 RVA: 0x0000AB99 File Offset: 0x00008D99
		public BossGroup bossGroup { get; private set; }

		// Token: 0x06000DD0 RID: 3536 RVA: 0x000559D0 File Offset: 0x00053BD0
		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);
				this.moneyWaves = new CombatDirector.DirectorMoneyWave[this.moneyWaveIntervals.Length];
				for (int i = 0; i < this.moneyWaveIntervals.Length; i++)
				{
					this.moneyWaves[i] = new CombatDirector.DirectorMoneyWave
					{
						interval = this.rng.RangeFloat(this.moneyWaveIntervals[i].min, this.moneyWaveIntervals[i].max),
						multiplier = this.creditMultiplier
					};
				}
			}
		}

		// Token: 0x06000DD1 RID: 3537 RVA: 0x0000ABA2 File Offset: 0x00008DA2
		private void OnEnable()
		{
			CombatDirector.instancesList.Add(this);
		}

		// Token: 0x06000DD2 RID: 3538 RVA: 0x00055A74 File Offset: 0x00053C74
		private void OnDisable()
		{
			CombatDirector.instancesList.Remove(this);
			if (NetworkServer.active && CombatDirector.instancesList.Count > 0)
			{
				float num = 0.4f;
				CombatDirector combatDirector = CombatDirector.instancesList[this.rng.RangeInt(0, CombatDirector.instancesList.Count)];
				this.monsterCredit *= num;
				combatDirector.monsterCredit += this.monsterCredit;
				Debug.LogFormat("Transfered {0} monster credits from {1} to {2}", new object[]
				{
					this.monsterCredit,
					base.gameObject,
					combatDirector.gameObject
				});
				this.monsterCredit = 0f;
			}
		}

		// Token: 0x06000DD3 RID: 3539 RVA: 0x00055B2C File Offset: 0x00053D2C
		private void GenerateAmbush(Vector3 victimPosition)
		{
			NodeGraph groundNodes = SceneInfo.instance.groundNodes;
			NodeGraph.NodeIndex nodeIndex = groundNodes.FindClosestNode(victimPosition, HullClassification.Human);
			NodeGraphSpider nodeGraphSpider = new NodeGraphSpider(groundNodes, HullMask.Human);
			nodeGraphSpider.AddNodeForNextStep(nodeIndex);
			List<NodeGraphSpider.StepInfo> list = new List<NodeGraphSpider.StepInfo>();
			int num = 0;
			List<NodeGraphSpider.StepInfo> collectedSteps = nodeGraphSpider.collectedSteps;
			while (nodeGraphSpider.PerformStep() && num < 8)
			{
				num++;
				for (int i = 0; i < collectedSteps.Count; i++)
				{
					if (CombatDirector.IsAcceptableAmbushSpiderStep(groundNodes, nodeIndex, collectedSteps[i]))
					{
						list.Add(collectedSteps[i]);
					}
				}
				collectedSteps.Clear();
			}
			for (int j = 0; j < list.Count; j++)
			{
				Vector3 position;
				groundNodes.GetNodePosition(list[j].node, out position);
				Resources.Load<SpawnCard>("SpawnCards/scLemurian").DoSpawn(position, Quaternion.identity);
			}
		}

		// Token: 0x06000DD4 RID: 3540 RVA: 0x00055C04 File Offset: 0x00053E04
		private static bool IsAcceptableAmbushSpiderStep(NodeGraph nodeGraph, NodeGraph.NodeIndex startNode, NodeGraphSpider.StepInfo stepInfo)
		{
			int num = 0;
			while (stepInfo.previousStep != null)
			{
				if (nodeGraph.TestNodeLineOfSight(startNode, stepInfo.node))
				{
					return false;
				}
				stepInfo = stepInfo.previousStep;
				num++;
				if (num > 2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000DD5 RID: 3541 RVA: 0x0000ABAF File Offset: 0x00008DAF
		public void OverrideCurrentMonsterCard(DirectorCard overrideMonsterCard)
		{
			this.currentMonsterCard = overrideMonsterCard;
		}

		// Token: 0x06000DD6 RID: 3542 RVA: 0x00055C44 File Offset: 0x00053E44
		public void SetNextSpawnAsBoss()
		{
			WeightedSelection<DirectorCard> weightedSelection = new WeightedSelection<DirectorCard>(8);
			Debug.LogFormat("CombatDirector.SetNextSpawnAsBoss() monsterCards.Count={0}", new object[]
			{
				this.monsterCards.Count
			});
			bool flag = this.rng.nextNormalizedFloat > 0.1f;
			int i = 0;
			int count = this.monsterCards.Count;
			while (i < count)
			{
				WeightedSelection<DirectorCard>.ChoiceInfo choice = this.monsterCards.GetChoice(i);
				if (choice.value.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab.GetComponent<CharacterBody>().isChampion == flag && choice.value.CardIsValid() && !choice.value.spawnCard.name.Contains("cscGolem"))
				{
					weightedSelection.AddChoice(choice);
					Debug.LogFormat("bossCards.AddChoice({0})", new object[]
					{
						choice.value.spawnCard.name
					});
				}
				i++;
			}
			if (weightedSelection.Count > 0)
			{
				this.currentMonsterCard = weightedSelection.Evaluate(this.rng.nextNormalizedFloat);
			}
			this.monsterSpawnTimer = -600f;
		}

		// Token: 0x06000DD7 RID: 3543 RVA: 0x00055D64 File Offset: 0x00053F64
		private void PickPlayerAsSpawnTarget()
		{
			ReadOnlyCollection<PlayerCharacterMasterController> instances = PlayerCharacterMasterController.instances;
			List<PlayerCharacterMasterController> list = new List<PlayerCharacterMasterController>();
			foreach (PlayerCharacterMasterController playerCharacterMasterController in instances)
			{
				if (playerCharacterMasterController.master.alive)
				{
					list.Add(playerCharacterMasterController);
				}
			}
			if (list.Count > 0)
			{
				this.currentSpawnTarget = list[this.rng.RangeInt(0, list.Count)].master.GetBodyObject();
			}
		}

		// Token: 0x06000DD8 RID: 3544 RVA: 0x00055DF4 File Offset: 0x00053FF4
		private void Simulate(float deltaTime)
		{
			if (this.targetPlayers)
			{
				this.playerRetargetTimer -= deltaTime;
				if (this.playerRetargetTimer <= 0f)
				{
					this.playerRetargetTimer = this.rng.RangeFloat(1f, 10f);
					this.PickPlayerAsSpawnTarget();
				}
			}
			this.monsterSpawnTimer -= deltaTime;
			if (this.monsterSpawnTimer <= 0f)
			{
				bool flag = false;
				if (TeamComponent.GetTeamMembers(TeamIndex.Monster).Count < 40 || this.isBoss)
				{
					flag = this.AttemptSpawnOnTarget(this.currentSpawnTarget);
				}
				if (flag)
				{
					if (this.shouldSpawnOneWave)
					{
						this.hasStartedWave = true;
					}
					this.monsterSpawnTimer += this.rng.RangeFloat(this.minSeriesSpawnInterval, this.maxSeriesSpawnInterval);
					return;
				}
				this.monsterSpawnTimer += this.rng.RangeFloat(this.minRerollSpawnInterval, this.maxRerollSpawnInterval);
				this.currentMonsterCard = null;
				if (this.shouldSpawnOneWave && this.hasStartedWave)
				{
					base.enabled = false;
					return;
				}
			}
		}

		// Token: 0x06000DD9 RID: 3545 RVA: 0x00055F04 File Offset: 0x00054104
		private bool AttemptSpawnOnTarget(GameObject spawnTarget)
		{
			if (spawnTarget)
			{
				if (this.currentMonsterCard == null)
				{
					this.currentMonsterCard = this.monsterCards.Evaluate(this.rng.nextNormalizedFloat);
					this.lastAttemptedMonsterCard = this.currentMonsterCard;
					this.currentActiveEliteIndex = EliteCatalog.eliteList[this.rng.RangeInt(0, EliteCatalog.eliteList.Count)];
				}
				bool flag = !(this.currentMonsterCard.spawnCard as CharacterSpawnCard).noElites;
				float num = CombatDirector.maximumNumberToSpawnBeforeSkipping * (flag ? CombatDirector.eliteMultiplierCost : 1f);
				if (this.currentMonsterCard.CardIsValid() && this.monsterCredit >= (float)this.currentMonsterCard.cost && (!this.skipSpawnIfTooCheap || this.monsterCredit <= (float)this.currentMonsterCard.cost * num))
				{
					SpawnCard spawnCard = this.currentMonsterCard.spawnCard;
					DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule
					{
						placementMode = DirectorPlacementRule.PlacementMode.Approximate,
						spawnOnTarget = spawnTarget.transform,
						preventOverhead = this.currentMonsterCard.preventOverhead
					};
					DirectorCore.GetMonsterSpawnDistance(this.currentMonsterCard.spawnDistance, out directorPlacementRule.minDistance, out directorPlacementRule.maxDistance);
					directorPlacementRule.minDistance *= this.spawnDistanceMultiplier;
					directorPlacementRule.maxDistance *= this.spawnDistanceMultiplier;
					GameObject gameObject = DirectorCore.instance.TrySpawnObject(spawnCard, directorPlacementRule, this.rng);
					if (gameObject)
					{
						int num2 = this.currentMonsterCard.cost;
						float num3 = 1f;
						float num4 = 1f;
						CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
						GameObject bodyObject = component.GetBodyObject();
						if (this.isBoss)
						{
							if (!this.bossGroup)
							{
								GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/BossGroup"));
								NetworkServer.Spawn(gameObject2);
								this.bossGroup = gameObject2.GetComponent<BossGroup>();
								this.bossGroup.dropPosition = this.dropPosition;
							}
							this.bossGroup.AddMember(component);
						}
						if (flag && (float)num2 * CombatDirector.eliteMultiplierCost <= this.monsterCredit)
						{
							num3 = 4.7f;
							num4 = 2f;
							component.inventory.SetEquipmentIndex(EliteCatalog.GetEliteDef(this.currentActiveEliteIndex).eliteEquipmentIndex);
							num2 = (int)((float)num2 * CombatDirector.eliteMultiplierCost);
						}
						int num5 = num2;
						this.monsterCredit -= (float)num5;
						if (this.isBoss)
						{
							int livingPlayerCount = Run.instance.livingPlayerCount;
							num3 *= Mathf.Pow((float)livingPlayerCount, 1f);
						}
						component.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num3 - 1f) * 10f));
						component.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((num4 - 1f) * 10f));
						DeathRewards component2 = bodyObject.GetComponent<DeathRewards>();
						if (component2)
						{
							component2.expReward = (uint)((float)num2 * this.expRewardCoefficient * Run.instance.compensatedDifficultyCoefficient);
							component2.goldReward = (uint)((float)num2 * this.expRewardCoefficient * 2f * Run.instance.compensatedDifficultyCoefficient);
						}
						if (this.spawnEffectPrefab && NetworkServer.active)
						{
							Vector3 origin = gameObject.transform.position;
							CharacterBody component3 = bodyObject.GetComponent<CharacterBody>();
							if (component3)
							{
								origin = component3.corePosition;
							}
							EffectManager.instance.SpawnEffect(this.spawnEffectPrefab, new EffectData
							{
								origin = origin
							}, true);
						}
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000DDA RID: 3546 RVA: 0x0005627C File Offset: 0x0005447C
		private void FixedUpdate()
		{
			if (CombatDirector.cvDirectorCombatDisable.value)
			{
				return;
			}
			if (NetworkServer.active && Run.instance)
			{
				float compensatedDifficultyCoefficient = Run.instance.compensatedDifficultyCoefficient;
				for (int i = 0; i < this.moneyWaves.Length; i++)
				{
					float num = this.moneyWaves[i].Update(Time.fixedDeltaTime, compensatedDifficultyCoefficient);
					this.monsterCredit += num;
				}
				this.Simulate(Time.fixedDeltaTime);
			}
		}

		// Token: 0x040011BD RID: 4541
		public float monsterCredit;

		// Token: 0x040011BE RID: 4542
		[HideInInspector]
		public float monsterSpawnTimer;

		// Token: 0x040011BF RID: 4543
		[HideInInspector]
		public DirectorCard lastAttemptedMonsterCard;

		// Token: 0x040011C0 RID: 4544
		private DirectorCard currentMonsterCard;

		// Token: 0x040011C1 RID: 4545
		private EliteIndex currentActiveEliteIndex;

		// Token: 0x040011C2 RID: 4546
		public float expRewardCoefficient = 0.2f;

		// Token: 0x040011C3 RID: 4547
		public float minSeriesSpawnInterval = 0.1f;

		// Token: 0x040011C4 RID: 4548
		public float maxSeriesSpawnInterval = 1f;

		// Token: 0x040011C5 RID: 4549
		public float minRerollSpawnInterval = 2.33333325f;

		// Token: 0x040011C6 RID: 4550
		public float maxRerollSpawnInterval = 4.33333349f;

		// Token: 0x040011C7 RID: 4551
		public bool isBoss;

		// Token: 0x040011C8 RID: 4552
		public bool shouldSpawnOneWave;

		// Token: 0x040011C9 RID: 4553
		public bool targetPlayers = true;

		// Token: 0x040011CA RID: 4554
		public bool skipSpawnIfTooCheap = true;

		// Token: 0x040011CB RID: 4555
		private bool hasStartedWave;

		// Token: 0x040011CC RID: 4556
		public RangeFloat[] moneyWaveIntervals;

		// Token: 0x040011CD RID: 4557
		public static readonly List<CombatDirector> instancesList = new List<CombatDirector>();

		// Token: 0x040011CE RID: 4558
		[Tooltip("How much to multiply money wave yield by.")]
		public float creditMultiplier = 1f;

		// Token: 0x040011CF RID: 4559
		[Tooltip("The coefficient to multiply spawn distances. Used for combat shrines, to keep spawns nearby.")]
		public float spawnDistanceMultiplier = 1f;

		// Token: 0x040011D1 RID: 4561
		[Tooltip("The position from which a reward will be dropped when the associated BossGroup is defeated.")]
		public Transform dropPosition;

		// Token: 0x040011D2 RID: 4562
		[Tooltip("A special effect for when a monster appears will be instantiated at its position. Used for combat shrine.")]
		public GameObject spawnEffectPrefab;

		// Token: 0x040011D3 RID: 4563
		private Xoroshiro128Plus rng;

		// Token: 0x040011D4 RID: 4564
		public GameObject currentSpawnTarget;

		// Token: 0x040011D5 RID: 4565
		private float playerRetargetTimer;

		// Token: 0x040011D6 RID: 4566
		public static float maximumNumberToSpawnBeforeSkipping = 4f;

		// Token: 0x040011D7 RID: 4567
		public static float eliteMultiplierCost = 6f;

		// Token: 0x040011D8 RID: 4568
		private static readonly BoolConVar cvDirectorCombatDisable = new BoolConVar("director_combat_disable", ConVarFlags.SenderMustBeServer | ConVarFlags.Cheat, "0", "Disables all combat directors.");

		// Token: 0x040011D9 RID: 4569
		private CombatDirector.DirectorMoneyWave[] moneyWaves;

		// Token: 0x020002A6 RID: 678
		private class DirectorMoneyWave
		{
			// Token: 0x06000DDD RID: 3549 RVA: 0x00056364 File Offset: 0x00054564
			public float Update(float deltaTime, float difficultyCoefficient)
			{
				this.timer += deltaTime;
				if (this.timer > this.interval)
				{
					float num = 0.5f + (float)Run.instance.participatingPlayerCount * 0.5f;
					this.timer -= this.interval;
					float num2 = 1f;
					float num3 = 0.4f;
					this.accumulatedAward += this.interval * this.multiplier * (num2 + num3 * difficultyCoefficient) * num;
				}
				float num4 = (float)Mathf.FloorToInt(this.accumulatedAward);
				this.accumulatedAward -= num4;
				return num4;
			}

			// Token: 0x040011DA RID: 4570
			public float interval;

			// Token: 0x040011DB RID: 4571
			public float timer;

			// Token: 0x040011DC RID: 4572
			public float multiplier;

			// Token: 0x040011DD RID: 4573
			private float accumulatedAward;
		}
	}
}
