using System;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000266 RID: 614
	public class BazaarController : MonoBehaviour
	{
		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000B6D RID: 2925 RVA: 0x0000918A File Offset: 0x0000738A
		// (set) Token: 0x06000B6E RID: 2926 RVA: 0x00009191 File Offset: 0x00007391
		public static BazaarController instance { get; private set; }

		// Token: 0x06000B6F RID: 2927 RVA: 0x00009199 File Offset: 0x00007399
		private void Awake()
		{
			BazaarController.instance = SingletonHelper.Assign<BazaarController>(BazaarController.instance, this);
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x000025F6 File Offset: 0x000007F6
		private void Start()
		{
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x000091AB File Offset: 0x000073AB
		private void OnDestroy()
		{
			BazaarController.instance = SingletonHelper.Unassign<BazaarController>(BazaarController.instance, this);
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x0004BCA8 File Offset: 0x00049EA8
		public void CommentOnAnnoy()
		{
			float percentChance = 20f;
			int max = 6;
			if (Util.CheckRoll(percentChance, 0f, null))
			{
				Chat.SendBroadcastChat(new Chat.NpcChatMessage
				{
					sender = this.shopkeeper,
					baseToken = "NEWT_ANNOY_" + UnityEngine.Random.Range(1, max)
				});
			}
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x000025F6 File Offset: 0x000007F6
		public void CommentOnEnter()
		{
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x000025F6 File Offset: 0x000007F6
		public void CommentOnLeaving()
		{
		}

		// Token: 0x06000B75 RID: 2933 RVA: 0x0004BCFC File Offset: 0x00049EFC
		public void CommentOnLunarPurchase()
		{
			float percentChance = 20f;
			int max = 8;
			if (Util.CheckRoll(percentChance, 0f, null))
			{
				Chat.SendBroadcastChat(new Chat.NpcChatMessage
				{
					sender = this.shopkeeper,
					baseToken = "NEWT_LUNAR_PURCHASE_" + UnityEngine.Random.Range(1, max)
				});
			}
		}

		// Token: 0x06000B76 RID: 2934 RVA: 0x000025F6 File Offset: 0x000007F6
		public void CommentOnBlueprintPurchase()
		{
		}

		// Token: 0x06000B77 RID: 2935 RVA: 0x000025F6 File Offset: 0x000007F6
		public void CommentOnDronePurchase()
		{
		}

		// Token: 0x06000B78 RID: 2936 RVA: 0x0004BD50 File Offset: 0x00049F50
		public void CommentOnUpgrade()
		{
			float percentChance = 100f;
			int max = 3;
			if (Util.CheckRoll(percentChance, 0f, null))
			{
				Chat.SendBroadcastChat(new Chat.NpcChatMessage
				{
					sender = this.shopkeeper,
					baseToken = "NEWT_UPGRADE_" + UnityEngine.Random.Range(1, max)
				});
			}
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x0004BDA4 File Offset: 0x00049FA4
		private void Update()
		{
			if (this.shopkeeper)
			{
				if (!this.shopkeeperInputBank)
				{
					this.shopkeeperInputBank = this.shopkeeper.GetComponent<InputBankTest>();
					return;
				}
				Ray aimRay = new Ray(this.shopkeeperInputBank.aimOrigin, this.shopkeeper.transform.forward);
				this.shopkeeperTargetBody = Util.GetEnemyEasyTarget(this.shopkeeper.GetComponent<CharacterBody>(), aimRay, this.shopkeeperTrackDistance, this.shopkeeperTrackAngle);
				if (this.shopkeeperTargetBody)
				{
					Vector3 direction = this.shopkeeperTargetBody.mainHurtBox.transform.position - aimRay.origin;
					aimRay.direction = direction;
				}
				this.shopkeeperInputBank.aimDirection = aimRay.direction;
			}
		}

		// Token: 0x04000F65 RID: 3941
		public GameObject shopkeeper;

		// Token: 0x04000F66 RID: 3942
		public TextMeshPro shopkeeperChat;

		// Token: 0x04000F67 RID: 3943
		public float shopkeeperTrackDistance = 250f;

		// Token: 0x04000F68 RID: 3944
		public float shopkeeperTrackAngle = 120f;

		// Token: 0x04000F69 RID: 3945
		[Tooltip("Any PurchaseInteraction objects here will have their activation state set based on whether or not the specified unlockable is available.")]
		public PurchaseInteraction[] unlockableTerminals;

		// Token: 0x04000F6A RID: 3946
		private InputBankTest shopkeeperInputBank;

		// Token: 0x04000F6B RID: 3947
		private CharacterBody shopkeeperTargetBody;
	}
}
