using System;

namespace RoR2.Achievements
{
	// Token: 0x020006A2 RID: 1698
	[RegisterAchievement("KillElementalLemurians", "Items.ElementalRings", null, typeof(KillElementalLemuriansAchievement.KillElementalLemuriansServerAchievement))]
	public class KillElementalLemuriansAchievement : BaseAchievement
	{
		// Token: 0x060025CC RID: 9676 RVA: 0x0001B314 File Offset: 0x00019514
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x020006A3 RID: 1699
		private class KillElementalLemuriansServerAchievement : BaseServerAchievement
		{
		}
	}
}
