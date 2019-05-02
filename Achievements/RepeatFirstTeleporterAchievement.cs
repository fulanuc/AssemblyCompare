using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006C2 RID: 1730
	[RegisterAchievement("RepeatFirstTeleporter", "Characters.Toolbot", null, typeof(RepeatFirstTeleporterAchievement.RepeatFirstTeleporterServerAchievement))]
	public class RepeatFirstTeleporterAchievement : BaseAchievement
	{
		// Token: 0x06002698 RID: 9880 RVA: 0x0001C3A2 File Offset: 0x0001A5A2
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			base.SetServerTracked(true);
		}

		// Token: 0x06002699 RID: 9881 RVA: 0x0001C3C8 File Offset: 0x0001A5C8
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600269A RID: 9882 RVA: 0x0001C3E7 File Offset: 0x0001A5E7
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.firstTeleporterCompleted) >= 5UL)
			{
				base.Grant();
			}
		}

		// Token: 0x0600269B RID: 9883 RVA: 0x0001C408 File Offset: 0x0001A608
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.firstTeleporterCompleted) / 5f;
		}

		// Token: 0x040028B0 RID: 10416
		private const int requirement = 5;

		// Token: 0x020006C3 RID: 1731
		private class RepeatFirstTeleporterServerAchievement : BaseServerAchievement
		{
			// Token: 0x0600269D RID: 9885 RVA: 0x0001C428 File Offset: 0x0001A628
			public override void OnInstall()
			{
				base.OnInstall();
				TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
			}

			// Token: 0x0600269E RID: 9886 RVA: 0x0001C441 File Offset: 0x0001A641
			public override void OnUninstall()
			{
				TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
				base.OnUninstall();
			}

			// Token: 0x0600269F RID: 9887 RVA: 0x000B2238 File Offset: 0x000B0438
			private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
			{
				SceneCatalog.GetSceneDefForCurrentScene();
				StatSheet currentStats = base.networkUser.masterPlayerStatsComponent.currentStats;
				if (Run.instance && Run.instance.stageClearCount == 0)
				{
					PlayerStatsComponent masterPlayerStatsComponent = base.networkUser.masterPlayerStatsComponent;
					if (masterPlayerStatsComponent)
					{
						masterPlayerStatsComponent.currentStats.PushStatValue(StatDef.firstTeleporterCompleted, 1UL);
					}
				}
			}
		}
	}
}
