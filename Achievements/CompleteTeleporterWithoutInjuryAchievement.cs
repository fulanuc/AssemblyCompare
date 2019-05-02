using System;

namespace RoR2.Achievements
{
	// Token: 0x0200068D RID: 1677
	[RegisterAchievement("CompleteTeleporterWithoutInjury", "Items.SecondarySkillMagazine", null, null)]
	public class CompleteTeleporterWithoutInjuryAchievement : BaseAchievement
	{
		// Token: 0x06002569 RID: 9577 RVA: 0x0001B408 File Offset: 0x00019608
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterBeginChargingGlobal += this.OnTeleporterBeginCharging;
			TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
			GlobalEventManager.onClientDamageNotified += this.OnClientDamageNotified;
		}

		// Token: 0x0600256A RID: 9578 RVA: 0x0001B443 File Offset: 0x00019643
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterBeginChargingGlobal -= this.OnTeleporterBeginCharging;
			TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
			GlobalEventManager.onClientDamageNotified -= this.OnClientDamageNotified;
			base.OnUninstall();
		}

		// Token: 0x0600256B RID: 9579 RVA: 0x0001B47E File Offset: 0x0001967E
		private void OnTeleporterBeginCharging(TeleporterInteraction teleporterInteraction)
		{
			this.hasBeenHit = false;
		}

		// Token: 0x0600256C RID: 9580 RVA: 0x0001B487 File Offset: 0x00019687
		private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x0600256D RID: 9581 RVA: 0x0001B48F File Offset: 0x0001968F
		private void OnClientDamageNotified(DamageDealtMessage damageDealtMessage)
		{
			if (!this.hasBeenHit && damageDealtMessage.victim && damageDealtMessage.victim == this.localUser.cachedBodyObject)
			{
				this.hasBeenHit = true;
			}
		}

		// Token: 0x0600256E RID: 9582 RVA: 0x000B0428 File Offset: 0x000AE628
		private void Check()
		{
			if (this.localUser.cachedBody && this.localUser.cachedBody.healthComponent && this.localUser.cachedBody.healthComponent.alive && !this.hasBeenHit)
			{
				base.Grant();
			}
		}

		// Token: 0x04002840 RID: 10304
		private bool hasBeenHit;
	}
}
