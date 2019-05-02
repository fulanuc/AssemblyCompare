using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006A0 RID: 1696
	[RegisterAchievement("CompleteThreeStages", "Characters.Huntress", null, null)]
	public class CompleteThreeStagesAchievement : BaseAchievement
	{
		// Token: 0x06002607 RID: 9735 RVA: 0x0001BC00 File Offset: 0x00019E00
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
		}

		// Token: 0x06002608 RID: 9736 RVA: 0x0001BC19 File Offset: 0x00019E19
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
			base.OnUninstall();
		}

		// Token: 0x06002609 RID: 9737 RVA: 0x0001BC32 File Offset: 0x00019E32
		private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x0600260A RID: 9738 RVA: 0x000B1B74 File Offset: 0x000AFD74
		private void Check()
		{
			if (Run.instance && Run.instance.GetType() == typeof(Run))
			{
				SceneDef sceneDefForCurrentScene = SceneCatalog.GetSceneDefForCurrentScene();
				if (sceneDefForCurrentScene == null)
				{
					return;
				}
				if (this.localUser.currentNetworkUser.masterPlayerStatsComponent.currentStats.GetStatValueULong(StatDef.totalDeaths) == 0UL && sceneDefForCurrentScene.stageOrder == 3)
				{
					base.Grant();
				}
			}
		}

		// Token: 0x0400289D RID: 10397
		private const int requirement = 3;
	}
}
