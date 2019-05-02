using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005B6 RID: 1462
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class AchievementListPanelController : MonoBehaviour
	{
		// Token: 0x060020FA RID: 8442 RVA: 0x00018095 File Offset: 0x00016295
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x060020FB RID: 8443 RVA: 0x000180A3 File Offset: 0x000162A3
		private void OnEnable()
		{
			this.Rebuild();
		}

		// Token: 0x060020FC RID: 8444 RVA: 0x0009F3E8 File Offset: 0x0009D5E8
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

		// Token: 0x060020FD RID: 8445 RVA: 0x000180AB File Offset: 0x000162AB
		static AchievementListPanelController()
		{
			AchievementListPanelController.BuildAchievementListOrder();
			AchievementManager.onAchievementsRegistered += AchievementListPanelController.BuildAchievementListOrder;
		}

		// Token: 0x060020FE RID: 8446 RVA: 0x0009F420 File Offset: 0x0009D620
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

		// Token: 0x060020FF RID: 8447 RVA: 0x0009F478 File Offset: 0x0009D678
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

		// Token: 0x06002100 RID: 8448 RVA: 0x0009F4C8 File Offset: 0x0009D6C8
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

		// Token: 0x06002101 RID: 8449 RVA: 0x0009F554 File Offset: 0x0009D754
		private void Rebuild()
		{
			UserProfile userProfile = this.GetUserProfile();
			this.SetCardCount(AchievementListPanelController.sortedAchievementIdentifiers.Count);
			for (int i = 0; i < AchievementListPanelController.sortedAchievementIdentifiers.Count; i++)
			{
				this.cardsList[i].SetAchievement(AchievementListPanelController.sortedAchievementIdentifiers[i], userProfile);
			}
		}

		// Token: 0x04002346 RID: 9030
		public GameObject achievementCardPrefab;

		// Token: 0x04002347 RID: 9031
		public RectTransform container;

		// Token: 0x04002348 RID: 9032
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002349 RID: 9033
		private readonly List<AchievementCardController> cardsList = new List<AchievementCardController>();

		// Token: 0x0400234A RID: 9034
		private static readonly List<string> sortedAchievementIdentifiers = new List<string>();
	}
}
