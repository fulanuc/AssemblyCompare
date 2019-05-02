using System;
using System.Collections.Generic;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI.MainMenu
{
	// Token: 0x02000674 RID: 1652
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ProfileMainMenuScreen : BaseMainMenuScreen
	{
		// Token: 0x0600250A RID: 9482 RVA: 0x000AE660 File Offset: 0x000AC860
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

		// Token: 0x0600250B RID: 9483 RVA: 0x0001AFF9 File Offset: 0x000191F9
		protected void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.existingProfileListController.onProfileSelected += this.SetMainProfile;
			this.existingProfileListController.onListRebuilt += this.OnListRebuilt;
		}

		// Token: 0x0600250C RID: 9484 RVA: 0x000AE690 File Offset: 0x000AC890
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

		// Token: 0x0600250D RID: 9485 RVA: 0x000AE71C File Offset: 0x000AC91C
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

		// Token: 0x0600250E RID: 9486 RVA: 0x0001B035 File Offset: 0x00019235
		private void OnListRebuilt()
		{
			this.existingProfileListController.GetReadOnlyElementsList();
		}

		// Token: 0x0600250F RID: 9487 RVA: 0x000025DA File Offset: 0x000007DA
		protected void OnDisable()
		{
		}

		// Token: 0x06002510 RID: 9488 RVA: 0x000AE76C File Offset: 0x000AC96C
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

		// Token: 0x06002511 RID: 9489 RVA: 0x0001B043 File Offset: 0x00019243
		private static bool IsProfileCustom(UserProfile profile)
		{
			return profile.fileName != "default";
		}

		// Token: 0x06002512 RID: 9490 RVA: 0x0001B055 File Offset: 0x00019255
		private static bool IsNewProfileNameAcceptable(string newProfileName)
		{
			return UserProfile.GetProfile(newProfileName) == null && !(newProfileName == "");
		}

		// Token: 0x06002513 RID: 9491 RVA: 0x000AE7B4 File Offset: 0x000AC9B4
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

		// Token: 0x06002514 RID: 9492 RVA: 0x000AE834 File Offset: 0x000ACA34
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

		// Token: 0x040027CC RID: 10188
		public GameObject createProfilePanel;

		// Token: 0x040027CD RID: 10189
		public TMP_InputField createProfileNameInputField;

		// Token: 0x040027CE RID: 10190
		public MPButton submitProfileNameButton;

		// Token: 0x040027CF RID: 10191
		public GameObject gotoSelectProfilePanelButtonContainer;

		// Token: 0x040027D0 RID: 10192
		public GameObject selectProfilePanel;

		// Token: 0x040027D1 RID: 10193
		public MPButton gotoCreateProfilePanelButton;

		// Token: 0x040027D2 RID: 10194
		public UserProfileListController existingProfileListController;

		// Token: 0x040027D3 RID: 10195
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040027D4 RID: 10196
		private bool firstTimeConfiguration;

		// Token: 0x040027D5 RID: 10197
		private const string defaultName = "Nameless Survivor";
	}
}
