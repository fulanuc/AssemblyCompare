using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x0200068F RID: 1679
	[RegisterAchievement("CompleteThreeStagesWithoutHealing", "Items.IncreaseHealing", null, null)]
	public class CompleteThreeStagesWithoutHealingsAchievement : BaseAchievement
	{
		// Token: 0x06002575 RID: 9589 RVA: 0x0001B4FF File Offset: 0x000196FF
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterFinishGlobal += this.OnTeleporterFinish;
		}

		// Token: 0x06002576 RID: 9590 RVA: 0x0001B518 File Offset: 0x00019718
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterFinishGlobal -= this.OnTeleporterFinish;
			base.OnUninstall();
		}

		// Token: 0x06002577 RID: 9591 RVA: 0x0001B531 File Offset: 0x00019731
		private void OnTeleporterFinish(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x06002578 RID: 9592 RVA: 0x000B04F0 File Offset: 0x000AE6F0
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

		// Token: 0x04002842 RID: 10306
		private const int requirement = 3;
	}
}
