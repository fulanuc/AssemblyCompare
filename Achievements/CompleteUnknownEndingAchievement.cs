using System;

namespace RoR2.Achievements
{
	// Token: 0x020006A2 RID: 1698
	[RegisterAchievement("CompleteUnknownEnding", "Characters.Mercenary", null, typeof(CompleteUnknownEndingAchievement.CompleteUnknownEndingServerAchievement))]
	public class CompleteUnknownEndingAchievement : BaseAchievement
	{
		// Token: 0x06002611 RID: 9745 RVA: 0x0001BA4F File Offset: 0x00019C4F
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002612 RID: 9746 RVA: 0x0001BA5E File Offset: 0x00019C5E
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006A3 RID: 1699
		private class CompleteUnknownEndingServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002614 RID: 9748 RVA: 0x0001BC74 File Offset: 0x00019E74
			public override void OnInstall()
			{
				base.OnInstall();
				Run.OnServerGameOver += this.OnServerGameOver;
			}

			// Token: 0x06002615 RID: 9749 RVA: 0x0001BC8D File Offset: 0x00019E8D
			public override void OnUninstall()
			{
				base.OnInstall();
				Run.OnServerGameOver -= this.OnServerGameOver;
			}

			// Token: 0x06002616 RID: 9750 RVA: 0x0001BCA6 File Offset: 0x00019EA6
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
