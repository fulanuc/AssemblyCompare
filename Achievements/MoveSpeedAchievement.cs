using System;

namespace RoR2.Achievements
{
	// Token: 0x020006AD RID: 1709
	[RegisterAchievement("MoveSpeed", "Items.JumpBoost", null, null)]
	public class MoveSpeedAchievement : BaseAchievement
	{
		// Token: 0x060025F5 RID: 9717 RVA: 0x0001BBC9 File Offset: 0x00019DC9
		public override void OnInstall()
		{
			base.OnInstall();
			RoR2Application.onUpdate += this.CheckMoveSpeed;
		}

		// Token: 0x060025F6 RID: 9718 RVA: 0x0001BBE2 File Offset: 0x00019DE2
		public override void OnUninstall()
		{
			base.OnUninstall();
			RoR2Application.onUpdate -= this.CheckMoveSpeed;
		}

		// Token: 0x060025F7 RID: 9719 RVA: 0x000B0A48 File Offset: 0x000AEC48
		public void CheckMoveSpeed()
		{
			if (this.localUser.cachedBody && this.localUser.cachedBody.moveSpeed / this.localUser.cachedBody.baseMoveSpeed >= 4f)
			{
				base.Grant();
			}
		}

		// Token: 0x04002850 RID: 10320
		private const float requirement = 4f;
	}
}
