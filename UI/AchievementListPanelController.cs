using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005A4 RID: 1444
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class AchievementListPanelController : MonoBehaviour
	{
		// Token: 0x06002069 RID: 8297 RVA: 0x0001799B File Offset: 0x00015B9B
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x0600206A RID: 8298 RVA: 0x000179A9 File Offset: 0x00015BA9
		private void OnEnable()
		{
			this.Rebuild();
		}

		// Token: 0x0600206B RID: 8299 RVA: 0x0009DE14 File Offset: 0x0009C014
		private UserProfile GetUserProfile()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			if (eventSystem)
			{
				LocalUser localUser = LocalUserManager.FindLocalUser(eventSystem.player);
				if (localUser != null)
				{
					return localUser.userProfile;
				}
			}
			return null;
		}

		// Token: 0x0600206C RID: 8300 RVA: 0x000179B1 File Offset: 0x00015BB1
		static AchievementListPanelController()
		{
			AchievementListPanelController.BuildAchievementListOrder();
			AchievementManager.onAchievementsRegistered += AchievementListPanelController.BuildAchievementListOrder;
		}

		// Token: 0x0600206D RID: 8301 RVA: 0x0009DE4C File Offset: 0x0009C04C
		private static void BuildAchievementListOrder()
		{
			AchievementListPanelController.sortedAchievementIdentifiers.Clear();
			HashSet<string> encounteredIdentifiers = new HashSet<string>();
			ReadOnlyCollection<string> readOnlyAchievementIdentifiers = AchievementManager.readOnlyAchievementIdentifiers;
			for (int i = 0; i < readOnlyAchievementIdentifiers.Count; i++)
			{
				string achievementIdentifier = readOnlyAchievementIdentifiers[i];
				if (string.IsNullOrEmpty(AchievementManager.GetAchievementDef(achievementIdentifier).prerequisiteAchievementIdentifier))
				{
					AchievementListPanelController.AddAchievementToOrderedList(achievementIdentifier, encounteredIdentifiers);
				}
			}
		}

		// Token: 0x0600206E RID: 8302 RVA: 0x0009DEA4 File Offset: 0x0009C0A4
		private static void AddAchievementToOrderedList(string achievementIdentifier, HashSet<string> encounteredIdentifiers)
		{
			if (encounteredIdentifiers.Contains(achievementIdentifier))
			{
				return;
			}
			encounteredIdentifiers.Add(achievementIdentifier);
			AchievementListPanelController.sortedAchievementIdentifiers.Add(achievementIdentifier);
			string[] childAchievementIdentifiers = AchievementManager.GetAchievementDef(achievementIdentifier).childAchievementIdentifiers;
			for (int i = 0; i < childAchievementIdentifiers.Length; i++)
			{
				AchievementListPanelController.AddAchievementToOrderedList(childAchievementIdentifiers[i], encounteredIdentifiers);
			}
		}

		// Token: 0x0600206F RID: 8303 RVA: 0x0009DEF4 File Offset: 0x0009C0F4
		private void SetCardCount(int desiredCardCount)
		{
			while (this.cardsList.Count < desiredCardCount)
			{
				AchievementCardController component = UnityEngine.Object.Instantiate<GameObject>(this.achievementCardPrefab, this.container).GetComponent<AchievementCardController>();
				this.cardsList.Add(component);
			}
			while (this.cardsList.Count > desiredCardCount)
			{
				UnityEngine.Object.Destroy(this.cardsList[this.cardsList.Count - 1].gameObject);
				this.cardsList.RemoveAt(this.cardsList.Count - 1);
			}
		}

		// Token: 0x06002070 RID: 8304 RVA: 0x0009DF80 File Offset: 0x0009C180
		private void Rebuild()
		{
			UserProfile userProfile = this.GetUserProfile();
			this.SetCardCount(AchievementListPanelController.sortedAchievementIdentifiers.Count);
			for (int i = 0; i < AchievementListPanelController.sortedAchievementIdentifiers.Count; i++)
			{
				this.cardsList[i].SetAchievement(AchievementListPanelController.sortedAchievementIdentifiers[i], userProfile);
			}
		}

		// Token: 0x040022F2 RID: 8946
		public GameObject achievementCardPrefab;

		// Token: 0x040022F3 RID: 8947
		public RectTransform container;

		// Token: 0x040022F4 RID: 8948
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040022F5 RID: 8949
		private readonly List<AchievementCardController> cardsList = new List<AchievementCardController>();

		// Token: 0x040022F6 RID: 8950
		private static readonly List<string> sortedAchievementIdentifiers = new List<string>();
	}
}
