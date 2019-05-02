using System;
using EntityStates.LockedMage;

namespace RoR2.Achievements
{
	// Token: 0x020006AD RID: 1709
	[RegisterAchievement("FreeMage", "Characters.Mage", null, typeof(FreeMageAchievement.FreeMageServerAchievement))]
	public class FreeMageAchievement : BaseAchievement
	{
		// Token: 0x06002646 RID: 9798 RVA: 0x0001BA4F File Offset: 0x00019C4F
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002647 RID: 9799 RVA: 0x0001BA5E File Offset: 0x00019C5E
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006AE RID: 1710
		private class FreeMageServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002649 RID: 9801 RVA: 0x0001BF60 File Offset: 0x0001A160
			public override void OnInstall()
			{
				base.OnInstall();
				UnlockingMage.onOpened += this.OnOpened;
			}

			// Token: 0x0600264A RID: 9802 RVA: 0x0001BF79 File Offset: 0x0001A179
			public override void OnUninstall()
			{
				base.OnInstall();
				UnlockingMage.onOpened -= this.OnOpened;
			}

			// Token: 0x0600264B RID: 9803 RVA: 0x000B1E34 File Offset: 0x000B0034
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
