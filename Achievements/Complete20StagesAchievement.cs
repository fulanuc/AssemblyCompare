using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x02000698 RID: 1688
	[RegisterAchievement("Complete20Stages", "Items.Clover", null, null)]
	public class Complete20StagesAchievement : BaseAchievement
	{
		// Token: 0x060025E1 RID: 9697 RVA: 0x0001B988 File Offset: 0x00019B88
		public override void OnInstall()
		{
			base.OnInstall();
			Run.onRunStartGlobal += this.OnRunStart;
		}

		// Token: 0x060025E2 RID: 9698 RVA: 0x0001B9A1 File Offset: 0x00019BA1
		public override void OnUninstall()
		{
			Run.onRunStartGlobal -= this.OnRunStart;
			this.SetListeningForStats(false);
			base.OnUninstall();
		}

		// Token: 0x060025E3 RID: 9699 RVA: 0x0001B9C1 File Offset: 0x00019BC1
		private void OnRunStart(Run run)
		{
			this.SetListeningForStats(true);
		}

		// Token: 0x060025E4 RID: 9700 RVA: 0x000B19B4 File Offset: 0x000AFBB4
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

		// Token: 0x060025E5 RID: 9701 RVA: 0x000B1A24 File Offset: 0x000AFC24
		private void OnStatsReceived()
		{
			PlayerStatsComponent playerStatsComponent = this.playerStatsComponentGetter.Get(this.localUser.cachedMasterObject);
			if (playerStatsComponent && playerStatsComponent.currentStats.GetStatValueULong(StatDef.highestStagesCompleted) >= 20UL)
			{
				base.Grant();
			}
		}

		// Token: 0x04002896 RID: 10390
		private const int requirement = 20;

		// Token: 0x04002897 RID: 10391
		private bool listeningForStats;

		// Token: 0x04002898 RID: 10392
		private UserProfile subscribedProfile;

		// Token: 0x04002899 RID: 10393
		private MemoizedGetComponent<PlayerStatsComponent> playerStatsComponentGetter;
	}
}
