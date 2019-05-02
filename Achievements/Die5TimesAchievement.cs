using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x02000692 RID: 1682
	[RegisterAchievement("Die5Times", "Items.Bear", null, null)]
	public class Die5TimesAchievement : BaseAchievement
	{
		// Token: 0x06002581 RID: 9601 RVA: 0x0001B577 File Offset: 0x00019777
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x06002582 RID: 9602 RVA: 0x0001B59C File Offset: 0x0001979C
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002583 RID: 9603 RVA: 0x0001B5BB File Offset: 0x000197BB
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.totalDeaths) / 5f;
		}

		// Token: 0x06002584 RID: 9604 RVA: 0x0001B5DA File Offset: 0x000197DA
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.totalDeaths) >= 5UL)
			{
				base.Grant();
			}
		}

		// Token: 0x04002843 RID: 10307
		private const int requirement = 5;
	}
}
