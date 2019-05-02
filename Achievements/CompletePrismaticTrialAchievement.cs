using System;

namespace RoR2.Achievements
{
	// Token: 0x0200068A RID: 1674
	[RegisterAchievement("CompletePrismaticTrial", "Items.HealOnCrit", null, typeof(CompletePrismaticTrialAchievement.CompletePrismaticTrialServerAchievement))]
	public class CompletePrismaticTrialAchievement : BaseAchievement
	{
		// Token: 0x0600255D RID: 9565 RVA: 0x0001B314 File Offset: 0x00019514
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x0600255E RID: 9566 RVA: 0x0001B323 File Offset: 0x00019523
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x0200068B RID: 1675
		private class CompletePrismaticTrialServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002560 RID: 9568 RVA: 0x0001B388 File Offset: 0x00019588
			public override void OnInstall()
			{
				base.OnInstall();
				Run.OnServerGameOver += this.OnServerGameOver;
			}

			// Token: 0x06002561 RID: 9569 RVA: 0x0001B3A1 File Offset: 0x000195A1
			public override void OnUninstall()
			{
				base.OnInstall();
				Run.OnServerGameOver -= this.OnServerGameOver;
			}

			// Token: 0x06002562 RID: 9570 RVA: 0x0001B3BA File Offset: 0x000195BA
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
