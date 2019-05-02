using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006A1 RID: 1697
	[RegisterAchievement("CompleteThreeStagesWithoutHealing", "Items.IncreaseHealing", null, null)]
	public class CompleteThreeStagesWithoutHealingsAchievement : BaseAchievement
	{
		// Token: 0x0600260C RID: 9740 RVA: 0x0001BC3A File Offset: 0x00019E3A
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterFinishGlobal += this.OnTeleporterFinish;
		}

		// Token: 0x0600260D RID: 9741 RVA: 0x0001BC53 File Offset: 0x00019E53
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterFinishGlobal -= this.OnTeleporterFinish;
			base.OnUninstall();
		}

		// Token: 0x0600260E RID: 9742 RVA: 0x0001BC6C File Offset: 0x00019E6C
		private void OnTeleporterFinish(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x0600260F RID: 9743 RVA: 0x000B1BE8 File Offset: 0x000AFDE8
		private void Check()
		{
			if (Run.instance && Run.instance.GetType() == typeof(Run) && this.localUser != null && this.localUser.currentNetworkUser != null)
			{
				SceneDef sceneDefForCurrentScene = SceneCatalog.GetSceneDefForCurrentScene();
				StatSheet currentStats = this.localUser.currentNetworkUser.masterPlayerStatsComponent.currentStats;
				if (sceneDefForCurrentScene.stageOrder >= 3 && currentStats.GetStatValueULong(StatDef.totalHealthHealed) <= 0f && this.localUser.cachedBody && this.localUser.cachedBody.healthComponent && this.localUser.cachedBody.healthComponent.alive)
				{
					base.Grant();
				}
			}
		}

		// Token: 0x0400289E RID: 10398
		private const int requirement = 3;
	}
}
