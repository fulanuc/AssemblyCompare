using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006B0 RID: 1712
	[RegisterAchievement("RepeatFirstTeleporter", "Characters.Toolbot", null, typeof(RepeatFirstTeleporterAchievement.RepeatFirstTeleporterServerAchievement))]
	public class RepeatFirstTeleporterAchievement : BaseAchievement
	{
		// Token: 0x06002601 RID: 9729 RVA: 0x0001BC67 File Offset: 0x00019E67
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			base.SetServerTracked(true);
		}

		// Token: 0x06002602 RID: 9730 RVA: 0x0001BC8D File Offset: 0x00019E8D
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002603 RID: 9731 RVA: 0x0001BCAC File Offset: 0x00019EAC
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.firstTeleporterCompleted) >= 5UL)
			{
				base.Grant();
			}
		}

		// Token: 0x06002604 RID: 9732 RVA: 0x0001BCCD File Offset: 0x00019ECD
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.firstTeleporterCompleted) / 5f;
		}

		// Token: 0x04002854 RID: 10324
		private const int requirement = 5;

		// Token: 0x020006B1 RID: 1713
		private class RepeatFirstTeleporterServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002606 RID: 9734 RVA: 0x0001BCED File Offset: 0x00019EED
			public override void OnInstall()
			{
				base.OnInstall();
				TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
			}

			// Token: 0x06002607 RID: 9735 RVA: 0x0001BD06 File Offset: 0x00019F06
			public override void OnUninstall()
			{
				TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
				base.OnUninstall();
			}

			// Token: 0x06002608 RID: 9736 RVA: 0x000B0B38 File Offset: 0x000AED38
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
