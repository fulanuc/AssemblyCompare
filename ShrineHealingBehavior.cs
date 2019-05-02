using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E1 RID: 993
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineHealingBehavior : NetworkBehaviour
	{
		// Token: 0x060015AC RID: 5548 RVA: 0x0000913D File Offset: 0x0000733D
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x060015AD RID: 5549 RVA: 0x000104A5 File Offset: 0x0000E6A5
		// (set) Token: 0x060015AE RID: 5550 RVA: 0x000104AD File Offset: 0x0000E6AD
		public int purchaseCount { get; private set; }

		// Token: 0x060015AF RID: 5551 RVA: 0x000104B6 File Offset: 0x0000E6B6
		private void Awake()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x060015B0 RID: 5552 RVA: 0x000744BC File Offset: 0x000726BC
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

		// Token: 0x060015B1 RID: 5553 RVA: 0x00074530 File Offset: 0x00072730
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

		// Token: 0x060015B2 RID: 5554 RVA: 0x000745D4 File Offset: 0x000727D4
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

		// Token: 0x1400002B RID: 43
		// (add) Token: 0x060015B3 RID: 5555 RVA: 0x000746E8 File Offset: 0x000728E8
		// (remove) Token: 0x060015B4 RID: 5556 RVA: 0x0007471C File Offset: 0x0007291C
		public static event Action<ShrineHealingBehavior, Interactor> onActivated;

		// Token: 0x060015B6 RID: 5558 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x060015B7 RID: 5559 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060015B8 RID: 5560 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018EC RID: 6380
		public GameObject wardPrefab;

		// Token: 0x040018ED RID: 6381
		private GameObject wardInstance;

		// Token: 0x040018EE RID: 6382
		public float baseRadius;

		// Token: 0x040018EF RID: 6383
		public float radiusBonusPerPurchase;

		// Token: 0x040018F0 RID: 6384
		public int maxPurchaseCount;

		// Token: 0x040018F1 RID: 6385
		public float costMultiplierPerPurchase;

		// Token: 0x040018F2 RID: 6386
		public Transform symbolTransform;

		// Token: 0x040018F3 RID: 6387
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x040018F5 RID: 6389
		private float refreshTimer;

		// Token: 0x040018F6 RID: 6390
		private const float refreshDuration = 2f;

		// Token: 0x040018F7 RID: 6391
		private bool waitingForRefresh;

		// Token: 0x040018F8 RID: 6392
		private HealingWard healingWard;
	}
}
