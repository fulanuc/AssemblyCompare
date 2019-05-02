using System;

namespace RoR2.Achievements
{
	// Token: 0x0200069F RID: 1695
	[RegisterAchievement("CompleteTeleporterWithoutInjury", "Items.SecondarySkillMagazine", null, null)]
	public class CompleteTeleporterWithoutInjuryAchievement : BaseAchievement
	{
		// Token: 0x06002600 RID: 9728 RVA: 0x0001BB43 File Offset: 0x00019D43
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterBeginChargingGlobal += this.OnTeleporterBeginCharging;
			TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
			GlobalEventManager.onClientDamageNotified += this.OnClientDamageNotified;
		}

		// Token: 0x06002601 RID: 9729 RVA: 0x0001BB7E File Offset: 0x00019D7E
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterBeginChargingGlobal -= this.OnTeleporterBeginCharging;
			TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
			GlobalEventManager.onClientDamageNotified -= this.OnClientDamageNotified;
			base.OnUninstall();
		}

		// Token: 0x06002602 RID: 9730 RVA: 0x0001BBB9 File Offset: 0x00019DB9
		private void OnTeleporterBeginCharging(TeleporterInteraction teleporterInteraction)
		{
			this.hasBeenHit = false;
		}

		// Token: 0x06002603 RID: 9731 RVA: 0x0001BBC2 File Offset: 0x00019DC2
		private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x06002604 RID: 9732 RVA: 0x0001BBCA File Offset: 0x00019DCA
		private void OnClientDamageNotified(DamageDealtMessage damageDealtMessage)
		{
			if (!this.hasBeenHit && damageDealtMessage.victim && damageDealtMessage.victim == this.localUser.cachedBodyObject)
			{
				this.hasBeenHit = true;
			}
		}

		// Token: 0x06002605 RID: 9733 RVA: 0x000B1B18 File Offset: 0x000AFD18
		private void Check()
		{
			if (this.localUser.cachedBody && this.localUser.cachedBody.healthComponent && this.localUser.cachedBody.healthComponent.alive && !this.hasBeenHit)
			{
				base.Grant();
			}
		}

		// Token: 0x0400289C RID: 10396
		private bool hasBeenHit;
	}
}
