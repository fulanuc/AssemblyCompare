using System;

namespace RoR2.Achievements
{
	// Token: 0x020006AE RID: 1710
	[RegisterAchievement("RepeatedlyDuplicateItems", "Items.Firework", null, typeof(RepeatedlyDuplicateItemsAchievement.RepeatedlyDuplicateItemsServerAchievement))]
	public class RepeatedlyDuplicateItemsAchievement : BaseAchievement
	{
		// Token: 0x060025F9 RID: 9721 RVA: 0x0001B314 File Offset: 0x00019514
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060025FA RID: 9722 RVA: 0x0001B323 File Offset: 0x00019523
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006AF RID: 1711
		private class RepeatedlyDuplicateItemsServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025FC RID: 9724 RVA: 0x0001BBFB File Offset: 0x00019DFB
			public override void OnInstall()
			{
				base.OnInstall();
				PurchaseInteraction.onItemSpentOnPurchase += this.OnItemSpentOnPurchase;
				Run.onRunStartGlobal += this.OnRunStartGlobal;
			}

			// Token: 0x060025FD RID: 9725 RVA: 0x0001BC25 File Offset: 0x00019E25
			public override void OnUninstall()
			{
				base.OnInstall();
				PurchaseInteraction.onItemSpentOnPurchase -= this.OnItemSpentOnPurchase;
				Run.onRunStartGlobal -= this.OnRunStartGlobal;
			}

			// Token: 0x060025FE RID: 9726 RVA: 0x0001BC4F File Offset: 0x00019E4F
			private void OnRunStartGlobal(Run run)
			{
				this.progress = 0;
			}

			// Token: 0x060025FF RID: 9727 RVA: 0x000B0A98 File Offset: 0x000AEC98
			private void OnItemSpentOnPurchase(PurchaseInteraction purchaseInteraction, Interactor interactor)
			{
				CharacterBody currentBody = this.serverAchievementTracker.networkUser.GetCurrentBody();
				if (currentBody && currentBody.GetComponent<Interactor>() == interactor && purchaseInteraction.gameObject.name.Contains("Duplicator"))
				{
					ShopTerminalBehavior component = purchaseInteraction.GetComponent<ShopTerminalBehavior>();
					if (component)
					{
						ItemIndex itemIndex = component.CurrentPickupIndex().itemIndex;
						if (this.trackingItemIndex != itemIndex)
						{
							this.trackingItemIndex = itemIndex;
							this.progress = 0;
						}
						this.progress++;
						if (this.progress >= 7)
						{
							base.Grant();
						}
					}
				}
			}

			// Token: 0x04002851 RID: 10321
			private const int requirement = 7;

			// Token: 0x04002852 RID: 10322
			private ItemIndex trackingItemIndex = ItemIndex.None;

			// Token: 0x04002853 RID: 10323
			private int progress;
		}
	}
}
