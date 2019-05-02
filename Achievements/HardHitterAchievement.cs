using System;

namespace RoR2.Achievements
{
	// Token: 0x0200069F RID: 1695
	[RegisterAchievement("HardHitter", "Items.ShockNearby", null, null)]
	public class HardHitterAchievement : BaseAchievement
	{
		// Token: 0x060025C1 RID: 9665 RVA: 0x0001B949 File Offset: 0x00019B49
		public override void OnInstall()
		{
			base.OnInstall();
			GlobalEventManager.onClientDamageNotified += this.CheckDamage;
		}

		// Token: 0x060025C2 RID: 9666 RVA: 0x0001B962 File Offset: 0x00019B62
		public override void OnUninstall()
		{
			GlobalEventManager.onClientDamageNotified -= this.CheckDamage;
			base.OnUninstall();
		}

		// Token: 0x060025C3 RID: 9667 RVA: 0x0001B97B File Offset: 0x00019B7B
		public void CheckDamage(DamageDealtMessage damageDealtMessage)
		{
			if (damageDealtMessage.damage >= 5000f && damageDealtMessage.attacker && damageDealtMessage.attacker == this.localUser.cachedBodyObject)
			{
				base.Grant();
			}
		}

		// Token: 0x0400284A RID: 10314
		private const float requirement = 5000f;
	}
}
