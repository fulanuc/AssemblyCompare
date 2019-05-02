using System;

namespace RoR2.Achievements
{
	// Token: 0x020006C0 RID: 1728
	[RegisterAchievement("RepeatedlyDuplicateItems", "Items.Firework", null, typeof(RepeatedlyDuplicateItemsAchievement.RepeatedlyDuplicateItemsServerAchievement))]
	public class RepeatedlyDuplicateItemsAchievement : BaseAchievement
	{
		// Token: 0x06002690 RID: 9872 RVA: 0x0001BA4F File Offset: 0x00019C4F
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002691 RID: 9873 RVA: 0x0001BA5E File Offset: 0x00019C5E
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006C1 RID: 1729
		private class RepeatedlyDuplicateItemsServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002693 RID: 9875 RVA: 0x0001C336 File Offset: 0x0001A536
			public override void OnInstall()
			{
				base.OnInstall();
				PurchaseInteraction.onItemSpentOnPurchase += this.OnItemSpentOnPurchase;
				Run.onRunStartGlobal += this.OnRunStartGlobal;
			}

			// Token: 0x06002694 RID: 9876 RVA: 0x0001C360 File Offset: 0x0001A560
			public override void OnUninstall()
			{
				base.OnInstall();
				PurchaseInteraction.onItemSpentOnPurchase -= this.OnItemSpentOnPurchase;
				Run.onRunStartGlobal -= this.OnRunStartGlobal;
			}

			// Token: 0x06002695 RID: 9877 RVA: 0x0001C38A File Offset: 0x0001A58A
			private void OnRunStartGlobal(Run run)
			{
				this.progress = 0;
			}

			// Token: 0x06002696 RID: 9878 RVA: 0x000B2198 File Offset: 0x000B0398
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

			// Token: 0x040028AD RID: 10413
			private const int requirement = 7;

			// Token: 0x040028AE RID: 10414
			private ItemIndex trackingItemIndex = ItemIndex.None;

			// Token: 0x040028AF RID: 10415
			private int progress;
		}
	}
}
