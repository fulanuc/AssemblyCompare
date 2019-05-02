using System;
using EntityStates.Destructible;

namespace RoR2.Achievements
{
	// Token: 0x020006A9 RID: 1705
	[RegisterAchievement("FindDevilAltar", "Items.NovaOnHeal", null, null)]
	public class FindDevilAltarAchievement : BaseAchievement
	{
		// Token: 0x06002633 RID: 9779 RVA: 0x0001BE6E File Offset: 0x0001A06E
		public override void OnInstall()
		{
			base.OnInstall();
			AltarSkeletonDeath.onDeath += this.OnDeath;
		}

		// Token: 0x06002634 RID: 9780 RVA: 0x0001BE87 File Offset: 0x0001A087
		public override void OnUninstall()
		{
			base.OnUninstall();
			AltarSkeletonDeath.onDeath -= this.OnDeath;
		}

		// Token: 0x06002635 RID: 9781 RVA: 0x0001BEA0 File Offset: 0x0001A0A0
		private void OnDeath()
		{
			base.Grant();
		}
	}
}
