using System;

namespace RoR2.Achievements
{
	// Token: 0x0200068C RID: 1676
	[RegisterAchievement("CompleteTeleporter", "Items.BossDamageBonus", null, null)]
	public class CompleteTeleporterAchievement : BaseAchievement
	{
		// Token: 0x06002564 RID: 9572 RVA: 0x0001B3CE File Offset: 0x000195CE
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterFinishGlobal += this.OnTeleporterFinish;
		}

		// Token: 0x06002565 RID: 9573 RVA: 0x0001B3E7 File Offset: 0x000195E7
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterFinishGlobal -= this.OnTeleporterFinish;
			base.OnUninstall();
		}

		// Token: 0x06002566 RID: 9574 RVA: 0x0001B400 File Offset: 0x00019600
		private void OnTeleporterFinish(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x06002567 RID: 9575 RVA: 0x000B03D4 File Offset: 0x000AE5D4
		private void Check()
		{
			if (this.localUser.cachedBody && this.localUser.cachedBody.healthComponent && this.localUser.cachedBody.healthComponent.alive)
			{
				base.Grant();
			}
		}
	}
}
