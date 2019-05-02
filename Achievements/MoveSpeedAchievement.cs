using System;

namespace RoR2.Achievements
{
	// Token: 0x020006BF RID: 1727
	[RegisterAchievement("MoveSpeed", "Items.JumpBoost", null, null)]
	public class MoveSpeedAchievement : BaseAchievement
	{
		// Token: 0x0600268C RID: 9868 RVA: 0x0001C304 File Offset: 0x0001A504
		public override void OnInstall()
		{
			base.OnInstall();
			RoR2Application.onUpdate += this.CheckMoveSpeed;
		}

		// Token: 0x0600268D RID: 9869 RVA: 0x0001C31D File Offset: 0x0001A51D
		public override void OnUninstall()
		{
			RoR2Application.onUpdate -= this.CheckMoveSpeed;
			base.OnUninstall();
		}

		// Token: 0x0600268E RID: 9870 RVA: 0x000B2140 File Offset: 0x000B0340
		public void CheckMoveSpeed()
		{
			if (this.localUser != null && this.localUser.cachedBody && this.localUser.cachedBody.moveSpeed / this.localUser.cachedBody.baseMoveSpeed >= 4f)
			{
				base.Grant();
			}
		}

		// Token: 0x040028AC RID: 10412
		private const float requirement = 4f;
	}
}
