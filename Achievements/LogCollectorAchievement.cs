using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006B9 RID: 1721
	[RegisterAchievement("LogCollector", "Items.Scanner", null, null)]
	public class LogCollectorAchievement : BaseAchievement
	{
		// Token: 0x06002672 RID: 9842 RVA: 0x0001C1F1 File Offset: 0x0001A3F1
		public override void OnInstall()
		{
			base.OnInstall();
			this.Check();
			UserProfile.onUnlockableGranted += this.OnUnlockCheck;
		}

		// Token: 0x06002673 RID: 9843 RVA: 0x0001C210 File Offset: 0x0001A410
		public override void OnUninstall()
		{
			UserProfile.onUnlockableGranted -= this.OnUnlockCheck;
			base.OnUninstall();
		}

		// Token: 0x06002674 RID: 9844 RVA: 0x0001C229 File Offset: 0x0001A429
		public override float ProgressForAchievement()
		{
			return (float)this.MonsterLogCount() / 10f;
		}

		// Token: 0x06002675 RID: 9845 RVA: 0x0001C238 File Offset: 0x0001A438
		private static bool IsUnlockableMonsterLog(UnlockableDef unlockableDef)
		{
			return unlockableDef.name.StartsWith("Logs.");
		}

		// Token: 0x06002676 RID: 9846 RVA: 0x000B1FCC File Offset: 0x000B01CC
		private int MonsterLogCount()
		{
			StatSheet statSheet = this.userProfile.statSheet;
			int num = 0;
			int i = 0;
			int unlockableCount = statSheet.GetUnlockableCount();
			while (i < unlockableCount)
			{
				if (LogCollectorAchievement.IsUnlockableMonsterLog(statSheet.GetUnlockable(i)))
				{
					num++;
				}
				i++;
			}
			return num;
		}

		// Token: 0x06002677 RID: 9847 RVA: 0x0001C24A File Offset: 0x0001A44A
		private void Check()
		{
			if (this.MonsterLogCount() >= 10)
			{
				base.Grant();
			}
		}

		// Token: 0x06002678 RID: 9848 RVA: 0x000B2010 File Offset: 0x000B0210
		private void OnUnlockCheck(UserProfile userProfile, string unlockableToken)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableToken);
			if (unlockableDef == null)
			{
				return;
			}
			if (userProfile == this.userProfile && LogCollectorAchievement.IsUnlockableMonsterLog(unlockableDef))
			{
				this.Check();
			}
		}

		// Token: 0x040028A9 RID: 10409
		private const int requirement = 10;
	}
}
