using System;
using EntityStates.Destructible;

namespace RoR2.Achievements
{
	// Token: 0x02000697 RID: 1687
	[RegisterAchievement("FindDevilAltar", "Items.NovaOnHeal", null, null)]
	public class FindDevilAltarAchievement : BaseAchievement
	{
		// Token: 0x0600259C RID: 9628 RVA: 0x0001B733 File Offset: 0x00019933
		public override void OnInstall()
		{
			base.OnInstall();
			AltarSkeletonDeath.onDeath += this.OnDeath;
		}

		// Token: 0x0600259D RID: 9629 RVA: 0x0001B74C File Offset: 0x0001994C
		public override void OnUninstall()
		{
			base.OnUninstall();
			AltarSkeletonDeath.onDeath -= this.OnDeath;
		}

		// Token: 0x0600259E RID: 9630 RVA: 0x0001B765 File Offset: 0x00019965
		private void OnDeath()
		{
			base.Grant();
		}
	}
}
