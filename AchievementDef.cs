using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001F9 RID: 505
	public class AchievementDef
	{
		// Token: 0x060009EE RID: 2542 RVA: 0x00046400 File Offset: 0x00044600
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

		// Token: 0x060009EF RID: 2543 RVA: 0x00008010 File Offset: 0x00006210
		public Sprite GetUnachievedIcon()
		{
			return Resources.Load<Sprite>("Textures/MiscIcons/texUnlockIcon");
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x0000801C File Offset: 0x0000621C
		public string GetAchievementSoundString()
		{
			if (this.unlockableRewardIdentifier.Contains("Characters"))
			{
				return "Play_UI_achievementUnlock_enhanced";
			}
			return "Play_UI_achievementUnlock";
		}

		// Token: 0x04000D19 RID: 3353
		public AchievementIndex index;

		// Token: 0x04000D1A RID: 3354
		public ServerAchievementIndex serverIndex = new ServerAchievementIndex
		{
			intValue = -1
		};

		// Token: 0x04000D1B RID: 3355
		public string identifier;

		// Token: 0x04000D1C RID: 3356
		public string unlockableRewardIdentifier;

		// Token: 0x04000D1D RID: 3357
		public string prerequisiteAchievementIdentifier;

		// Token: 0x04000D1E RID: 3358
		public string nameToken;

		// Token: 0x04000D1F RID: 3359
		public string descriptionToken;

		// Token: 0x04000D20 RID: 3360
		public string iconPath;

		// Token: 0x04000D21 RID: 3361
		public Type type;

		// Token: 0x04000D22 RID: 3362
		public Type serverTrackerType;

		// Token: 0x04000D23 RID: 3363
		private static readonly string[] emptyStringArray = Array.Empty<string>();

		// Token: 0x04000D24 RID: 3364
		public string[] childAchievementIdentifiers = AchievementDef.emptyStringArray;

		// Token: 0x04000D25 RID: 3365
		private Sprite achievedIcon;

		// Token: 0x04000D26 RID: 3366
		private Sprite unachievedIcon;
	}
}
