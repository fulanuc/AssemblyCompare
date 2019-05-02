using System;

namespace RoR2.Achievements
{
	// Token: 0x02000693 RID: 1683
	[RegisterAchievement("AttackSpeed", "Items.AttackSpeedOnCrit", null, null)]
	public class AttackSpeedAchievement : BaseAchievement
	{
		// Token: 0x060025C4 RID: 9668 RVA: 0x0001B7A6 File Offset: 0x000199A6
		public override void OnInstall()
		{
			base.OnInstall();
			RoR2Application.onUpdate += this.CheckAttackSpeed;
		}

		// Token: 0x060025C5 RID: 9669 RVA: 0x0001B7BF File Offset: 0x000199BF
		public override void OnUninstall()
		{
			RoR2Application.onUpdate -= this.CheckAttackSpeed;
			base.OnUninstall();
		}

		// Token: 0x060025C6 RID: 9670 RVA: 0x0001B7D8 File Offset: 0x000199D8
		public void CheckAttackSpeed()
		{
			if (this.localUser != null && this.localUser.cachedBody && this.localUser.cachedBody.attackSpeed >= 3f)
			{
				base.Grant();
			}
		}

		// Token: 0x0400288A RID: 10378
		private const float requirement = 3f;
	}
}
