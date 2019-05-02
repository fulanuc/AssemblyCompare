using System;

namespace RoR2.Achievements
{
	// Token: 0x02000688 RID: 1672
	[RegisterAchievement("CompleteMultiBossShrine", "Items.Lightning", null, typeof(CompleteMultiBossShrineAchievement.CompleteMultiBossShrineServerAchievement))]
	public class CompleteMultiBossShrineAchievement : BaseAchievement
	{
		// Token: 0x06002555 RID: 9557 RVA: 0x0001B314 File Offset: 0x00019514
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002556 RID: 9558 RVA: 0x0001B323 File Offset: 0x00019523
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x02000689 RID: 1673
		private class CompleteMultiBossShrineServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002558 RID: 9560 RVA: 0x0001B32B File Offset: 0x0001952B
			public override void OnInstall()
			{
				base.OnInstall();
				BossGroup.onBossGroupDefeatedServer += this.OnBossGroupDefeatedServer;
			}

			// Token: 0x06002559 RID: 9561 RVA: 0x0001B344 File Offset: 0x00019544
			public override void OnUninstall()
			{
				BossGroup.onBossGroupDefeatedServer -= this.OnBossGroupDefeatedServer;
				base.OnUninstall();
			}

			// Token: 0x0600255A RID: 9562 RVA: 0x000B037C File Offset: 0x000AE57C
			private void OnBossGroupDefeatedServer(BossGroup bossGroup)
			{
				CharacterBody currentBody = base.GetCurrentBody();
				if (currentBody && currentBody.healthComponent && currentBody.healthComponent.alive && TeleporterInteraction.instance && this.CheckTeleporter(bossGroup, TeleporterInteraction.instance))
				{
					base.Grant();
				}
			}

			// Token: 0x0600255B RID: 9563 RVA: 0x0001B35D File Offset: 0x0001955D
			private bool CheckTeleporter(BossGroup bossGroup, TeleporterInteraction teleporterInteraction)
			{
				return teleporterInteraction.bossDirector.bossGroup == bossGroup && teleporterInteraction.shrineBonusStacks >= 2;
			}

			// Token: 0x0400283F RID: 10303
			private const int requirement = 2;
		}
	}
}
