using System;

namespace RoR2.Achievements
{
	// Token: 0x020006A0 RID: 1696
	[RegisterAchievement("KillBossQuick", "Items.TreasureCache", null, typeof(KillBossQuickAchievement.KillBossQuickServerAchievement))]
	public class KillBossQuickAchievement : BaseAchievement
	{
		// Token: 0x060025C5 RID: 9669 RVA: 0x0001B314 File Offset: 0x00019514
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060025C6 RID: 9670 RVA: 0x0001B323 File Offset: 0x00019523
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006A1 RID: 1697
		private class KillBossQuickServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025C8 RID: 9672 RVA: 0x0001B9B5 File Offset: 0x00019BB5
			public override void OnInstall()
			{
				base.OnInstall();
				BossGroup.onBossGroupDefeatedServer += this.OnBossGroupDefeatedServer;
			}

			// Token: 0x060025C9 RID: 9673 RVA: 0x0001B9CE File Offset: 0x00019BCE
			public override void OnUninstall()
			{
				BossGroup.onBossGroupDefeatedServer -= this.OnBossGroupDefeatedServer;
				base.OnUninstall();
			}

			// Token: 0x060025CA RID: 9674 RVA: 0x0001B9E7 File Offset: 0x00019BE7
			private void OnBossGroupDefeatedServer(BossGroup bossGroup)
			{
				if (bossGroup.fixedAge <= 15f)
				{
					base.Grant();
				}
			}

			// Token: 0x0400284B RID: 10315
			private const float requirement = 15f;
		}
	}
}
