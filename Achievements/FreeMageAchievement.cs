using System;
using EntityStates.LockedMage;

namespace RoR2.Achievements
{
	// Token: 0x0200069B RID: 1691
	[RegisterAchievement("FreeMage", "Characters.Mage", null, typeof(FreeMageAchievement.FreeMageServerAchievement))]
	public class FreeMageAchievement : BaseAchievement
	{
		// Token: 0x060025AF RID: 9647 RVA: 0x0001B314 File Offset: 0x00019514
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060025B0 RID: 9648 RVA: 0x0001B323 File Offset: 0x00019523
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x0200069C RID: 1692
		private class FreeMageServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025B2 RID: 9650 RVA: 0x0001B825 File Offset: 0x00019A25
			public override void OnInstall()
			{
				base.OnInstall();
				UnlockingMage.onOpened += this.OnOpened;
			}

			// Token: 0x060025B3 RID: 9651 RVA: 0x0001B83E File Offset: 0x00019A3E
			public override void OnUninstall()
			{
				base.OnInstall();
				UnlockingMage.onOpened -= this.OnOpened;
			}

			// Token: 0x060025B4 RID: 9652 RVA: 0x000B073C File Offset: 0x000AE93C
			private void OnOpened(Interactor interactor)
			{
				CharacterBody currentBody = this.serverAchievementTracker.networkUser.GetCurrentBody();
				if (currentBody && currentBody.GetComponent<Interactor>() == interactor)
				{
					base.Grant();
				}
			}
		}
	}
}
