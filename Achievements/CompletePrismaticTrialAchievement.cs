using System;

namespace RoR2.Achievements
{
	// Token: 0x0200069C RID: 1692
	[RegisterAchievement("CompletePrismaticTrial", "Items.HealOnCrit", null, typeof(CompletePrismaticTrialAchievement.CompletePrismaticTrialServerAchievement))]
	public class CompletePrismaticTrialAchievement : BaseAchievement
	{
		// Token: 0x060025F4 RID: 9716 RVA: 0x0001BA4F File Offset: 0x00019C4F
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060025F5 RID: 9717 RVA: 0x0001BA5E File Offset: 0x00019C5E
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x0200069D RID: 1693
		private class CompletePrismaticTrialServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025F7 RID: 9719 RVA: 0x0001BAC3 File Offset: 0x00019CC3
			public override void OnInstall()
			{
				base.OnInstall();
				Run.OnServerGameOver += this.OnServerGameOver;
			}

			// Token: 0x060025F8 RID: 9720 RVA: 0x0001BADC File Offset: 0x00019CDC
			public override void OnUninstall()
			{
				base.OnInstall();
				Run.OnServerGameOver -= this.OnServerGameOver;
			}

			// Token: 0x060025F9 RID: 9721 RVA: 0x0001BAF5 File Offset: 0x00019CF5
			private void OnServerGameOver(Run run, GameResultType result)
			{
				if (run is WeeklyRun && result == GameResultType.Won)
				{
					base.Grant();
				}
			}
		}
	}
}
