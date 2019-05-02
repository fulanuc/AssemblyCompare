using System;

namespace RoR2.Achievements
{
	// Token: 0x020006A8 RID: 1704
	[RegisterAchievement("LoopOnce", "Items.BounceNearby", null, null)]
	public class LoopOnceAchievement : BaseAchievement
	{
		// Token: 0x060025E3 RID: 9699 RVA: 0x0001BB21 File Offset: 0x00019D21
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x060025E4 RID: 9700 RVA: 0x0001BB46 File Offset: 0x00019D46
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x060025E5 RID: 9701 RVA: 0x000B0948 File Offset: 0x000AEB48
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
