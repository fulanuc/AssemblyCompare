using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x0200069A RID: 1690
	[RegisterAchievement("FindUniqueNewtStatues", "Items.Talisman", null, null)]
	public class FindUniqueNewtStatues : BaseAchievement
	{
		// Token: 0x060025A7 RID: 9639 RVA: 0x0001B7BB File Offset: 0x000199BB
		public override void OnInstall()
		{
			base.OnInstall();
			this.Check();
			UserProfile.onUnlockableGranted += this.OnUnlockCheck;
		}

		// Token: 0x060025A8 RID: 9640 RVA: 0x0001B7DA File Offset: 0x000199DA
		public override void OnUninstall()
		{
			UserProfile.onUnlockableGranted -= this.OnUnlockCheck;
			base.OnUninstall();
		}

		// Token: 0x060025A9 RID: 9641 RVA: 0x0001B7F3 File Offset: 0x000199F3
		public override float ProgressForAchievement()
		{
			return (float)this.UniqueNewtStatueCount() / 8f;
		}

		// Token: 0x060025AA RID: 9642 RVA: 0x0001B802 File Offset: 0x00019A02
		private static bool IsUnlockableNewtStatue(UnlockableDef unlockableDef)
		{
			return unlockableDef.name.StartsWith("NewtStatue.");
		}

		// Token: 0x060025AB RID: 9643 RVA: 0x000B06C8 File Offset: 0x000AE8C8
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

		// Token: 0x060025AC RID: 9644 RVA: 0x0001B814 File Offset: 0x00019A14
		private void Check()
		{
			if (this.UniqueNewtStatueCount() >= 8)
			{
				base.Grant();
			}
		}

		// Token: 0x060025AD RID: 9645 RVA: 0x000B070C File Offset: 0x000AE90C
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

		// Token: 0x04002848 RID: 10312
		private const int requirement = 8;
	}
}
