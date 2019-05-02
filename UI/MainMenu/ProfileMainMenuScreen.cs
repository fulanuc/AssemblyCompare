using System;
using System.Collections.Generic;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI.MainMenu
{
	// Token: 0x02000662 RID: 1634
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ProfileMainMenuScreen : BaseMainMenuScreen
	{
		// Token: 0x06002473 RID: 9331 RVA: 0x000ACF70 File Offset: 0x000AB170
		private string GuessDefaultProfileName()
		{
			Client instance = Client.Instance;
			string text = (instance != null) ? instance.Username : null;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return "Nameless Survivor";
		}

		// Token: 0x06002474 RID: 9332 RVA: 0x0001A8C6 File Offset: 0x00018AC6
		protected void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.existingProfileListController.onProfileSelected += this.SetMainProfile;
			this.existingProfileListController.onListRebuilt += this.OnListRebuilt;
		}

		// Token: 0x06002475 RID: 9333 RVA: 0x000ACFA0 File Offset: 0x000AB1A0
		protected void OnEnable()
		{
			this.firstTimeConfiguration = true;
			List<string> availableProfileNames = UserProfile.GetAvailableProfileNames();
			for (int i = 0; i < availableProfileNames.Count; i++)
			{
				if (ProfileMainMenuScreen.IsProfileCustom(UserProfile.GetProfile(availableProfileNames[i])))
				{
					this.firstTimeConfiguration = false;
					break;
				}
			}
			if (this.firstTimeConfiguration)
			{
				Debug.Log("First-Time Profile Configuration");
				this.OpenCreateProfileMenu(true);
				return;
			}
			this.createProfilePanel.SetActive(false);
			this.selectProfilePanel.SetActive(true);
			this.OnListRebuilt();
			this.gotoSelectProfilePanelButtonContainer.SetActive(true);
		}

		// Token: 0x06002476 RID: 9334 RVA: 0x000AD02C File Offset: 0x000AB22C
		public void OpenCreateProfileMenu(bool firstTime)
		{
			this.selectProfilePanel.SetActive(false);
			this.createProfilePanel.SetActive(true);
			this.createProfileNameInputField.text = this.GuessDefaultProfileName();
			this.createProfileNameInputField.ActivateInputField();
			if (firstTime)
			{
				this.gotoSelectProfilePanelButtonContainer.SetActive(false);
			}
		}

		// Token: 0x06002477 RID: 9335 RVA: 0x0001A902 File Offset: 0x00018B02
		private void OnListRebuilt()
		{
			this.existingProfileListController.GetReadOnlyElementsList();
		}

		// Token: 0x06002478 RID: 9336 RVA: 0x000025F6 File Offset: 0x000007F6
		protected void OnDisable()
		{
		}

		// Token: 0x06002479 RID: 9337 RVA: 0x000AD07C File Offset: 0x000AB27C
		private void SetMainProfile(UserProfile profile)
		{
			LocalUserManager.SetLocalUsers(new LocalUserManager.LocalUserInitializationInfo[]
			{
				new LocalUserManager.LocalUserInitializationInfo
				{
					profile = profile
				}
			});
			this.myMainMenuController.desiredMenuScreen = this.myMainMenuController.titleMenuScreen;
		}

		// Token: 0x0600247A RID: 9338 RVA: 0x0001A910 File Offset: 0x00018B10
		private static bool IsProfileCustom(UserProfile profile)
		{
			return profile.fileName != "default";
		}

		// Token: 0x0600247B RID: 9339 RVA: 0x0001A922 File Offset: 0x00018B22
		private static bool IsNewProfileNameAcceptable(string newProfileName)
		{
			return UserProfile.GetProfile(newProfileName) == null && !(newProfileName == "");
		}

		// Token: 0x0600247C RID: 9340 RVA: 0x000AD0C4 File Offset: 0x000AB2C4
		public void OnAddProfilePressed()
		{
			if (this.eventSystemLocator.eventSystem.currentSelectedGameObject == this.createProfileNameInputField.gameObject && !Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				return;
			}
			string text = this.createProfileNameInputField.text;
			if (!ProfileMainMenuScreen.IsNewProfileNameAcceptable(text))
			{
				return;
			}
			this.createProfileNameInputField.text = "";
			UserProfile userProfile = UserProfile.CreateProfile(RoR2Application.cloudStorage, text);
			if (userProfile != null)
			{
				this.SetMainProfile(userProfile);
			}
		}

		// Token: 0x0600247D RID: 9341 RVA: 0x000AD144 File Offset: 0x000AB344
		protected void Update()
		{
			if (Input.GetKeyDown(KeyCode.Delete))
			{
				GameObject currentSelectedGameObject = MPEventSystemManager.combinedEventSystem.currentSelectedGameObject;
				if (currentSelectedGameObject)
				{
					UserProfileListElementController component = currentSelectedGameObject.GetComponent<UserProfileListElementController>();
					if (component)
					{
						if (component.userProfile == null)
						{
							Debug.LogError("!!!???");
							return;
						}
						SimpleDialogBox simpleDialogBox = SimpleDialogBox.Create(null);
						string consoleString = "user_profile_delete \"" + component.userProfile.fileName + "\"";
						simpleDialogBox.headerToken = new SimpleDialogBox.TokenParamsPair
						{
							token = "USER_PROFILE_DELETE_HEADER",
							formatParams = null
						};
						simpleDialogBox.descriptionToken = new SimpleDialogBox.TokenParamsPair
						{
							token = "USER_PROFILE_DELETE_DESCRIPTION",
							formatParams = new object[]
							{
								component.userProfile.name
							}
						};
						simpleDialogBox.AddCommandButton(consoleString, "USER_PROFILE_DELETE_YES", Array.Empty<object>());
						simpleDialogBox.AddCancelButton("USER_PROFILE_DELETE_NO", Array.Empty<object>());
					}
				}
			}
		}

		// Token: 0x04002770 RID: 10096
		public GameObject createProfilePanel;

		// Token: 0x04002771 RID: 10097
		public TMP_InputField createProfileNameInputField;

		// Token: 0x04002772 RID: 10098
		public MPButton submitProfileNameButton;

		// Token: 0x04002773 RID: 10099
		public GameObject gotoSelectProfilePanelButtonContainer;

		// Token: 0x04002774 RID: 10100
		public GameObject selectProfilePanel;

		// Token: 0x04002775 RID: 10101
		public MPButton gotoCreateProfilePanelButton;

		// Token: 0x04002776 RID: 10102
		public UserProfileListController existingProfileListController;

		// Token: 0x04002777 RID: 10103
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002778 RID: 10104
		private bool firstTimeConfiguration;

		// Token: 0x04002779 RID: 10105
		private const string defaultName = "Nameless Survivor";
	}
}
