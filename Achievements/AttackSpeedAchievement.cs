using System;

namespace RoR2.Achievements
{
	// Token: 0x02000681 RID: 1665
	[RegisterAchievement("AttackSpeed", "Items.AttackSpeedOnCrit", null, null)]
	public class AttackSpeedAchievement : BaseAchievement
	{
		// Token: 0x0600252D RID: 9517 RVA: 0x0001B073 File Offset: 0x00019273
		public override void OnInstall()
		{
			base.OnInstall();
			RoR2Application.onUpdate += this.CheckAttackSpeed;
		}

		// Token: 0x0600252E RID: 9518 RVA: 0x0001B08C File Offset: 0x0001928C
		public override void OnUninstall()
		{
			base.OnUninstall();
			RoR2Application.onUpdate -= this.CheckAttackSpeed;
		}

		// Token: 0x0600252F RID: 9519 RVA: 0x0001B0A5 File Offset: 0x000192A5
		public void CheckAttackSpeed()
		{
			if (this.localUser.cachedBody && this.localUser.cachedBody.attackSpeed >= 3f)
			{
				base.Grant();
			}
		}

		// Token: 0x0400282E RID: 10286
		private const float requirement = 3f;
	}
}
