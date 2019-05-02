using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E8 RID: 1000
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineRestackBehavior : NetworkBehaviour
	{
		// Token: 0x060015F6 RID: 5622 RVA: 0x00009162 File Offset: 0x00007362
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x060015F7 RID: 5623 RVA: 0x000108CD File Offset: 0x0000EACD
		private void Start()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.stageRng.nextUlong);
			}
		}

		// Token: 0x060015F8 RID: 5624 RVA: 0x00074D88 File Offset: 0x00072F88
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

		// Token: 0x060015F9 RID: 5625 RVA: 0x00074DFC File Offset: 0x00072FFC
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

		// Token: 0x060015FB RID: 5627 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001923 RID: 6435
		public int maxPurchaseCount;

		// Token: 0x04001924 RID: 6436
		public float costMultiplierPerPurchase;

		// Token: 0x04001925 RID: 6437
		public Transform symbolTransform;

		// Token: 0x04001926 RID: 6438
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x04001927 RID: 6439
		private int purchaseCount;

		// Token: 0x04001928 RID: 6440
		private float refreshTimer;

		// Token: 0x04001929 RID: 6441
		private const float refreshDuration = 2f;

		// Token: 0x0400192A RID: 6442
		private bool waitingForRefresh;

		// Token: 0x0400192B RID: 6443
		private Xoroshiro128Plus rng;
	}
}
