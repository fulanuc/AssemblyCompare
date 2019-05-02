using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006A4 RID: 1700
	[RegisterAchievement("Die5Times", "Items.Bear", null, null)]
	public class Die5TimesAchievement : BaseAchievement
	{
		// Token: 0x06002618 RID: 9752 RVA: 0x0001BCB2 File Offset: 0x00019EB2
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x06002619 RID: 9753 RVA: 0x0001BCD7 File Offset: 0x00019ED7
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600261A RID: 9754 RVA: 0x0001BCF6 File Offset: 0x00019EF6
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.totalDeaths) / 5f;
		}

		// Token: 0x0600261B RID: 9755 RVA: 0x0001BD15 File Offset: 0x00019F15
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.totalDeaths) >= 5UL)
			{
				base.Grant();
			}
		}

		// Token: 0x0400289F RID: 10399
		private const int requirement = 5;
	}
}
