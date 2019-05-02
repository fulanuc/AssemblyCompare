using System;

namespace RoR2.Achievements
{
	// Token: 0x02000695 RID: 1685
	public class BaseServerAchievement
	{
		// Token: 0x17000339 RID: 825
		// (get) Token: 0x060025CF RID: 9679 RVA: 0x0001B8B3 File Offset: 0x00019AB3
		public NetworkUser networkUser
		{
			get
			{
				return this.serverAchievementTracker.networkUser;
			}
		}

		// Token: 0x060025D0 RID: 9680 RVA: 0x0001B8C0 File Offset: 0x00019AC0
		protected CharacterBody GetCurrentBody()
		{
			return this.networkUser.GetCurrentBody();
		}

		// Token: 0x060025D1 RID: 9681 RVA: 0x000025DA File Offset: 0x000007DA
		public virtual void OnInstall()
		{
		}

		// Token: 0x060025D2 RID: 9682 RVA: 0x000025DA File Offset: 0x000007DA
		public virtual void OnUninstall()
		{
		}

		// Token: 0x060025D3 RID: 9683 RVA: 0x0001B8CD File Offset: 0x00019ACD
		protected void Grant()
		{
			this.serverAchievementTracker.CallRpcGrantAchievement(this.achievementDef.serverIndex);
		}

		// Token: 0x060025D4 RID: 9684 RVA: 0x000B1830 File Offset: 0x000AFA30
		public static BaseServerAchievement Instantiate(ServerAchievementIndex serverAchievementIndex)
		{
			AchievementDef achievementDef = AchievementManager.GetAchievementDef(serverAchievementIndex);
			if (achievementDef == null || achievementDef.serverTrackerType == null)
			{
				return null;
			}
			BaseServerAchievement baseServerAchievement = (BaseServerAchievement)Activator.CreateInstance(achievementDef.serverTrackerType);
			baseServerAchievement.achievementDef = achievementDef;
			return baseServerAchievement;
		}

		// Token: 0x04002890 RID: 10384
		public ServerAchievementTracker serverAchievementTracker;

		// Token: 0x04002891 RID: 10385
		public AchievementDef achievementDef;
	}
}
