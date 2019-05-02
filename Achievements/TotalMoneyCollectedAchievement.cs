using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006B6 RID: 1718
	[RegisterAchievement("TotalMoneyCollected", "Items.GoldGat", null, null)]
	public class TotalMoneyCollectedAchievement : BaseAchievement
	{
		// Token: 0x0600261E RID: 9758 RVA: 0x0001BE97 File Offset: 0x0001A097
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x0600261F RID: 9759 RVA: 0x0001BEBC File Offset: 0x0001A0BC
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002620 RID: 9760 RVA: 0x0001BEDB File Offset: 0x0001A0DB
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.goldCollected) / 30480f;
		}

		// Token: 0x06002621 RID: 9761 RVA: 0x0001BEFA File Offset: 0x0001A0FA
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.goldCollected) >= 30480UL)
			{
				base.Grant();
			}
		}

		// Token: 0x04002858 RID: 10328
		private const int requirement = 30480;
	}
}
