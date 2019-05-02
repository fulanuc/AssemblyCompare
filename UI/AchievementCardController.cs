using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B5 RID: 1461
	public class AchievementCardController : MonoBehaviour
	{
		// Token: 0x060020F6 RID: 8438 RVA: 0x00018082 File Offset: 0x00016282
		private static string GetAchievementParentIdentifier(string achievementIdentifier)
		{
			AchievementDef achievementDef = AchievementManager.GetAchievementDef(achievementIdentifier);
			if (achievementDef == null)
			{
				return null;
			}
			return achievementDef.prerequisiteAchievementIdentifier;
		}

		// Token: 0x060020F7 RID: 8439 RVA: 0x0009F1E8 File Offset: 0x0009D3E8
		private static int CalcAchievementTabCount(string achievementIdentifier)
		{
			int num = -1;
			while (!string.IsNullOrEmpty(achievementIdentifier))
			{
				num++;
				achievementIdentifier = AchievementCardController.GetAchievementParentIdentifier(achievementIdentifier);
			}
			return num;
		}

		// Token: 0x060020F8 RID: 8440 RVA: 0x0009F210 File Offset: 0x0009D410
		public void SetAchievement(string achievementIdentifier, UserProfile userProfile)
		{
			AchievementDef achievementDef = AchievementManager.GetAchievementDef(achievementIdentifier);
			if (achievementDef != null)
			{
				bool flag = userProfile.HasAchievement(achievementIdentifier);
				bool flag2 = userProfile.CanSeeAchievement(achievementIdentifier);
				if (this.iconImage)
				{
					this.iconImage.sprite = (flag ? achievementDef.GetAchievedIcon() : achievementDef.GetUnachievedIcon());
				}
				if (this.nameLabel)
				{
					this.nameLabel.token = (userProfile.CanSeeAchievement(achievementIdentifier) ? achievementDef.nameToken : "???");
				}
				if (this.descriptionLabel)
				{
					this.descriptionLabel.token = (userProfile.CanSeeAchievement(achievementIdentifier) ? achievementDef.descriptionToken : "???");
				}
				if (this.unlockedImage)
				{
					this.unlockedImage.gameObject.SetActive(flag);
				}
				if (this.cantBeAchievedImage)
				{
					this.cantBeAchievedImage.gameObject.SetActive(!flag2);
				}
				if (this.tooltipProvider)
				{
					string overrideBodyText = "???";
					if (flag2)
					{
						if (flag)
						{
							UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(achievementDef.unlockableRewardIdentifier);
							if (unlockableDef != null)
							{
								string @string = Language.GetString("ACHIEVEMENT_CARD_REWARD_FORMAT");
								string string2 = Language.GetString(unlockableDef.nameToken);
								overrideBodyText = string.Format(@string, string2);
							}
						}
						else
						{
							string string3 = Language.GetString("ACHIEVEMENT_CARD_REWARD_FORMAT");
							string arg = "???";
							overrideBodyText = string.Format(string3, arg);
						}
					}
					else
					{
						AchievementDef achievementDef2 = AchievementManager.GetAchievementDef(achievementDef.prerequisiteAchievementIdentifier);
						if (achievementDef2 != null)
						{
							string string4 = Language.GetString("ACHIEVEMENT_CARD_PREREQ_FORMAT");
							string string5 = Language.GetString(achievementDef2.nameToken);
							overrideBodyText = string.Format(string4, string5);
						}
					}
					this.tooltipProvider.titleToken = (flag2 ? achievementDef.nameToken : "???");
					this.tooltipProvider.overrideBodyText = overrideBodyText;
				}
				if (this.tabLayoutElement)
				{
					this.tabLayoutElement.preferredWidth = (float)AchievementCardController.CalcAchievementTabCount(achievementIdentifier) * this.tabWidth;
				}
			}
		}

		// Token: 0x0400233E RID: 9022
		public Image iconImage;

		// Token: 0x0400233F RID: 9023
		public LanguageTextMeshController nameLabel;

		// Token: 0x04002340 RID: 9024
		public LanguageTextMeshController descriptionLabel;

		// Token: 0x04002341 RID: 9025
		public LayoutElement tabLayoutElement;

		// Token: 0x04002342 RID: 9026
		public float tabWidth;

		// Token: 0x04002343 RID: 9027
		public GameObject unlockedImage;

		// Token: 0x04002344 RID: 9028
		public GameObject cantBeAchievedImage;

		// Token: 0x04002345 RID: 9029
		public TooltipProvider tooltipProvider;
	}
}
