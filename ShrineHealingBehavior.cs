using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E7 RID: 999
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineHealingBehavior : NetworkBehaviour
	{
		// Token: 0x060015E9 RID: 5609 RVA: 0x00009162 File Offset: 0x00007362
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x060015EA RID: 5610 RVA: 0x000108AE File Offset: 0x0000EAAE
		// (set) Token: 0x060015EB RID: 5611 RVA: 0x000108B6 File Offset: 0x0000EAB6
		public int purchaseCount { get; private set; }

		// Token: 0x060015EC RID: 5612 RVA: 0x000108BF File Offset: 0x0000EABF
		private void Awake()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x060015ED RID: 5613 RVA: 0x00074AF4 File Offset: 0x00072CF4
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

		// Token: 0x060015EE RID: 5614 RVA: 0x00074B68 File Offset: 0x00072D68
		[Server]
		private void SetWardEnabled(bool enableWard)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineHealingBehavior::SetWardEnabled(System.Boolean)' called on client");
				return;
			}
			if (enableWard != this.wardInstance)
			{
				if (enableWard)
				{
					this.wardInstance = UnityEngine.Object.Instantiate<GameObject>(this.wardPrefab, base.transform.position, base.transform.rotation);
					this.wardInstance.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;
					this.healingWard = this.wardInstance.GetComponent<HealingWard>();
					NetworkServer.Spawn(this.wardInstance);
					return;
				}
				UnityEngine.Object.Destroy(this.wardInstance);
				this.wardInstance = null;
				this.healingWard = null;
			}
		}

		// Token: 0x060015EF RID: 5615 RVA: 0x00074C0C File Offset: 0x00072E0C
		[Server]
		public void AddShrineStack(Interactor activator)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineHealingBehavior::AddShrineStack(RoR2.Interactor)' called on client");
				return;
			}
			this.SetWardEnabled(true);
			Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
			{
				subjectCharacterBodyGameObject = activator.gameObject,
				baseToken = "SHRINE_HEALING_USE_MESSAGE"
			});
			this.waitingForRefresh = true;
			int purchaseCount = this.purchaseCount;
			this.purchaseCount = purchaseCount + 1;
			float networkradius = this.baseRadius + this.radiusBonusPerPurchase * (float)(this.purchaseCount - 1);
			this.healingWard.Networkradius = networkradius;
			this.refreshTimer = 2f;
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
			{
				origin = base.transform.position,
				rotation = Quaternion.identity,
				scale = 1f,
				color = Color.green
			}, true);
			if (this.purchaseCount >= this.maxPurchaseCount)
			{
				this.symbolTransform.gameObject.SetActive(false);
			}
			Action<ShrineHealingBehavior, Interactor> action = ShrineHealingBehavior.onActivated;
			if (action == null)
			{
				return;
			}
			action(this, activator);
		}

		// Token: 0x1400002D RID: 45
		// (add) Token: 0x060015F0 RID: 5616 RVA: 0x00074D20 File Offset: 0x00072F20
		// (remove) Token: 0x060015F1 RID: 5617 RVA: 0x00074D54 File Offset: 0x00072F54
		public static event Action<ShrineHealingBehavior, Interactor> onActivated;

		// Token: 0x060015F3 RID: 5619 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x060015F4 RID: 5620 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060015F5 RID: 5621 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001915 RID: 6421
		public GameObject wardPrefab;

		// Token: 0x04001916 RID: 6422
		private GameObject wardInstance;

		// Token: 0x04001917 RID: 6423
		public float baseRadius;

		// Token: 0x04001918 RID: 6424
		public float radiusBonusPerPurchase;

		// Token: 0x04001919 RID: 6425
		public int maxPurchaseCount;

		// Token: 0x0400191A RID: 6426
		public float costMultiplierPerPurchase;

		// Token: 0x0400191B RID: 6427
		public Transform symbolTransform;

		// Token: 0x0400191C RID: 6428
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x0400191E RID: 6430
		private float refreshTimer;

		// Token: 0x0400191F RID: 6431
		private const float refreshDuration = 2f;

		// Token: 0x04001920 RID: 6432
		private bool waitingForRefresh;

		// Token: 0x04001921 RID: 6433
		private HealingWard healingWard;
	}
}
