using System;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x020005AC RID: 1452
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class BaseSettingsControl : MonoBehaviour
	{
		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06002091 RID: 8337 RVA: 0x00017BBE File Offset: 0x00015DBE
		public bool hasBeenChanged
		{
			get
			{
				return this.originalValue != null;
			}
		}

		// Token: 0x06002092 RID: 8338 RVA: 0x0009E3A4 File Offset: 0x0009C5A4
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

		// Token: 0x06002093 RID: 8339 RVA: 0x00017BC9 File Offset: 0x00015DC9
		protected void Start()
		{
			this.Initialize();
		}

		// Token: 0x06002094 RID: 8340 RVA: 0x00017BD1 File Offset: 0x00015DD1
		protected void OnEnable()
		{
			this.UpdateControls();
		}

		// Token: 0x06002095 RID: 8341 RVA: 0x000025F6 File Offset: 0x000007F6
		public virtual void Initialize()
		{
		}

		// Token: 0x06002096 RID: 8342 RVA: 0x00017BD9 File Offset: 0x00015DD9
		public void SubmitSetting(string newValue)
		{
			if (this.useConfirmationDialog)
			{
				this.SubmitSettingTemporary(newValue);
				return;
			}
			this.SubmitSettingInternal(newValue);
		}

		// Token: 0x06002097 RID: 8343 RVA: 0x0009E414 File Offset: 0x0009C614
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

		// Token: 0x06002098 RID: 8344 RVA: 0x0009E4AC File Offset: 0x0009C6AC
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

		// Token: 0x06002099 RID: 8345 RVA: 0x0009E5B4 File Offset: 0x0009C7B4
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

		// Token: 0x0600209A RID: 8346 RVA: 0x00017BF2 File Offset: 0x00015DF2
		protected BaseConVar GetConVar()
		{
			return Console.instance.FindConVar(this.settingName);
		}

		// Token: 0x0600209B RID: 8347 RVA: 0x00017C04 File Offset: 0x00015E04
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

		// Token: 0x0600209C RID: 8348 RVA: 0x00017C27 File Offset: 0x00015E27
		public void Revert()
		{
			if (this.hasBeenChanged)
			{
				this.SubmitSetting(this.originalValue);
				this.originalValue = null;
			}
		}

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x0600209D RID: 8349 RVA: 0x00017C44 File Offset: 0x00015E44
		// (set) Token: 0x0600209E RID: 8350 RVA: 0x00017C4C File Offset: 0x00015E4C
		private protected bool inUpdateControls { protected get; private set; }

		// Token: 0x0600209F RID: 8351 RVA: 0x00017C55 File Offset: 0x00015E55
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

		// Token: 0x060020A0 RID: 8352 RVA: 0x000025F6 File Offset: 0x000007F6
		protected virtual void OnUpdateControls()
		{
		}

		// Token: 0x04002311 RID: 8977
		public BaseSettingsControl.SettingSource settingSource;

		// Token: 0x04002312 RID: 8978
		[FormerlySerializedAs("convarName")]
		public string settingName;

		// Token: 0x04002313 RID: 8979
		public string nameToken;

		// Token: 0x04002314 RID: 8980
		public LanguageTextMeshController nameLabel;

		// Token: 0x04002315 RID: 8981
		[Tooltip("Whether or not this setting requires a confirmation dialog. This is mainly for video options.")]
		public bool useConfirmationDialog;

		// Token: 0x04002316 RID: 8982
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002317 RID: 8983
		private string originalValue;

		// Token: 0x020005AD RID: 1453
		public enum SettingSource
		{
			// Token: 0x0400231A RID: 8986
			ConVar,
			// Token: 0x0400231B RID: 8987
			UserProfilePref
		}
	}
}
