using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006C8 RID: 1736
	[RegisterAchievement("TotalMoneyCollected", "Items.GoldGat", null, null)]
	public class TotalMoneyCollectedAchievement : BaseAchievement
	{
		// Token: 0x060026B5 RID: 9909 RVA: 0x0001C5D2 File Offset: 0x0001A7D2
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x060026B6 RID: 9910 RVA: 0x0001C5F7 File Offset: 0x0001A7F7
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x060026B7 RID: 9911 RVA: 0x0001C616 File Offset: 0x0001A816
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.goldCollected) / 30480f;
		}

		// Token: 0x060026B8 RID: 9912 RVA: 0x0001C635 File Offset: 0x0001A835
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.goldCollected) >= 30480UL)
			{
				base.Grant();
			}
		}

		// Token: 0x040028B4 RID: 10420
		private const int requirement = 30480;
	}
}
