using System;

namespace RoR2.Achievements
{
	// Token: 0x02000685 RID: 1669
	[RegisterAchievement("ChargeTeleporterWhileNearDeath", "Items.WarCryOnMultiKill", null, null)]
	public class ChargeTeleporterWhileNearDeathAchievement : BaseAchievement
	{
		// Token: 0x06002545 RID: 9541 RVA: 0x0001B213 File Offset: 0x00019413
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
		}

		// Token: 0x06002546 RID: 9542 RVA: 0x0001B22C File Offset: 0x0001942C
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
			base.OnUninstall();
		}

		// Token: 0x06002547 RID: 9543 RVA: 0x0001B245 File Offset: 0x00019445
		private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x06002548 RID: 9544 RVA: 0x000B0254 File Offset: 0x000AE454
		private void Check()
		{
			if (this.localUser.cachedBody && this.localUser.cachedBody.healthComponent && this.localUser.cachedBody.healthComponent.alive && this.localUser.cachedBody.healthComponent.combinedHealthFraction <= 0.1f)
			{
				base.Grant();
			}
		}

		// Token: 0x04002839 RID: 10297
		private const float requirement = 0.1f;
	}
}
