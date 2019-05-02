using System;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000266 RID: 614
	public class BazaarController : MonoBehaviour
	{
		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000B73 RID: 2931 RVA: 0x000091CA File Offset: 0x000073CA
		// (set) Token: 0x06000B74 RID: 2932 RVA: 0x000091D1 File Offset: 0x000073D1
		public static BazaarController instance { get; private set; }

		// Token: 0x06000B75 RID: 2933 RVA: 0x000091D9 File Offset: 0x000073D9
		private void Awake()
		{
			BazaarController.instance = SingletonHelper.Assign<BazaarController>(BazaarController.instance, this);
		}

		// Token: 0x06000B76 RID: 2934 RVA: 0x000025DA File Offset: 0x000007DA
		private void Start()
		{
		}

		// Token: 0x06000B77 RID: 2935 RVA: 0x000091EB File Offset: 0x000073EB
		private void OnDestroy()
		{
			BazaarController.instance = SingletonHelper.Unassign<BazaarController>(BazaarController.instance, this);
		}

		// Token: 0x06000B78 RID: 2936 RVA: 0x0004BEB4 File Offset: 0x0004A0B4
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

		// Token: 0x06000B79 RID: 2937 RVA: 0x000025DA File Offset: 0x000007DA
		public void CommentOnEnter()
		{
		}

		// Token: 0x06000B7A RID: 2938 RVA: 0x000025DA File Offset: 0x000007DA
		public void CommentOnLeaving()
		{
		}

		// Token: 0x06000B7B RID: 2939 RVA: 0x0004BF08 File Offset: 0x0004A108
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

		// Token: 0x06000B7C RID: 2940 RVA: 0x000025DA File Offset: 0x000007DA
		public void CommentOnBlueprintPurchase()
		{
		}

		// Token: 0x06000B7D RID: 2941 RVA: 0x000025DA File Offset: 0x000007DA
		public void CommentOnDronePurchase()
		{
		}

		// Token: 0x06000B7E RID: 2942 RVA: 0x0004BF5C File Offset: 0x0004A15C
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

		// Token: 0x06000B7F RID: 2943 RVA: 0x0004BFB0 File Offset: 0x0004A1B0
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

		// Token: 0x04000F6B RID: 3947
		public GameObject shopkeeper;

		// Token: 0x04000F6C RID: 3948
		public TextMeshPro shopkeeperChat;

		// Token: 0x04000F6D RID: 3949
		public float shopkeeperTrackDistance = 250f;

		// Token: 0x04000F6E RID: 3950
		public float shopkeeperTrackAngle = 120f;

		// Token: 0x04000F6F RID: 3951
		[Tooltip("Any PurchaseInteraction objects here will have their activation state set based on whether or not the specified unlockable is available.")]
		public PurchaseInteraction[] unlockableTerminals;

		// Token: 0x04000F70 RID: 3952
		private InputBankTest shopkeeperInputBank;

		// Token: 0x04000F71 RID: 3953
		private CharacterBody shopkeeperTargetBody;
	}
}
