using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x02000687 RID: 1671
	[RegisterAchievement("Complete30StagesCareer", "Characters.Engineer", null, null)]
	public class Complete30StagesCareerAchievement : BaseAchievement
	{
		// Token: 0x06002550 RID: 9552 RVA: 0x0001B28F File Offset: 0x0001948F
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x06002551 RID: 9553 RVA: 0x0001B2B4 File Offset: 0x000194B4
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002552 RID: 9554 RVA: 0x0001B2D3 File Offset: 0x000194D3
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.totalStagesCompleted) / 30f;
		}

		// Token: 0x06002553 RID: 9555 RVA: 0x0001B2F2 File Offset: 0x000194F2
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.totalStagesCompleted) >= 30UL)
			{
				base.Grant();
			}
		}

		// Token: 0x0400283E RID: 10302
		private const int requirement = 30;
	}
}
