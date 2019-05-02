using System;

namespace RoR2.Achievements
{
	// Token: 0x020006BA RID: 1722
	[RegisterAchievement("LoopOnce", "Items.BounceNearby", null, null)]
	public class LoopOnceAchievement : BaseAchievement
	{
		// Token: 0x0600267A RID: 9850 RVA: 0x0001C25C File Offset: 0x0001A45C
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x0600267B RID: 9851 RVA: 0x0001C281 File Offset: 0x0001A481
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600267C RID: 9852 RVA: 0x000B2040 File Offset: 0x000B0240
		private void Check()
		{
			if (Run.instance && Run.instance.GetType() == typeof(Run))
			{
				SceneDef sceneDefForCurrentScene = SceneCatalog.GetSceneDefForCurrentScene();
				if (sceneDefForCurrentScene && sceneDefForCurrentScene.stageOrder < Run.instance.stageClearCount)
				{
					base.Grant();
				}
			}
		}
	}
}
