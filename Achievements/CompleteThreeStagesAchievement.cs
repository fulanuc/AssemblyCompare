using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x0200068E RID: 1678
	[RegisterAchievement("CompleteThreeStages", "Characters.Huntress", null, null)]
	public class CompleteThreeStagesAchievement : BaseAchievement
	{
		// Token: 0x06002570 RID: 9584 RVA: 0x0001B4C5 File Offset: 0x000196C5
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterFinishGlobal += this.OnTeleporterFinish;
		}

		// Token: 0x06002571 RID: 9585 RVA: 0x0001B4DE File Offset: 0x000196DE
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterFinishGlobal -= this.OnTeleporterFinish;
			base.OnUninstall();
		}

		// Token: 0x06002572 RID: 9586 RVA: 0x0001B4F7 File Offset: 0x000196F7
		private void OnTeleporterFinish(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x06002573 RID: 9587 RVA: 0x000B0484 File Offset: 0x000AE684
		private void Check()
		{
			if (Run.instance && Run.instance.GetType() == typeof(Run))
			{
				SceneDef sceneDefForCurrentScene = SceneCatalog.GetSceneDefForCurrentScene();
				if (this.localUser.currentNetworkUser.masterPlayerStatsComponent.currentStats.GetStatValueULong(StatDef.totalDeaths) == 0UL && sceneDefForCurrentScene.stageOrder == 3)
				{
					base.Grant();
				}
			}
		}

		// Token: 0x04002841 RID: 10305
		private const int requirement = 3;
	}
}
