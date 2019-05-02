using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006C7 RID: 1735
	[RegisterAchievement("TotalDronesRepaired", "Items.DroneBackup", null, null)]
	public class TotalDronesRepairedAchievement : BaseAchievement
	{
		// Token: 0x060026AF RID: 9903 RVA: 0x0001C555 File Offset: 0x0001A755
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x060026B0 RID: 9904 RVA: 0x0001C57A File Offset: 0x0001A77A
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x060026B1 RID: 9905 RVA: 0x0001C599 File Offset: 0x0001A799
		public override float ProgressForAchievement()
		{
			return (float)this.TotalDronesPurchased() / 30f;
		}

		// Token: 0x060026B2 RID: 9906 RVA: 0x0001C5A8 File Offset: 0x0001A7A8
		private int TotalDronesPurchased()
		{
			return (int)this.userProfile.statSheet.GetStatValueULong(StatDef.totalDronesPurchased);
		}

		// Token: 0x060026B3 RID: 9907 RVA: 0x0001C5C0 File Offset: 0x0001A7C0
		private void Check()
		{
			if (this.TotalDronesPurchased() >= 30)
			{
				base.Grant();
			}
		}

		// Token: 0x040028B3 RID: 10419
		private const int requirement = 30;
	}
}
