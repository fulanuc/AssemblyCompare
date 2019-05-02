using System;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x020005BE RID: 1470
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class BaseSettingsControl : MonoBehaviour
	{
		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06002122 RID: 8482 RVA: 0x000182B8 File Offset: 0x000164B8
		public bool hasBeenChanged
		{
			get
			{
				return this.originalValue != null;
			}
		}

		// Token: 0x06002123 RID: 8483 RVA: 0x0009F978 File Offset: 0x0009DB78
		protected void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			if (this.nameLabel && !string.IsNullOrEmpty(this.nameToken))
			{
				this.nameLabel.token = this.nameToken;
			}
			if (this.settingSource == BaseSettingsControl.SettingSource.ConVar && this.GetConVar() == null)
			{
				Debug.LogErrorFormat("Null convar{0} detected in options", new object[]
				{
					this.settingName
				});
			}
		}

		// Token: 0x06002124 RID: 8484 RVA: 0x000182C3 File Offset: 0x000164C3
		protected void Start()
		{
			this.Initialize();
		}

		// Token: 0x06002125 RID: 8485 RVA: 0x000182CB File Offset: 0x000164CB
		protected void OnEnable()
		{
			this.UpdateControls();
		}

		// Token: 0x06002126 RID: 8486 RVA: 0x000025DA File Offset: 0x000007DA
		public virtual void Initialize()
		{
		}

		// Token: 0x06002127 RID: 8487 RVA: 0x000182D3 File Offset: 0x000164D3
		public void SubmitSetting(string newValue)
		{
			if (this.useConfirmationDialog)
			{
				this.SubmitSettingTemporary(newValue);
				return;
			}
			this.SubmitSettingInternal(newValue);
		}

		// Token: 0x06002128 RID: 8488 RVA: 0x0009F9E8 File Offset: 0x0009DBE8
		private void SubmitSettingInternal(string newValue)
		{
			if (this.originalValue == null)
			{
				this.originalValue = this.GetCurrentValue();
			}
			if (this.originalValue == newValue)
			{
				this.originalValue = null;
			}
			BaseSettingsControl.SettingSource settingSource = this.settingSource;
			if (settingSource != BaseSettingsControl.SettingSource.ConVar)
			{
				if (settingSource == BaseSettingsControl.SettingSource.UserProfilePref)
				{
					UserProfile currentUserProfile = this.GetCurrentUserProfile();
					if (currentUserProfile != null)
					{
						currentUserProfile.SetSaveFieldString(this.settingName, newValue);
					}
					UserProfile currentUserProfile2 = this.GetCurrentUserProfile();
					if (currentUserProfile2 != null)
					{
						currentUserProfile2.RequestSave(false);
					}
				}
			}
			else
			{
				BaseConVar conVar = this.GetConVar();
				if (conVar != null)
				{
					conVar.SetString(newValue);
				}
			}
			RoR2Application.onNextUpdate += this.UpdateControls;
		}

		// Token: 0x06002129 RID: 8489 RVA: 0x0009FA80 File Offset: 0x0009DC80
		private void SubmitSettingTemporary(string newValue)
		{
			string oldValue = this.GetCurrentValue();
			if (newValue == oldValue)
			{
				return;
			}
			this.SubmitSettingInternal(newValue);
			SimpleDialogBox dialogBox = SimpleDialogBox.Create(null);
			Action revertFunction = delegate()
			{
				if (dialogBox)
				{
					this.SubmitSettingInternal(oldValue);
				}
			};
			float num = 10f;
			float timeEnd = Time.unscaledTime + num;
			MPButton revertButton = dialogBox.AddActionButton(delegate
			{
				revertFunction();
			}, "OPTION_REVERT", Array.Empty<object>());
			dialogBox.AddActionButton(delegate
			{
			}, "OPTION_ACCEPT", Array.Empty<object>());
			Action updateText = null;
			updateText = delegate()
			{
				if (dialogBox)
				{
					int num2 = Mathf.FloorToInt(timeEnd - Time.unscaledTime);
					if (num2 < 0)
					{
						num2 = 0;
					}
					dialogBox.descriptionToken = new SimpleDialogBox.TokenParamsPair
					{
						token = "OPTION_AUTOREVERT_DIALOG_DESCRIPTION",
						formatParams = new object[]
						{
							num2
						}
					};
					if (num2 > 0)
					{
						RoR2Application.unscaledTimeTimers.CreateTimer(1f, updateText);
					}
				}
			};
			updateText();
			RoR2Application.unscaledTimeTimers.CreateTimer(num, delegate
			{
				if (revertButton)
				{
					revertButton.onClick.Invoke();
				}
			});
		}

		// Token: 0x0600212A RID: 8490 RVA: 0x0009FB88 File Offset: 0x0009DD88
		public string GetCurrentValue()
		{
			BaseSettingsControl.SettingSource settingSource = this.settingSource;
			if (settingSource != BaseSettingsControl.SettingSource.ConVar)
			{
				if (settingSource != BaseSettingsControl.SettingSource.UserProfilePref)
				{
					return "";
				}
				UserProfile currentUserProfile = this.GetCurrentUserProfile();
				return ((currentUserProfile != null) ? currentUserProfile.GetSaveFieldString(this.settingName) : null) ?? "";
			}
			else
			{
				BaseConVar conVar = this.GetConVar();
				if (conVar == null)
				{
					return null;
				}
				return conVar.GetString();
			}
		}

		// Token: 0x0600212B RID: 8491 RVA: 0x000182EC File Offset: 0x000164EC
		protected BaseConVar GetConVar()
		{
			return Console.instance.FindConVar(this.settingName);
		}

		// Token: 0x0600212C RID: 8492 RVA: 0x000182FE File Offset: 0x000164FE
		public UserProfile GetCurrentUserProfile()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			if (eventSystem == null)
			{
				return null;
			}
			LocalUser localUser = eventSystem.localUser;
			if (localUser == null)
			{
				return null;
			}
			return localUser.userProfile;
		}

		// Token: 0x0600212D RID: 8493 RVA: 0x00018321 File Offset: 0x00016521
		public void Revert()
		{
			if (this.hasBeenChanged)
			{
				this.SubmitSetting(this.originalValue);
				this.originalValue = null;
			}
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x0600212E RID: 8494 RVA: 0x0001833E File Offset: 0x0001653E
		// (set) Token: 0x0600212F RID: 8495 RVA: 0x00018346 File Offset: 0x00016546
		private protected bool inUpdateControls { protected get; private set; }

		// Token: 0x06002130 RID: 8496 RVA: 0x0001834F File Offset: 0x0001654F
		protected void UpdateControls()
		{
			if (!this)
			{
				return;
			}
			if (this.inUpdateControls)
			{
				return;
			}
			this.inUpdateControls = true;
			this.OnUpdateControls();
			this.inUpdateControls = false;
		}

		// Token: 0x06002131 RID: 8497 RVA: 0x000025DA File Offset: 0x000007DA
		protected virtual void OnUpdateControls()
		{
		}

		// Token: 0x04002365 RID: 9061
		public BaseSettingsControl.SettingSource settingSource;

		// Token: 0x04002366 RID: 9062
		[FormerlySerializedAs("convarName")]
		public string settingName;

		// Token: 0x04002367 RID: 9063
		public string nameToken;

		// Token: 0x04002368 RID: 9064
		public LanguageTextMeshController nameLabel;

		// Token: 0x04002369 RID: 9065
		[Tooltip("Whether or not this setting requires a confirmation dialog. This is mainly for video options.")]
		public bool useConfirmationDialog;

		// Token: 0x0400236A RID: 9066
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x0400236B RID: 9067
		private string originalValue;

		// Token: 0x020005BF RID: 1471
		public enum SettingSource
		{
			// Token: 0x0400236E RID: 9070
			ConVar,
			// Token: 0x0400236F RID: 9071
			UserProfilePref
		}
	}
}
