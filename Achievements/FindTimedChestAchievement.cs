using System;
using EntityStates.TimedChest;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006AA RID: 1706
	[RegisterAchievement("FindTimedChest", "Items.BFG", null, typeof(FindTimedChestAchievement.FindTimedChestServerAchievement))]
	public class FindTimedChestAchievement : BaseAchievement
	{
		// Token: 0x06002637 RID: 9783 RVA: 0x0001BA4F File Offset: 0x00019C4F
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002638 RID: 9784 RVA: 0x0001BA5E File Offset: 0x00019C5E
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006AB RID: 1707
		private class FindTimedChestServerAchievement : BaseServerAchievement
		{
			// Token: 0x0600263A RID: 9786 RVA: 0x0001BEA8 File Offset: 0x0001A0A8
			public override void OnInstall()
			{
				base.OnInstall();
				Debug.Log("subscribed");
				Opening.onOpened += this.OnOpened;
			}

			// Token: 0x0600263B RID: 9787 RVA: 0x0001BECB File Offset: 0x0001A0CB
			public override void OnUninstall()
			{
				base.OnInstall();
				Opening.onOpened -= this.OnOpened;
			}

			// Token: 0x0600263C RID: 9788 RVA: 0x0001BEE4 File Offset: 0x0001A0E4
			private void OnOpened()
			{
				Debug.Log("grant");
				base.Grant();
			}
		}
	}
}
