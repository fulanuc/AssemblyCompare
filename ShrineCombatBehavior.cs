using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E0 RID: 992
	[RequireComponent(typeof(CombatDirector))]
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineCombatBehavior : NetworkBehaviour
	{
		// Token: 0x060015A2 RID: 5538 RVA: 0x0000913D File Offset: 0x0000733D
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x060015A3 RID: 5539 RVA: 0x0001047A File Offset: 0x0000E67A
		private float monsterCredit
		{
			get
			{
				return (float)this.baseMonsterCredit * this.cachedDifficultyCoefficient * (1f + (float)this.purchaseCount * (this.monsterCreditCoefficientPerPurchase - 1f));
			}
		}

		// Token: 0x060015A4 RID: 5540 RVA: 0x000741C8 File Offset: 0x000723C8
		private void Start()
		{
			this.cachedDifficultyCoefficient = Run.instance.difficultyCoefficient;
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
			this.combatDirector = base.GetComponent<CombatDirector>();
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
				this.InitCombatShrineValues();
			}
		}

		// Token: 0x060015A5 RID: 5541 RVA: 0x00074224 File Offset: 0x00072424
		private void InitCombatShrineValues()
		{
			WeightedSelection<DirectorCard> monsterSelection = ClassicStageInfo.instance.monsterSelection;
			WeightedSelection<DirectorCard> weightedSelection = new WeightedSelection<DirectorCard>(8);
			for (int i = 0; i < monsterSelection.Count; i++)
			{
				DirectorCard value = monsterSelection.choices[i].value;
				if ((float)value.cost <= this.monsterCredit && (float)value.cost * CombatDirector.maximumNumberToSpawnBeforeSkipping * CombatDirector.eliteMultiplierCost / 2f > this.monsterCredit && value.CardIsValid())
				{
					weightedSelection.AddChoice(value, monsterSelection.choices[i].weight);
				}
			}
			if (weightedSelection.Count == 0)
			{
				if (this.chosenDirectorCard == null)
				{
					Debug.Log("Could not find appropriate spawn card for Combat Shrine");
					this.purchaseInteraction.SetAvailable(false);
				}
				return;
			}
			this.chosenDirectorCard = weightedSelection.Evaluate(this.rng.nextNormalizedFloat);
		}

		// Token: 0x060015A6 RID: 5542 RVA: 0x000742F4 File Offset: 0x000724F4
		public void FixedUpdate()
		{
			if (this.waitingForRefresh)
			{
				this.refreshTimer -= Time.fixedDeltaTime;
				if (this.refreshTimer <= 0f && this.purchaseCount < this.maxPurchaseCount)
				{
					this.purchaseInteraction.SetAvailable(true);
					this.waitingForRefresh = false;
				}
			}
		}

		// Token: 0x060015A7 RID: 5543 RVA: 0x0007434C File Offset: 0x0007254C
		[Server]
		public void AddShrineStack(Interactor interactor)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineCombatBehavior::AddShrineStack(RoR2.Interactor)' called on client");
				return;
			}
			this.waitingForRefresh = true;
			if (this.combatDirector)
			{
				this.combatDirector.OverrideCurrentMonsterCard(this.chosenDirectorCard);
				this.combatDirector.enabled = true;
				this.combatDirector.monsterCredit += this.monsterCredit;
				this.combatDirector.monsterSpawnTimer = 0f;
			}
			CharacterMaster component = this.chosenDirectorCard.spawnCard.prefab.GetComponent<CharacterMaster>();
			if (component)
			{
				CharacterBody component2 = component.bodyPrefab.GetComponent<CharacterBody>();
				if (component2)
				{
					Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
					{
						subjectCharacterBodyGameObject = interactor.gameObject,
						baseToken = "SHRINE_COMBAT_USE_MESSAGE",
						paramTokens = new string[]
						{
							component2.baseNameToken
						}
					});
				}
			}
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
			{
				origin = base.transform.position,
				rotation = Quaternion.identity,
				scale = 1f,
				color = this.shrineEffectColor
			}, true);
			this.purchaseCount++;
			this.refreshTimer = 2f;
			if (this.purchaseCount >= this.maxPurchaseCount)
			{
				this.symbolTransform.gameObject.SetActive(false);
			}
		}

		// Token: 0x060015A9 RID: 5545 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x060015AA RID: 5546 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018DD RID: 6365
		public Color shrineEffectColor;

		// Token: 0x040018DE RID: 6366
		public int maxPurchaseCount;

		// Token: 0x040018DF RID: 6367
		public int baseMonsterCredit;

		// Token: 0x040018E0 RID: 6368
		public float monsterCreditCoefficientPerPurchase;

		// Token: 0x040018E1 RID: 6369
		public Transform symbolTransform;

		// Token: 0x040018E2 RID: 6370
		public GameObject spawnPositionEffectPrefab;

		// Token: 0x040018E3 RID: 6371
		private CombatDirector combatDirector;

		// Token: 0x040018E4 RID: 6372
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x040018E5 RID: 6373
		private int purchaseCount;

		// Token: 0x040018E6 RID: 6374
		private float refreshTimer;

		// Token: 0x040018E7 RID: 6375
		private const float refreshDuration = 2f;

		// Token: 0x040018E8 RID: 6376
		private bool waitingForRefresh;

		// Token: 0x040018E9 RID: 6377
		private Xoroshiro128Plus rng;

		// Token: 0x040018EA RID: 6378
		private DirectorCard chosenDirectorCard;

		// Token: 0x040018EB RID: 6379
		private float cachedDifficultyCoefficient;
	}
}
