using System;

namespace RoR2.Achievements
{
	// Token: 0x02000694 RID: 1684
	public abstract class BaseAchievement
	{
		// Token: 0x060025C8 RID: 9672 RVA: 0x0001B819 File Offset: 0x00019A19
		public virtual void OnInstall()
		{
			this.localUser = this.owner.localUser;
			this.userProfile = this.owner.userProfile;
		}

		// Token: 0x060025C9 RID: 9673 RVA: 0x0001B83D File Offset: 0x00019A3D
		public virtual float ProgressForAchievement()
		{
			return 0f;
		}

		// Token: 0x060025CA RID: 9674 RVA: 0x0001B844 File Offset: 0x00019A44
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

		// Token: 0x060025CB RID: 9675 RVA: 0x0001B875 File Offset: 0x00019A75
		public void Grant()
		{
			if (this.shouldGrant)
			{
				return;
			}
			this.shouldGrant = true;
			this.owner.dirtyGrantsCount++;
		}

		// Token: 0x060025CC RID: 9676 RVA: 0x000B17C0 File Offset: 0x000AF9C0
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

		// Token: 0x060025CD RID: 9677 RVA: 0x0001B89A File Offset: 0x00019A9A
		public void SetServerTracked(bool shouldTrack)
		{
			this.owner.SetServerAchievementTracked(this.achievementDef.serverIndex, shouldTrack);
		}

		// Token: 0x0400288B RID: 10379
		public UserAchievementManager owner;

		// Token: 0x0400288C RID: 10380
		protected LocalUser localUser;

		// Token: 0x0400288D RID: 10381
		protected UserProfile userProfile;

		// Token: 0x0400288E RID: 10382
		public bool shouldGrant;

		// Token: 0x0400288F RID: 10383
		public AchievementDef achievementDef;
	}
}
