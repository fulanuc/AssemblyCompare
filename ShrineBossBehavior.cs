using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003DE RID: 990
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineBossBehavior : NetworkBehaviour
	{
		// Token: 0x06001590 RID: 5520 RVA: 0x0000913D File Offset: 0x0000733D
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x0001043B File Offset: 0x0000E63B
		private void Start()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x00073D0C File Offset: 0x00071F0C
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

		// Token: 0x06001593 RID: 5523 RVA: 0x00073D80 File Offset: 0x00071F80
		[Server]
		public void AddShrineStack(Interactor interactor)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineBossBehavior::AddShrineStack(RoR2.Interactor)' called on client");
				return;
			}
			this.waitingForRefresh = true;
			if (TeleporterInteraction.instance)
			{
				TeleporterInteraction.instance.AddShrineStack();
			}
			CharacterBody component = interactor.GetComponent<CharacterBody>();
			if (component && component.master)
			{
				Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
				{
					subjectCharacterBodyGameObject = interactor.gameObject,
					baseToken = "SHRINE_BOSS_USE_MESSAGE"
				});
			}
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
			{
				origin = base.transform.position,
				rotation = Quaternion.identity,
				scale = 1f,
				color = new Color(0.7372549f, 0.905882359f, 0.945098042f)
			}, true);
			this.purchaseCount++;
			this.refreshTimer = 2f;
			if (this.purchaseCount >= this.maxPurchaseCount)
			{
				this.symbolTransform.gameObject.SetActive(false);
			}
		}

		// Token: 0x06001595 RID: 5525 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06001596 RID: 5526 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001597 RID: 5527 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018C4 RID: 6340
		public int maxPurchaseCount;

		// Token: 0x040018C5 RID: 6341
		public float costMultiplierPerPurchase;

		// Token: 0x040018C6 RID: 6342
		public Transform symbolTransform;

		// Token: 0x040018C7 RID: 6343
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x040018C8 RID: 6344
		private int purchaseCount;

		// Token: 0x040018C9 RID: 6345
		private float refreshTimer;

		// Token: 0x040018CA RID: 6346
		private const float refreshDuration = 2f;

		// Token: 0x040018CB RID: 6347
		private bool waitingForRefresh;
	}
}
