using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006B5 RID: 1717
	[RegisterAchievement("TotalDronesRepaired", "Items.DroneBackup", null, null)]
	public class TotalDronesRepairedAchievement : BaseAchievement
	{
		// Token: 0x06002618 RID: 9752 RVA: 0x0001BE1A File Offset: 0x0001A01A
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x06002619 RID: 9753 RVA: 0x0001BE3F File Offset: 0x0001A03F
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600261A RID: 9754 RVA: 0x0001BE5E File Offset: 0x0001A05E
		public override float ProgressForAchievement()
		{
			return (float)this.TotalDronesPurchased() / 30f;
		}

		// Token: 0x0600261B RID: 9755 RVA: 0x0001BE6D File Offset: 0x0001A06D
		private int TotalDronesPurchased()
		{
			return (int)this.userProfile.statSheet.GetStatValueULong(StatDef.totalDronesPurchased);
		}

		// Token: 0x0600261C RID: 9756 RVA: 0x0001BE85 File Offset: 0x0001A085
		private void Check()
		{
			if (this.TotalDronesPurchased() >= 30)
			{
				base.Grant();
			}
		}

		// Token: 0x04002857 RID: 10327
		private const int requirement = 30;
	}
}
