using System;

namespace RoR2.Achievements
{
	// Token: 0x020006B1 RID: 1713
	[RegisterAchievement("HardHitter", "Items.ShockNearby", null, null)]
	public class HardHitterAchievement : BaseAchievement
	{
		// Token: 0x06002658 RID: 9816 RVA: 0x0001C084 File Offset: 0x0001A284
		public override void OnInstall()
		{
			base.OnInstall();
			GlobalEventManager.onClientDamageNotified += this.CheckDamage;
		}

		// Token: 0x06002659 RID: 9817 RVA: 0x0001C09D File Offset: 0x0001A29D
		public override void OnUninstall()
		{
			GlobalEventManager.onClientDamageNotified -= this.CheckDamage;
			base.OnUninstall();
		}

		// Token: 0x0600265A RID: 9818 RVA: 0x0001C0B6 File Offset: 0x0001A2B6
		public void CheckDamage(DamageDealtMessage damageDealtMessage)
		{
			if (damageDealtMessage.damage >= 5000f && damageDealtMessage.attacker && damageDealtMessage.attacker == this.localUser.cachedBodyObject)
			{
				base.Grant();
			}
		}

		// Token: 0x040028A6 RID: 10406
		private const float requirement = 5000f;
	}
}
