using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E5 RID: 997
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineChanceBehavior : NetworkBehaviour
	{
		// Token: 0x1400002C RID: 44
		// (add) Token: 0x060015D5 RID: 5589 RVA: 0x000744CC File Offset: 0x000726CC
		// (remove) Token: 0x060015D6 RID: 5590 RVA: 0x00074500 File Offset: 0x00072700
		public static event Action<bool, Interactor> onShrineChancePurchaseGlobal;

		// Token: 0x060015D7 RID: 5591 RVA: 0x00010852 File Offset: 0x0000EA52
		private void Awake()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x060015D8 RID: 5592 RVA: 0x00010860 File Offset: 0x0000EA60
		public void Start()
		{
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
			}
		}

		// Token: 0x060015D9 RID: 5593 RVA: 0x00074534 File Offset: 0x00072734
		public void FixedUpdate()
		{
			if (this.waitingForRefresh)
			{
				this.refreshTimer -= Time.fixedDeltaTime;
				if (this.refreshTimer <= 0f && this.successfulPurchaseCount < this.maxPurchaseCount)
				{
					this.purchaseInteraction.SetAvailable(true);
					this.purchaseInteraction.Networkcost = (int)((float)this.purchaseInteraction.cost * this.costMultiplierPerPurchase);
					this.waitingForRefresh = false;
				}
			}
		}

		// Token: 0x060015DA RID: 5594 RVA: 0x000745A8 File Offset: 0x000727A8
		[Server]
		public void AddShrineStack(Interactor activator)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineChanceBehavior::AddShrineStack(RoR2.Interactor)' called on client");
				return;
			}
			PickupIndex none = PickupIndex.none;
			PickupIndex value = Run.instance.availableTier1DropList[this.rng.RangeInt(0, Run.instance.availableTier1DropList.Count - 1)];
			PickupIndex value2 = Run.instance.availableTier2DropList[this.rng.RangeInt(0, Run.instance.availableTier2DropList.Count - 1)];
			PickupIndex value3 = Run.instance.availableTier3DropList[this.rng.RangeInt(0, Run.instance.availableTier3DropList.Count - 1)];
			PickupIndex value4 = Run.instance.availableEquipmentDropList[this.rng.RangeInt(0, Run.instance.availableEquipmentDropList.Count - 1)];
			WeightedSelection<PickupIndex> weightedSelection = new WeightedSelection<PickupIndex>(8);
			weightedSelection.AddChoice(none, this.failureWeight);
			weightedSelection.AddChoice(value, this.tier1Weight);
			weightedSelection.AddChoice(value2, this.tier2Weight);
			weightedSelection.AddChoice(value3, this.tier3Weight);
			weightedSelection.AddChoice(value4, this.equipmentWeight);
			PickupIndex pickupIndex = weightedSelection.Evaluate(this.rng.nextNormalizedFloat);
			bool flag = pickupIndex == PickupIndex.none;
			if (flag)
			{
				Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
				{
					subjectCharacterBodyGameObject = activator.gameObject,
					baseToken = "SHRINE_CHANCE_FAIL_MESSAGE"
				});
			}
			else
			{
				this.successfulPurchaseCount++;
				PickupDropletController.CreatePickupDroplet(pickupIndex, this.dropletOrigin.position, this.dropletOrigin.forward * 20f);
				Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
				{
					subjectCharacterBodyGameObject = activator.gameObject,
					baseToken = "SHRINE_CHANCE_SUCCESS_MESSAGE"
				});
			}
			Action<bool, Interactor> action = ShrineChanceBehavior.onShrineChancePurchaseGlobal;
			if (action != null)
			{
				action(flag, activator);
			}
			this.waitingForRefresh = true;
			this.refreshTimer = 2f;
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
			{
				origin = base.transform.position,
				rotation = Quaternion.identity,
				scale = 1f,
				color = this.shrineColor
			}, true);
			if (this.successfulPurchaseCount >= this.maxPurchaseCount)
			{
				this.symbolTransform.gameObject.SetActive(false);
			}
		}

		// Token: 0x060015DC RID: 5596 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x060015DD RID: 5597 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060015DE RID: 5598 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018F5 RID: 6389
		public int maxPurchaseCount;

		// Token: 0x040018F6 RID: 6390
		public float costMultiplierPerPurchase;

		// Token: 0x040018F7 RID: 6391
		public float failureWeight;

		// Token: 0x040018F8 RID: 6392
		public float equipmentWeight;

		// Token: 0x040018F9 RID: 6393
		public float tier1Weight;

		// Token: 0x040018FA RID: 6394
		public float tier2Weight;

		// Token: 0x040018FB RID: 6395
		public float tier3Weight;

		// Token: 0x040018FC RID: 6396
		public Transform symbolTransform;

		// Token: 0x040018FD RID: 6397
		public Transform dropletOrigin;

		// Token: 0x040018FE RID: 6398
		public Color shrineColor;

		// Token: 0x040018FF RID: 6399
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x04001900 RID: 6400
		private int successfulPurchaseCount;

		// Token: 0x04001901 RID: 6401
		private float refreshTimer;

		// Token: 0x04001902 RID: 6402
		private const float refreshDuration = 2f;

		// Token: 0x04001903 RID: 6403
		private bool waitingForRefresh;

		// Token: 0x04001905 RID: 6405
		private Xoroshiro128Plus rng;
	}
}
