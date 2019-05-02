using System;

namespace RoR2.Achievements
{
	// Token: 0x02000683 RID: 1667
	public class BaseServerAchievement
	{
		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06002538 RID: 9528 RVA: 0x0001B178 File Offset: 0x00019378
		public NetworkUser networkUser
		{
			get
			{
				return this.serverAchievementTracker.networkUser;
			}
		}

		// Token: 0x06002539 RID: 9529 RVA: 0x0001B185 File Offset: 0x00019385
		protected CharacterBody GetCurrentBody()
		{
			return this.networkUser.GetCurrentBody();
		}

		// Token: 0x0600253A RID: 9530 RVA: 0x000025F6 File Offset: 0x000007F6
		public virtual void OnInstall()
		{
		}

		// Token: 0x0600253B RID: 9531 RVA: 0x000025F6 File Offset: 0x000007F6
		public virtual void OnUninstall()
		{
		}

		// Token: 0x0600253C RID: 9532 RVA: 0x0001B192 File Offset: 0x00019392
		protected void Grant()
		{
			this.serverAchievementTracker.CallRpcGrantAchievement(this.achievementDef.serverIndex);
		}

		// Token: 0x0600253D RID: 9533 RVA: 0x000B0140 File Offset: 0x000AE340
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

		// Token: 0x04002834 RID: 10292
		public ServerAchievementTracker serverAchievementTracker;

		// Token: 0x04002835 RID: 10293
		public AchievementDef achievementDef;
	}
}
