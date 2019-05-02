using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006B8 RID: 1720
	[RegisterAchievement("KillTotalEnemies", "Items.Infusion", null, null)]
	public class KillTotalEnemiesAchievement : BaseAchievement
	{
		// Token: 0x0600266D RID: 9837 RVA: 0x0001C169 File Offset: 0x0001A369
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x0600266E RID: 9838 RVA: 0x0001C18E File Offset: 0x0001A38E
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600266F RID: 9839 RVA: 0x0001C1AD File Offset: 0x0001A3AD
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.totalKills) / 3000f;
		}

		// Token: 0x06002670 RID: 9840 RVA: 0x0001C1CC File Offset: 0x0001A3CC
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.totalKills) >= 3000UL)
			{
				base.Grant();
			}
		}

		// Token: 0x040028A8 RID: 10408
		private const int requirement = 3000;
	}
}
