using System;

namespace RoR2.Achievements
{
	// Token: 0x020006B2 RID: 1714
	[RegisterAchievement("KillBossQuick", "Items.TreasureCache", null, typeof(KillBossQuickAchievement.KillBossQuickServerAchievement))]
	public class KillBossQuickAchievement : BaseAchievement
	{
		// Token: 0x0600265C RID: 9820 RVA: 0x0001BA4F File Offset: 0x00019C4F
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x0600265D RID: 9821 RVA: 0x0001BA5E File Offset: 0x00019C5E
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006B3 RID: 1715
		private class KillBossQuickServerAchievement : BaseServerAchievement
		{
			// Token: 0x0600265F RID: 9823 RVA: 0x0001C0F0 File Offset: 0x0001A2F0
			public override void OnInstall()
			{
				base.OnInstall();
				BossGroup.onBossGroupDefeatedServer += this.OnBossGroupDefeatedServer;
			}

			// Token: 0x06002660 RID: 9824 RVA: 0x0001C109 File Offset: 0x0001A309
			public override void OnUninstall()
			{
				BossGroup.onBossGroupDefeatedServer -= this.OnBossGroupDefeatedServer;
				base.OnUninstall();
			}

			// Token: 0x06002661 RID: 9825 RVA: 0x0001C122 File Offset: 0x0001A322
			private void OnBossGroupDefeatedServer(BossGroup bossGroup)
			{
				if (bossGroup.fixedAge <= 15f)
				{
					base.Grant();
				}
			}

			// Token: 0x040028A7 RID: 10407
			private const float requirement = 15f;
		}
	}
}
