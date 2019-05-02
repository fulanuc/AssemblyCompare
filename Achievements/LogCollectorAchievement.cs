using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006A7 RID: 1703
	[RegisterAchievement("LogCollector", "Items.Scanner", null, null)]
	public class LogCollectorAchievement : BaseAchievement
	{
		// Token: 0x060025DB RID: 9691 RVA: 0x0001BAB6 File Offset: 0x00019CB6
		public override void OnInstall()
		{
			base.OnInstall();
			this.Check();
			UserProfile.onUnlockableGranted += this.OnUnlockCheck;
		}

		// Token: 0x060025DC RID: 9692 RVA: 0x0001BAD5 File Offset: 0x00019CD5
		public override void OnUninstall()
		{
			UserProfile.onUnlockableGranted -= this.OnUnlockCheck;
			base.OnUninstall();
		}

		// Token: 0x060025DD RID: 9693 RVA: 0x0001BAEE File Offset: 0x00019CEE
		public override float ProgressForAchievement()
		{
			return (float)this.MonsterLogCount() / 10f;
		}

		// Token: 0x060025DE RID: 9694 RVA: 0x0001BAFD File Offset: 0x00019CFD
		private static bool IsUnlockableMonsterLog(UnlockableDef unlockableDef)
		{
			return unlockableDef.name.StartsWith("Logs.");
		}

		// Token: 0x060025DF RID: 9695 RVA: 0x000B08D4 File Offset: 0x000AEAD4
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

		// Token: 0x060025E0 RID: 9696 RVA: 0x0001BB0F File Offset: 0x00019D0F
		private void Check()
		{
			if (this.MonsterLogCount() >= 10)
			{
				base.Grant();
			}
		}

		// Token: 0x060025E1 RID: 9697 RVA: 0x000B0918 File Offset: 0x000AEB18
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

		// Token: 0x0400284D RID: 10317
		private const int requirement = 10;
	}
}
