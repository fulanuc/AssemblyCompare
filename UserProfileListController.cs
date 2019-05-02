using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x0200064C RID: 1612
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class UserProfileListController : MonoBehaviour
	{
		// Token: 0x1700031C RID: 796
		// (get) Token: 0x06002420 RID: 9248 RVA: 0x0001A4BD File Offset: 0x000186BD
		private EventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x06002421 RID: 9249 RVA: 0x0001A4CA File Offset: 0x000186CA
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x06002422 RID: 9250 RVA: 0x0001A4D8 File Offset: 0x000186D8
		private void OnEnable()
		{
			this.RebuildElements();
			UserProfile.onAvailableUserProfilesChanged += this.RebuildElements;
		}

		// Token: 0x06002423 RID: 9251 RVA: 0x0001A4F1 File Offset: 0x000186F1
		private void OnDisable()
		{
			UserProfile.onAvailableUserProfilesChanged -= this.RebuildElements;
		}

		// Token: 0x06002424 RID: 9252 RVA: 0x000AC168 File Offset: 0x000AA368
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

		// Token: 0x06002425 RID: 9253 RVA: 0x0001A504 File Offset: 0x00018704
		public ReadOnlyCollection<UserProfileListElementController> GetReadOnlyElementsList()
		{
			return new ReadOnlyCollection<UserProfileListElementController>(this.elementsList);
		}

		// Token: 0x1400005A RID: 90
		// (add) Token: 0x06002426 RID: 9254 RVA: 0x000AC2B4 File Offset: 0x000AA4B4
		// (remove) Token: 0x06002427 RID: 9255 RVA: 0x000AC2EC File Offset: 0x000AA4EC
		public event UserProfileListController.ProfileSelectedDelegate onProfileSelected;

		// Token: 0x06002428 RID: 9256 RVA: 0x0001A511 File Offset: 0x00018711
		public void SendProfileSelection(UserProfile userProfile)
		{
			if (this.onProfileSelected != null)
			{
				this.onProfileSelected(userProfile);
			}
		}

		// Token: 0x1400005B RID: 91
		// (add) Token: 0x06002429 RID: 9257 RVA: 0x000AC324 File Offset: 0x000AA524
		// (remove) Token: 0x0600242A RID: 9258 RVA: 0x000AC35C File Offset: 0x000AA55C
		public event Action onListRebuilt;

		// Token: 0x0400270F RID: 9999
		public GameObject elementPrefab;

		// Token: 0x04002710 RID: 10000
		public RectTransform contentRect;

		// Token: 0x04002711 RID: 10001
		[Tooltip("Whether or not \"default\" profile appears as a selectable option.")]
		public bool allowDefault = true;

		// Token: 0x04002712 RID: 10002
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002713 RID: 10003
		private readonly List<UserProfileListElementController> elementsList = new List<UserProfileListElementController>();

		// Token: 0x04002714 RID: 10004
		private int currentSelectionIndex;

		// Token: 0x0200064D RID: 1613
		// (Invoke) Token: 0x0600242D RID: 9261
		public delegate void ProfileSelectedDelegate(UserProfile userProfile);
	}
}
