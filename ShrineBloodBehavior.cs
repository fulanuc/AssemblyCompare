using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E3 RID: 995
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineBloodBehavior : NetworkBehaviour
	{
		// Token: 0x060015C5 RID: 5573 RVA: 0x00009162 File Offset: 0x00007362
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x060015C6 RID: 5574 RVA: 0x00010823 File Offset: 0x0000EA23
		private void Start()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x060015C7 RID: 5575 RVA: 0x00074178 File Offset: 0x00072378
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

		// Token: 0x060015C8 RID: 5576 RVA: 0x00074208 File Offset: 0x00072408
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

		// Token: 0x060015CA RID: 5578 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x060015CB RID: 5579 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060015CC RID: 5580 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018E4 RID: 6372
		public int maxPurchaseCount;

		// Token: 0x040018E5 RID: 6373
		public float goldToPaidHpRatio = 0.5f;

		// Token: 0x040018E6 RID: 6374
		public float costMultiplierPerPurchase;

		// Token: 0x040018E7 RID: 6375
		public Transform symbolTransform;

		// Token: 0x040018E8 RID: 6376
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x040018E9 RID: 6377
		private int purchaseCount;

		// Token: 0x040018EA RID: 6378
		private float refreshTimer;

		// Token: 0x040018EB RID: 6379
		private const float refreshDuration = 2f;

		// Token: 0x040018EC RID: 6380
		private bool waitingForRefresh;
	}
}
