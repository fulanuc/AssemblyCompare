using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001F9 RID: 505
	public class AchievementDef
	{
		// Token: 0x060009EB RID: 2539 RVA: 0x00046154 File Offset: 0x00044354
		public Sprite GetAchievedIcon()
		{
			if (!this.achievedIcon)
			{
				this.achievedIcon = Resources.Load<Sprite>(this.iconPath);
				if (!this.achievedIcon)
				{
					this.achievedIcon = Resources.Load<Sprite>("Textures/AchievementIcons/texPlaceholderAchievement");
				}
			}
			return this.achievedIcon;
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x00008001 File Offset: 0x00006201
		public Sprite GetUnachievedIcon()
		{
			return Resources.Load<Sprite>("Textures/MiscIcons/texUnlockIcon");
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x0000800D File Offset: 0x0000620D
		public string GetAchievementSoundString()
		{
			if (this.unlockableRewardIdentifier.Contains("Characters"))
			{
				return "Play_UI_achievementUnlock_enhanced";
			}
			return "Play_UI_achievementUnlock";
		}

		// Token: 0x04000D15 RID: 3349
		public AchievementIndex index;

		// Token: 0x04000D16 RID: 3350
		public ServerAchievementIndex serverIndex = new ServerAchievementIndex
		{
			intValue = -1
		};

		// Token: 0x04000D17 RID: 3351
		public string identifier;

		// Token: 0x04000D18 RID: 3352
		public string unlockableRewardIdentifier;

		// Token: 0x04000D19 RID: 3353
		public string prerequisiteAchievementIdentifier;

		// Token: 0x04000D1A RID: 3354
		public string nameToken;

		// Token: 0x04000D1B RID: 3355
		public string descriptionToken;

		// Token: 0x04000D1C RID: 3356
		public string iconPath;

		// Token: 0x04000D1D RID: 3357
		public Type type;

		// Token: 0x04000D1E RID: 3358
		public Type serverTrackerType;

		// Token: 0x04000D1F RID: 3359
		private static readonly string[] emptyStringArray = Array.Empty<string>();

		// Token: 0x04000D20 RID: 3360
		public string[] childAchievementIdentifiers = AchievementDef.emptyStringArray;

		// Token: 0x04000D21 RID: 3361
		private Sprite achievedIcon;

		// Token: 0x04000D22 RID: 3362
		private Sprite unachievedIcon;
	}
}
