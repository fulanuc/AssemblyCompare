using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E4 RID: 996
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineBossBehavior : NetworkBehaviour
	{
		// Token: 0x060015CD RID: 5581 RVA: 0x00009162 File Offset: 0x00007362
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x060015CE RID: 5582 RVA: 0x00010844 File Offset: 0x0000EA44
		private void Start()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x060015CF RID: 5583 RVA: 0x00074344 File Offset: 0x00072544
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

		// Token: 0x060015D0 RID: 5584 RVA: 0x000743B8 File Offset: 0x000725B8
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

		// Token: 0x060015D2 RID: 5586 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x060015D3 RID: 5587 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018ED RID: 6381
		public int maxPurchaseCount;

		// Token: 0x040018EE RID: 6382
		public float costMultiplierPerPurchase;

		// Token: 0x040018EF RID: 6383
		public Transform symbolTransform;

		// Token: 0x040018F0 RID: 6384
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x040018F1 RID: 6385
		private int purchaseCount;

		// Token: 0x040018F2 RID: 6386
		private float refreshTimer;

		// Token: 0x040018F3 RID: 6387
		private const float refreshDuration = 2f;

		// Token: 0x040018F4 RID: 6388
		private bool waitingForRefresh;
	}
}
