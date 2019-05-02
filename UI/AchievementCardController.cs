using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005A3 RID: 1443
	public class AchievementCardController : MonoBehaviour
	{
		// Token: 0x06002065 RID: 8293 RVA: 0x00017988 File Offset: 0x00015B88
		private static string GetAchievementParentIdentifier(string achievementIdentifier)
		{
			AchievementDef achievementDef = AchievementManager.GetAchievementDef(achievementIdentifier);
			if (achievementDef == null)
			{
				return null;
			}
			return achievementDef.prerequisiteAchievementIdentifier;
		}

		// Token: 0x06002066 RID: 8294 RVA: 0x0009DC14 File Offset: 0x0009BE14
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

		// Token: 0x06002067 RID: 8295 RVA: 0x0009DC3C File Offset: 0x0009BE3C
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

		// Token: 0x040022EA RID: 8938
		public Image iconImage;

		// Token: 0x040022EB RID: 8939
		public LanguageTextMeshController nameLabel;

		// Token: 0x040022EC RID: 8940
		public LanguageTextMeshController descriptionLabel;

		// Token: 0x040022ED RID: 8941
		public LayoutElement tabLayoutElement;

		// Token: 0x040022EE RID: 8942
		public float tabWidth;

		// Token: 0x040022EF RID: 8943
		public GameObject unlockedImage;

		// Token: 0x040022F0 RID: 8944
		public GameObject cantBeAchievedImage;

		// Token: 0x040022F1 RID: 8945
		public TooltipProvider tooltipProvider;
	}
}
