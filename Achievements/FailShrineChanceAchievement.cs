using System;

namespace RoR2.Achievements
{
	// Token: 0x020006A7 RID: 1703
	[RegisterAchievement("FailShrineChance", "Items.Hoof", null, typeof(FailShrineChanceAchievement.FailShrineChanceServerAchievement))]
	public class FailShrineChanceAchievement : BaseAchievement
	{
		// Token: 0x0600262B RID: 9771 RVA: 0x0001BA4F File Offset: 0x00019C4F
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x0600262C RID: 9772 RVA: 0x0001BA5E File Offset: 0x00019C5E
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006A8 RID: 1704
		private class FailShrineChanceServerAchievement : BaseServerAchievement
		{
			// Token: 0x0600262E RID: 9774 RVA: 0x0001BE11 File Offset: 0x0001A011
			public override void OnInstall()
			{
				base.OnInstall();
				ShrineChanceBehavior.onShrineChancePurchaseGlobal += this.OnShrineChancePurchase;
				Run.onRunStartGlobal += this.OnRunStartGlobal;
			}

			// Token: 0x0600262F RID: 9775 RVA: 0x0001BE3B File Offset: 0x0001A03B
			public override void OnUninstall()
			{
				base.OnInstall();
				ShrineChanceBehavior.onShrineChancePurchaseGlobal -= this.OnShrineChancePurchase;
				Run.onRunStartGlobal -= this.OnRunStartGlobal;
			}

			// Token: 0x06002630 RID: 9776 RVA: 0x0001BE65 File Offset: 0x0001A065
			private void OnRunStartGlobal(Run run)
			{
				this.failedInARow = 0;
			}

			// Token: 0x06002631 RID: 9777 RVA: 0x000B1D64 File Offset: 0x000AFF64
			private void OnShrineChancePurchase(bool failed, Interactor interactor)
			{
				CharacterBody currentBody = this.serverAchievementTracker.networkUser.GetCurrentBody();
				if (currentBody && currentBody.GetComponent<Interactor>() == interactor)
				{
					if (failed)
					{
						this.failedInARow++;
						if (this.failedInARow >= 3)
						{
							base.Grant();
							return;
						}
					}
					else
					{
						this.failedInARow = 0;
					}
				}
			}

			// Token: 0x040028A2 RID: 10402
			private int failedInARow;

			// Token: 0x040028A3 RID: 10403
			private const int requirement = 3;
		}
	}
}
