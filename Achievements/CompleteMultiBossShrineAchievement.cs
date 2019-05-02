using System;

namespace RoR2.Achievements
{
	// Token: 0x0200069A RID: 1690
	[RegisterAchievement("CompleteMultiBossShrine", "Items.Lightning", null, typeof(CompleteMultiBossShrineAchievement.CompleteMultiBossShrineServerAchievement))]
	public class CompleteMultiBossShrineAchievement : BaseAchievement
	{
		// Token: 0x060025EC RID: 9708 RVA: 0x0001BA4F File Offset: 0x00019C4F
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060025ED RID: 9709 RVA: 0x0001BA5E File Offset: 0x00019C5E
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x0200069B RID: 1691
		private class CompleteMultiBossShrineServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025EF RID: 9711 RVA: 0x0001BA66 File Offset: 0x00019C66
			public override void OnInstall()
			{
				base.OnInstall();
				BossGroup.onBossGroupDefeatedServer += this.OnBossGroupDefeatedServer;
			}

			// Token: 0x060025F0 RID: 9712 RVA: 0x0001BA7F File Offset: 0x00019C7F
			public override void OnUninstall()
			{
				BossGroup.onBossGroupDefeatedServer -= this.OnBossGroupDefeatedServer;
				base.OnUninstall();
			}

			// Token: 0x060025F1 RID: 9713 RVA: 0x000B1A6C File Offset: 0x000AFC6C
			private void OnBossGroupDefeatedServer(BossGroup bossGroup)
			{
				CharacterBody currentBody = base.GetCurrentBody();
				if (currentBody && currentBody.healthComponent && currentBody.healthComponent.alive && TeleporterInteraction.instance && this.CheckTeleporter(bossGroup, TeleporterInteraction.instance))
				{
					base.Grant();
				}
			}

			// Token: 0x060025F2 RID: 9714 RVA: 0x0001BA98 File Offset: 0x00019C98
			private bool CheckTeleporter(BossGroup bossGroup, TeleporterInteraction teleporterInteraction)
			{
				return teleporterInteraction.bossDirector.bossGroup == bossGroup && teleporterInteraction.shrineBonusStacks >= 2;
			}

			// Token: 0x0400289B RID: 10395
			private const int requirement = 2;
		}
	}
}
