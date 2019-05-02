using System;

namespace RoR2.Achievements
{
	// Token: 0x0200069E RID: 1694
	[RegisterAchievement("CompleteTeleporter", "Items.BossDamageBonus", null, null)]
	public class CompleteTeleporterAchievement : BaseAchievement
	{
		// Token: 0x060025FB RID: 9723 RVA: 0x0001BB09 File Offset: 0x00019D09
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterFinishGlobal += this.OnTeleporterFinish;
		}

		// Token: 0x060025FC RID: 9724 RVA: 0x0001BB22 File Offset: 0x00019D22
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterFinishGlobal -= this.OnTeleporterFinish;
			base.OnUninstall();
		}

		// Token: 0x060025FD RID: 9725 RVA: 0x0001BB3B File Offset: 0x00019D3B
		private void OnTeleporterFinish(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x060025FE RID: 9726 RVA: 0x000B1AC4 File Offset: 0x000AFCC4
		private void Check()
		{
			if (this.localUser.cachedBody && this.localUser.cachedBody.healthComponent && this.localUser.cachedBody.healthComponent.alive)
			{
				base.Grant();
			}
		}
	}
}
