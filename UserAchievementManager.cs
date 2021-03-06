﻿using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Achievements;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001FB RID: 507
	public class UserAchievementManager
	{
		// Token: 0x060009F4 RID: 2548 RVA: 0x0000806C File Offset: 0x0000626C
		public void SetServerAchievementTracked(ServerAchievementIndex serverAchievementIndex, bool shouldTrack)
		{
			if (this.serverAchievementTrackingMask[serverAchievementIndex.intValue] == shouldTrack)
			{
				return;
			}
			this.serverAchievementTrackingMask[serverAchievementIndex.intValue] = shouldTrack;
			this.serverAchievementTrackingMaskDirty = true;
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x00008094 File Offset: 0x00006294
		public void TransmitAchievementRequestsToServer()
		{
			if (this.localUser.currentNetworkUser)
			{
				this.localUser.currentNetworkUser.GetComponent<ServerAchievementTracker>().SendAchievementTrackerRequestsMaskToServer(this.serverAchievementTrackingMask);
			}
		}

		// Token: 0x060009F6 RID: 2550 RVA: 0x00046488 File Offset: 0x00044688
		public void Update()
		{
			if (this.serverAchievementTrackingMaskDirty)
			{
				this.serverAchievementTrackingMaskDirty = false;
				this.TransmitAchievementRequestsToServer();
			}
			int num = this.achievementsList.Count - 1;
			while (num >= 0 && this.dirtyGrantsCount > 0)
			{
				BaseAchievement baseAchievement = this.achievementsList[num];
				if (baseAchievement.shouldGrant)
				{
					this.dirtyGrantsCount--;
					this.achievementsList.RemoveAt(num);
					this.userProfile.AddAchievement(baseAchievement.achievementDef.identifier, true);
					baseAchievement.OnGranted();
					baseAchievement.OnUninstall();
					NetworkUser currentNetworkUser = this.localUser.currentNetworkUser;
					if (currentNetworkUser != null)
					{
						currentNetworkUser.CallCmdReportAchievement(baseAchievement.achievementDef.nameToken);
					}
				}
				num--;
			}
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x00046548 File Offset: 0x00044748
		public void GrantAchievement(AchievementDef achievementDef)
		{
			for (int i = 0; i < this.achievementsList.Count; i++)
			{
				if (this.achievementsList[i].achievementDef == achievementDef)
				{
					this.achievementsList[i].Grant();
				}
			}
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x00046590 File Offset: 0x00044790
		public void HandleServerAchievementCompleted(ServerAchievementIndex serverAchievementIndex)
		{
			BaseAchievement baseAchievement = this.achievementsList.FirstOrDefault((BaseAchievement a) => a.achievementDef.serverIndex == serverAchievementIndex);
			if (baseAchievement == null)
			{
				return;
			}
			baseAchievement.Grant();
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x000465CC File Offset: 0x000447CC
		public float GetAchievementProgress(AchievementDef achievementDef)
		{
			BaseAchievement baseAchievement = this.achievementsList.FirstOrDefault((BaseAchievement a) => a.achievementDef == achievementDef);
			if (baseAchievement == null)
			{
				return 1f;
			}
			return baseAchievement.ProgressForAchievement();
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x0004660C File Offset: 0x0004480C
		public void OnInstall(LocalUser localUser)
		{
			this.localUser = localUser;
			this.userProfile = localUser.userProfile;
			foreach (string text in AchievementManager.readOnlyAchievementIdentifiers)
			{
				AchievementDef achievementDef = AchievementManager.GetAchievementDef(text);
				if (this.userProfile.HasAchievement(text))
				{
					if (!this.userProfile.HasUnlockable(achievementDef.unlockableRewardIdentifier))
					{
						Debug.LogFormat("UserProfile {0} has achievement {1} but not its unlockable {2}. Granting.", new object[]
						{
							this.userProfile.name,
							achievementDef.nameToken,
							achievementDef.unlockableRewardIdentifier
						});
						this.userProfile.AddUnlockToken(achievementDef.unlockableRewardIdentifier);
					}
				}
				else
				{
					BaseAchievement baseAchievement = (BaseAchievement)Activator.CreateInstance(achievementDef.type);
					baseAchievement.achievementDef = achievementDef;
					baseAchievement.owner = this;
					this.achievementsList.Add(baseAchievement);
					baseAchievement.OnInstall();
				}
			}
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x00046708 File Offset: 0x00044908
		public void OnUninstall()
		{
			for (int i = this.achievementsList.Count - 1; i >= 0; i--)
			{
				this.achievementsList[i].OnUninstall();
			}
			this.achievementsList.Clear();
			this.localUser = null;
			this.userProfile = null;
		}

		// Token: 0x04000D2B RID: 3371
		private readonly List<BaseAchievement> achievementsList = new List<BaseAchievement>();

		// Token: 0x04000D2C RID: 3372
		public LocalUser localUser;

		// Token: 0x04000D2D RID: 3373
		public UserProfile userProfile;

		// Token: 0x04000D2E RID: 3374
		public int dirtyGrantsCount;

		// Token: 0x04000D2F RID: 3375
		private readonly bool[] serverAchievementTrackingMask = new bool[AchievementManager.serverAchievementCount];

		// Token: 0x04000D30 RID: 3376
		private bool serverAchievementTrackingMaskDirty;
	}
}
