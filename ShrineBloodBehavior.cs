using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003DD RID: 989
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineBloodBehavior : NetworkBehaviour
	{
		// Token: 0x06001588 RID: 5512 RVA: 0x0000913D File Offset: 0x0000733D
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06001589 RID: 5513 RVA: 0x0001041A File Offset: 0x0000E61A
		private void Start()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x0600158A RID: 5514 RVA: 0x00073B40 File Offset: 0x00071D40
		public void FixedUpdate()
		{
			if (this.waitingForRefresh)
			{
				this.refreshTimer -= Time.fixedDeltaTime;
				if (this.refreshTimer <= 0f && this.purchaseCount < this.maxPurchaseCount)
				{
					this.purchaseInteraction.SetAvailable(true);
					this.purchaseInteraction.Networkcost = (int)(100f * (1f - Mathf.Pow(1f - (float)this.purchaseInteraction.cost / 100f, this.costMultiplierPerPurchase)));
					this.waitingForRefresh = false;
				}
			}
		}

		// Token: 0x0600158B RID: 5515 RVA: 0x00073BD0 File Offset: 0x00071DD0
		[Server]
		public void AddShrineStack(Interactor interactor)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineBloodBehavior::AddShrineStack(RoR2.Interactor)' called on client");
				return;
			}
			this.waitingForRefresh = true;
			CharacterBody component = interactor.GetComponent<CharacterBody>();
			if (component)
			{
				uint amount = (uint)(component.healthComponent.fullHealth * (float)this.purchaseInteraction.cost / 100f * this.goldToPaidHpRatio);
				if (component.master)
				{
					component.master.GiveMoney(amount);
					Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
					{
						subjectCharacterBodyGameObject = interactor.gameObject,
						baseToken = "SHRINE_BLOOD_USE_MESSAGE",
						paramTokens = new string[]
						{
							amount.ToString()
						}
					});
				}
			}
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
			{
				origin = base.transform.position,
				rotation = Quaternion.identity,
				scale = 1f,
				color = Color.red
			}, true);
			this.purchaseCount++;
			this.refreshTimer = 2f;
			if (this.purchaseCount >= this.maxPurchaseCount)
			{
				this.symbolTransform.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018BB RID: 6331
		public int maxPurchaseCount;

		// Token: 0x040018BC RID: 6332
		public float goldToPaidHpRatio = 0.5f;

		// Token: 0x040018BD RID: 6333
		public float costMultiplierPerPurchase;

		// Token: 0x040018BE RID: 6334
		public Transform symbolTransform;

		// Token: 0x040018BF RID: 6335
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x040018C0 RID: 6336
		private int purchaseCount;

		// Token: 0x040018C1 RID: 6337
		private float refreshTimer;

		// Token: 0x040018C2 RID: 6338
		private const float refreshDuration = 2f;

		// Token: 0x040018C3 RID: 6339
		private bool waitingForRefresh;
	}
}
