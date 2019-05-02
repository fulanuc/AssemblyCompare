using System;

namespace RoR2.Achievements
{
	// Token: 0x02000695 RID: 1685
	[RegisterAchievement("FailShrineChance", "Items.Hoof", null, typeof(FailShrineChanceAchievement.FailShrineChanceServerAchievement))]
	public class FailShrineChanceAchievement : BaseAchievement
	{
		// Token: 0x06002594 RID: 9620 RVA: 0x0001B314 File Offset: 0x00019514
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002595 RID: 9621 RVA: 0x0001B323 File Offset: 0x00019523
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x02000696 RID: 1686
		private class FailShrineChanceServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002597 RID: 9623 RVA: 0x0001B6D6 File Offset: 0x000198D6
			public override void OnInstall()
			{
				base.OnInstall();
				ShrineChanceBehavior.onShrineChancePurchaseGlobal += this.OnShrineChancePurchase;
				Run.onRunStartGlobal += this.OnRunStartGlobal;
			}

			// Token: 0x06002598 RID: 9624 RVA: 0x0001B700 File Offset: 0x00019900
			public override void OnUninstall()
			{
				base.OnInstall();
				ShrineChanceBehavior.onShrineChancePurchaseGlobal -= this.OnShrineChancePurchase;
				Run.onRunStartGlobal -= this.OnRunStartGlobal;
			}

			// Token: 0x06002599 RID: 9625 RVA: 0x0001B72A File Offset: 0x0001992A
			private void OnRunStartGlobal(Run run)
			{
				this.failedInARow = 0;
			}

			// Token: 0x0600259A RID: 9626 RVA: 0x000B066C File Offset: 0x000AE86C
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

			// Token: 0x04002846 RID: 10310
			private int failedInARow;

			// Token: 0x04002847 RID: 10311
			private const int requirement = 3;
		}
	}
}
