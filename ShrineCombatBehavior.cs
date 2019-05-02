using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E6 RID: 998
	[RequireComponent(typeof(CombatDirector))]
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineCombatBehavior : NetworkBehaviour
	{
		// Token: 0x060015DF RID: 5599 RVA: 0x00009162 File Offset: 0x00007362
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x060015E0 RID: 5600 RVA: 0x00010883 File Offset: 0x0000EA83
		private float monsterCredit
		{
			get
			{
				return (float)this.baseMonsterCredit * this.cachedDifficultyCoefficient * (1f + (float)this.purchaseCount * (this.monsterCreditCoefficientPerPurchase - 1f));
			}
		}

		// Token: 0x060015E1 RID: 5601 RVA: 0x00074800 File Offset: 0x00072A00
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

		// Token: 0x060015E2 RID: 5602 RVA: 0x0007485C File Offset: 0x00072A5C
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

		// Token: 0x060015E3 RID: 5603 RVA: 0x0007492C File Offset: 0x00072B2C
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

		// Token: 0x060015E4 RID: 5604 RVA: 0x00074984 File Offset: 0x00072B84
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

		// Token: 0x060015E6 RID: 5606 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x060015E7 RID: 5607 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060015E8 RID: 5608 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001906 RID: 6406
		public Color shrineEffectColor;

		// Token: 0x04001907 RID: 6407
		public int maxPurchaseCount;

		// Token: 0x04001908 RID: 6408
		public int baseMonsterCredit;

		// Token: 0x04001909 RID: 6409
		public float monsterCreditCoefficientPerPurchase;

		// Token: 0x0400190A RID: 6410
		public Transform symbolTransform;

		// Token: 0x0400190B RID: 6411
		public GameObject spawnPositionEffectPrefab;

		// Token: 0x0400190C RID: 6412
		private CombatDirector combatDirector;

		// Token: 0x0400190D RID: 6413
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x0400190E RID: 6414
		private int purchaseCount;

		// Token: 0x0400190F RID: 6415
		private float refreshTimer;

		// Token: 0x04001910 RID: 6416
		private const float refreshDuration = 2f;

		// Token: 0x04001911 RID: 6417
		private bool waitingForRefresh;

		// Token: 0x04001912 RID: 6418
		private Xoroshiro128Plus rng;

		// Token: 0x04001913 RID: 6419
		private DirectorCard chosenDirectorCard;

		// Token: 0x04001914 RID: 6420
		private float cachedDifficultyCoefficient;
	}
}
