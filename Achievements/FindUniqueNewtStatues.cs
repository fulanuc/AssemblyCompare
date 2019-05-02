using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006AC RID: 1708
	[RegisterAchievement("FindUniqueNewtStatues", "Items.Talisman", null, null)]
	public class FindUniqueNewtStatues : BaseAchievement
	{
		// Token: 0x0600263E RID: 9790 RVA: 0x0001BEF6 File Offset: 0x0001A0F6
		public override void OnInstall()
		{
			base.OnInstall();
			this.Check();
			UserProfile.onUnlockableGranted += this.OnUnlockCheck;
		}

		// Token: 0x0600263F RID: 9791 RVA: 0x0001BF15 File Offset: 0x0001A115
		public override void OnUninstall()
		{
			UserProfile.onUnlockableGranted -= this.OnUnlockCheck;
			base.OnUninstall();
		}

		// Token: 0x06002640 RID: 9792 RVA: 0x0001BF2E File Offset: 0x0001A12E
		public override float ProgressForAchievement()
		{
			return (float)this.UniqueNewtStatueCount() / 8f;
		}

		// Token: 0x06002641 RID: 9793 RVA: 0x0001BF3D File Offset: 0x0001A13D
		private static bool IsUnlockableNewtStatue(UnlockableDef unlockableDef)
		{
			return unlockableDef.name.StartsWith("NewtStatue.");
		}

		// Token: 0x06002642 RID: 9794 RVA: 0x000B1DC0 File Offset: 0x000AFFC0
		private int UniqueNewtStatueCount()
		{
			StatSheet statSheet = this.userProfile.statSheet;
			int num = 0;
			int i = 0;
			int unlockableCount = statSheet.GetUnlockableCount();
			while (i < unlockableCount)
			{
				if (FindUniqueNewtStatues.IsUnlockableNewtStatue(statSheet.GetUnlockable(i)))
				{
					num++;
				}
				i++;
			}
			return num;
		}

		// Token: 0x06002643 RID: 9795 RVA: 0x0001BF4F File Offset: 0x0001A14F
		private void Check()
		{
			if (this.UniqueNewtStatueCount() >= 8)
			{
				base.Grant();
			}
		}

		// Token: 0x06002644 RID: 9796 RVA: 0x000B1E04 File Offset: 0x000B0004
		private void OnUnlockCheck(UserProfile userProfile, string unlockableToken)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableToken);
			if (unlockableDef == null)
			{
				return;
			}
			if (userProfile == this.userProfile && FindUniqueNewtStatues.IsUnlockableNewtStatue(unlockableDef))
			{
				this.Check();
			}
		}

		// Token: 0x040028A4 RID: 10404
		private const int requirement = 8;
	}
}
