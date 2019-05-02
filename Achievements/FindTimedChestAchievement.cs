using System;
using EntityStates.TimedChest;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x02000698 RID: 1688
	[RegisterAchievement("FindTimedChest", "Items.BFG", null, typeof(FindTimedChestAchievement.FindTimedChestServerAchievement))]
	public class FindTimedChestAchievement : BaseAchievement
	{
		// Token: 0x060025A0 RID: 9632 RVA: 0x0001B314 File Offset: 0x00019514
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060025A1 RID: 9633 RVA: 0x0001B323 File Offset: 0x00019523
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x02000699 RID: 1689
		private class FindTimedChestServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025A3 RID: 9635 RVA: 0x0001B76D File Offset: 0x0001996D
			public override void OnInstall()
			{
				base.OnInstall();
				Debug.Log("subscribed");
				Opening.onOpened += this.OnOpened;
			}

			// Token: 0x060025A4 RID: 9636 RVA: 0x0001B790 File Offset: 0x00019990
			public override void OnUninstall()
			{
				base.OnInstall();
				Opening.onOpened -= this.OnOpened;
			}

			// Token: 0x060025A5 RID: 9637 RVA: 0x0001B7A9 File Offset: 0x000199A9
			private void OnOpened()
			{
				Debug.Log("grant");
				base.Grant();
			}
		}
	}
}
