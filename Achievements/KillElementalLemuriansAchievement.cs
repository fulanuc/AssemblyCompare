using System;

namespace RoR2.Achievements
{
	// Token: 0x020006B4 RID: 1716
	[RegisterAchievement("KillElementalLemurians", "Items.ElementalRings", null, typeof(KillElementalLemuriansAchievement.KillElementalLemuriansServerAchievement))]
	public class KillElementalLemuriansAchievement : BaseAchievement
	{
		// Token: 0x06002663 RID: 9827 RVA: 0x0001BA4F File Offset: 0x00019C4F
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x020006B5 RID: 1717
		private class KillElementalLemuriansServerAchievement : BaseServerAchievement
		{
		}
	}
}
