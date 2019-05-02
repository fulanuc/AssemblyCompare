using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x0200065E RID: 1630
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class UserProfileListController : MonoBehaviour
	{
		// Token: 0x1700032E RID: 814
		// (get) Token: 0x060024B0 RID: 9392 RVA: 0x0001AB8B File Offset: 0x00018D8B
		private EventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x060024B1 RID: 9393 RVA: 0x0001AB98 File Offset: 0x00018D98
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x060024B2 RID: 9394 RVA: 0x0001ABA6 File Offset: 0x00018DA6
		private void OnEnable()
		{
			this.RebuildElements();
			UserProfile.onAvailableUserProfilesChanged += this.RebuildElements;
		}

		// Token: 0x060024B3 RID: 9395 RVA: 0x0001ABBF File Offset: 0x00018DBF
		private void OnDisable()
		{
			UserProfile.onAvailableUserProfilesChanged -= this.RebuildElements;
		}

		// Token: 0x060024B4 RID: 9396 RVA: 0x000AD7E8 File Offset: 0x000AB9E8
		private void RebuildElements()
		{
			foreach (object obj in this.contentRect)
			{
				UnityEngine.Object.Destroy(((Transform)obj).gameObject);
			}
			this.elementsList.Clear();
			List<string> availableProfileNames = UserProfile.GetAvailableProfileNames();
			for (int i = 0; i < availableProfileNames.Count; i++)
			{
				if (this.allowDefault || !(availableProfileNames[i] == "default"))
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.elementPrefab, this.contentRect);
					UserProfileListElementController component = gameObject.GetComponent<UserProfileListElementController>();
					component.listController = this;
					component.userProfile = UserProfile.GetProfile(availableProfileNames[i]);
					this.elementsList.Add(component);
					gameObject.SetActive(true);
				}
			}
			if (this.elementsList.Count > 0)
			{
				if (this.currentSelectionIndex >= this.elementsList.Count)
				{
					this.currentSelectionIndex = this.elementsList.Count - 1;
				}
				this.eventSystemLocator.eventSystem.SetSelectedGameObject(this.elementsList[this.currentSelectionIndex].gameObject);
			}
			if (this.onListRebuilt != null)
			{
				this.onListRebuilt();
			}
		}

		// Token: 0x060024B5 RID: 9397 RVA: 0x0001ABD2 File Offset: 0x00018DD2
		public ReadOnlyCollection<UserProfileListElementController> GetReadOnlyElementsList()
		{
			return new ReadOnlyCollection<UserProfileListElementController>(this.elementsList);
		}

		// Token: 0x1400005D RID: 93
		// (add) Token: 0x060024B6 RID: 9398 RVA: 0x000AD934 File Offset: 0x000ABB34
		// (remove) Token: 0x060024B7 RID: 9399 RVA: 0x000AD96C File Offset: 0x000ABB6C
		public event UserProfileListController.ProfileSelectedDelegate onProfileSelected;

		// Token: 0x060024B8 RID: 9400 RVA: 0x0001ABDF File Offset: 0x00018DDF
		public void SendProfileSelection(UserProfile userProfile)
		{
			UserProfileListController.ProfileSelectedDelegate profileSelectedDelegate = this.onProfileSelected;
			if (profileSelectedDelegate == null)
			{
				return;
			}
			profileSelectedDelegate(userProfile);
		}

		// Token: 0x1400005E RID: 94
		// (add) Token: 0x060024B9 RID: 9401 RVA: 0x000AD9A4 File Offset: 0x000ABBA4
		// (remove) Token: 0x060024BA RID: 9402 RVA: 0x000AD9DC File Offset: 0x000ABBDC
		public event Action onListRebuilt;

		// Token: 0x0400276A RID: 10090
		public GameObject elementPrefab;

		// Token: 0x0400276B RID: 10091
		public RectTransform contentRect;

		// Token: 0x0400276C RID: 10092
		[Tooltip("Whether or not \"default\" profile appears as a selectable option.")]
		public bool allowDefault = true;

		// Token: 0x0400276D RID: 10093
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x0400276E RID: 10094
		private readonly List<UserProfileListElementController> elementsList = new List<UserProfileListElementController>();

		// Token: 0x0400276F RID: 10095
		private int currentSelectionIndex;

		// Token: 0x0200065F RID: 1631
		// (Invoke) Token: 0x060024BD RID: 9405
		public delegate void ProfileSelectedDelegate(UserProfile userProfile);
	}
}
