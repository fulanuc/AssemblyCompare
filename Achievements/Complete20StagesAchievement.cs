using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x02000686 RID: 1670
	[RegisterAchievement("Complete20Stages", "Items.Clover", null, null)]
	public class Complete20StagesAchievement : BaseAchievement
	{
		// Token: 0x0600254A RID: 9546 RVA: 0x0001B24D File Offset: 0x0001944D
		public override void OnInstall()
		{
			base.OnInstall();
			Run.onRunStartGlobal += this.OnRunStart;
		}

		// Token: 0x0600254B RID: 9547 RVA: 0x0001B266 File Offset: 0x00019466
		public override void OnUninstall()
		{
			Run.onRunStartGlobal -= this.OnRunStart;
			this.SetListeningForStats(false);
			base.OnUninstall();
		}

		// Token: 0x0600254C RID: 9548 RVA: 0x0001B286 File Offset: 0x00019486
		private void OnRunStart(Run run)
		{
			this.SetListeningForStats(true);
		}

		// Token: 0x0600254D RID: 9549 RVA: 0x000B02C4 File Offset: 0x000AE4C4
		private void SetListeningForStats(bool shouldListen)
		{
			if (this.listeningForStats == shouldListen)
			{
				return;
			}
			this.listeningForStats = shouldListen;
			if (this.listeningForStats)
			{
				this.subscribedProfile = this.localUser.userProfile;
				this.subscribedProfile.onStatsReceived += this.OnStatsReceived;
				return;
			}
			this.subscribedProfile.onStatsReceived -= this.OnStatsReceived;
			this.subscribedProfile = null;
		}

		// Token: 0x0600254E RID: 9550 RVA: 0x000B0334 File Offset: 0x000AE534
		private void OnStatsReceived()
		{
			PlayerStatsComponent playerStatsComponent = this.playerStatsComponentGetter.Get(this.localUser.cachedMasterObject);
			if (playerStatsComponent && playerStatsComponent.currentStats.GetStatValueULong(StatDef.highestStagesCompleted) >= 20UL)
			{
				base.Grant();
			}
		}

		// Token: 0x0400283A RID: 10298
		private const int requirement = 20;

		// Token: 0x0400283B RID: 10299
		private bool listeningForStats;

		// Token: 0x0400283C RID: 10300
		private UserProfile subscribedProfile;

		// Token: 0x0400283D RID: 10301
		private MemoizedGetComponent<PlayerStatsComponent> playerStatsComponentGetter;
	}
}
