using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x02000699 RID: 1689
	[RegisterAchievement("Complete30StagesCareer", "Characters.Engineer", null, null)]
	public class Complete30StagesCareerAchievement : BaseAchievement
	{
		// Token: 0x060025E7 RID: 9703 RVA: 0x0001B9CA File Offset: 0x00019BCA
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x060025E8 RID: 9704 RVA: 0x0001B9EF File Offset: 0x00019BEF
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x060025E9 RID: 9705 RVA: 0x0001BA0E File Offset: 0x00019C0E
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.totalStagesCompleted) / 30f;
		}

		// Token: 0x060025EA RID: 9706 RVA: 0x0001BA2D File Offset: 0x00019C2D
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.totalStagesCompleted) >= 30UL)
			{
				base.Grant();
			}
		}

		// Token: 0x0400289A RID: 10394
		private const int requirement = 30;
	}
}
