using System;

namespace RoR2.Achievements
{
	// Token: 0x02000690 RID: 1680
	[RegisterAchievement("CompleteUnknownEnding", "Characters.Mercenary", null, typeof(CompleteUnknownEndingAchievement.CompleteUnknownEndingServerAchievement))]
	public class CompleteUnknownEndingAchievement : BaseAchievement
	{
		// Token: 0x0600257A RID: 9594 RVA: 0x0001B314 File Offset: 0x00019514
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x0600257B RID: 9595 RVA: 0x0001B323 File Offset: 0x00019523
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x02000691 RID: 1681
		private class CompleteUnknownEndingServerAchievement : BaseServerAchievement
		{
			// Token: 0x0600257D RID: 9597 RVA: 0x0001B539 File Offset: 0x00019739
			public override void OnInstall()
			{
				base.OnInstall();
				Run.OnServerGameOver += this.OnServerGameOver;
			}

			// Token: 0x0600257E RID: 9598 RVA: 0x0001B552 File Offset: 0x00019752
			public override void OnUninstall()
			{
				base.OnInstall();
				Run.OnServerGameOver -= this.OnServerGameOver;
			}

			// Token: 0x0600257F RID: 9599 RVA: 0x0001B56B File Offset: 0x0001976B
			private void OnServerGameOver(Run run, GameResultType result)
			{
				if (result == GameResultType.Unknown)
				{
					base.Grant();
				}
			}
		}
	}
}
