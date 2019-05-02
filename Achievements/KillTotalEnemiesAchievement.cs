using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006A6 RID: 1702
	[RegisterAchievement("KillTotalEnemies", "Items.Infusion", null, null)]
	public class KillTotalEnemiesAchievement : BaseAchievement
	{
		// Token: 0x060025D6 RID: 9686 RVA: 0x0001BA2E File Offset: 0x00019C2E
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x060025D7 RID: 9687 RVA: 0x0001BA53 File Offset: 0x00019C53
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x060025D8 RID: 9688 RVA: 0x0001BA72 File Offset: 0x00019C72
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.totalKills) / 3000f;
		}

		// Token: 0x060025D9 RID: 9689 RVA: 0x0001BA91 File Offset: 0x00019C91
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.totalKills) >= 3000UL)
			{
				base.Grant();
			}
		}

		// Token: 0x0400284C RID: 10316
		private const int requirement = 3000;
	}
}
