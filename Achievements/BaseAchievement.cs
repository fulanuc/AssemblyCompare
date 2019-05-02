using System;

namespace RoR2.Achievements
{
	// Token: 0x02000682 RID: 1666
	public abstract class BaseAchievement
	{
		// Token: 0x06002531 RID: 9521 RVA: 0x0001B0DE File Offset: 0x000192DE
		public virtual void OnInstall()
		{
			this.localUser = this.owner.localUser;
			this.userProfile = this.owner.userProfile;
		}

		// Token: 0x06002532 RID: 9522 RVA: 0x0001B102 File Offset: 0x00019302
		public virtual float ProgressForAchievement()
		{
			return 0f;
		}

		// Token: 0x06002533 RID: 9523 RVA: 0x0001B109 File Offset: 0x00019309
		public virtual void OnUninstall()
		{
			if (this.achievementDef.serverTrackerType != null)
			{
				this.SetServerTracked(false);
			}
			this.owner = null;
			this.localUser = null;
			this.userProfile = null;
		}

		// Token: 0x06002534 RID: 9524 RVA: 0x0001B13A File Offset: 0x0001933A
		public void Grant()
		{
			if (this.shouldGrant)
			{
				return;
			}
			this.shouldGrant = true;
			this.owner.dirtyGrantsCount++;
		}

		// Token: 0x06002535 RID: 9525 RVA: 0x000B00D0 File Offset: 0x000AE2D0
		public virtual void OnGranted()
		{
			if (!string.IsNullOrEmpty(this.achievementDef.unlockableRewardIdentifier))
			{
				if (this.localUser.currentNetworkUser)
				{
					UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(this.achievementDef.unlockableRewardIdentifier);
					this.localUser.currentNetworkUser.CallCmdReportUnlock(unlockableDef.index);
				}
				this.userProfile.AddUnlockToken(this.achievementDef.unlockableRewardIdentifier);
			}
		}

		// Token: 0x06002536 RID: 9526 RVA: 0x0001B15F File Offset: 0x0001935F
		public void SetServerTracked(bool shouldTrack)
		{
			this.owner.SetServerAchievementTracked(this.achievementDef.serverIndex, shouldTrack);
		}

		// Token: 0x0400282F RID: 10287
		public UserAchievementManager owner;

		// Token: 0x04002830 RID: 10288
		protected LocalUser localUser;

		// Token: 0x04002831 RID: 10289
		protected UserProfile userProfile;

		// Token: 0x04002832 RID: 10290
		public bool shouldGrant;

		// Token: 0x04002833 RID: 10291
		public AchievementDef achievementDef;
	}
}
