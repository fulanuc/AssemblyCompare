using System;

namespace RoR2.Achievements
{
	// Token: 0x02000697 RID: 1687
	[RegisterAchievement("ChargeTeleporterWhileNearDeath", "Items.WarCryOnMultiKill", null, null)]
	public class ChargeTeleporterWhileNearDeathAchievement : BaseAchievement
	{
		// Token: 0x060025DC RID: 9692 RVA: 0x0001B94E File Offset: 0x00019B4E
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
		}

		// Token: 0x060025DD RID: 9693 RVA: 0x0001B967 File Offset: 0x00019B67
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
			base.OnUninstall();
		}

		// Token: 0x060025DE RID: 9694 RVA: 0x0001B980 File Offset: 0x00019B80
		private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x060025DF RID: 9695 RVA: 0x000B1944 File Offset: 0x000AFB44
		private void Check()
		{
			if (this.localUser.cachedBody && this.localUser.cachedBody.healthComponent && this.localUser.cachedBody.healthComponent.alive && this.localUser.cachedBody.healthComponent.combinedHealthFraction <= 0.1f)
			{
				base.Grant();
			}
		}

		// Token: 0x04002895 RID: 10389
		private const float requirement = 0.1f;
	}
}
