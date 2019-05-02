using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E2 RID: 994
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineRestackBehavior : NetworkBehaviour
	{
		// Token: 0x060015B9 RID: 5561 RVA: 0x0000913D File Offset: 0x0000733D
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x060015BA RID: 5562 RVA: 0x000104C4 File Offset: 0x0000E6C4
		private void Start()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.stageRng.nextUlong);
			}
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x00074750 File Offset: 0x00072950
		public void FixedUpdate()
		{
			if (this.waitingForRefresh)
			{
				this.refreshTimer -= Time.fixedDeltaTime;
				if (this.refreshTimer <= 0f && this.purchaseCount < this.maxPurchaseCount)
				{
					this.purchaseInteraction.SetAvailable(true);
					this.purchaseInteraction.Networkcost = (int)((float)this.purchaseInteraction.cost * this.costMultiplierPerPurchase);
					this.waitingForRefresh = false;
				}
			}
		}

		// Token: 0x060015BC RID: 5564 RVA: 0x000747C4 File Offset: 0x000729C4
		[Server]
		public void AddShrineStack(Interactor interactor)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineRestackBehavior::AddShrineStack(RoR2.Interactor)' called on client");
				return;
			}
			this.waitingForRefresh = true;
			CharacterBody component = interactor.GetComponent<CharacterBody>();
			if (component && component.master)
			{
				Inventory inventory = component.master.inventory;
				if (inventory)
				{
					inventory.ShrineRestackInventory(this.rng);
					Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
					{
						subjectCharacterBodyGameObject = interactor.gameObject,
						baseToken = "SHRINE_RESTACK_USE_MESSAGE"
					});
				}
			}
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
			{
				origin = base.transform.position,
				rotation = Quaternion.identity,
				scale = 1f,
				color = new Color(1f, 0.23f, 0.6337214f)
			}, true);
			this.purchaseCount++;
			this.refreshTimer = 2f;
			if (this.purchaseCount >= this.maxPurchaseCount)
			{
				this.symbolTransform.gameObject.SetActive(false);
			}
		}

		// Token: 0x060015BE RID: 5566 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x060015BF RID: 5567 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060015C0 RID: 5568 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018FA RID: 6394
		public int maxPurchaseCount;

		// Token: 0x040018FB RID: 6395
		public float costMultiplierPerPurchase;

		// Token: 0x040018FC RID: 6396
		public Transform symbolTransform;

		// Token: 0x040018FD RID: 6397
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x040018FE RID: 6398
		private int purchaseCount;

		// Token: 0x040018FF RID: 6399
		private float refreshTimer;

		// Token: 0x04001900 RID: 6400
		private const float refreshDuration = 2f;

		// Token: 0x04001901 RID: 6401
		private bool waitingForRefresh;

		// Token: 0x04001902 RID: 6402
		private Xoroshiro128Plus rng;
	}
}
