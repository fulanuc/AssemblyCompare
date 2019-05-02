using System;
using System.Reflection;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200025F RID: 607
	[RequireComponent(typeof(AkGameObj))]
	public class AudioManager : MonoBehaviour
	{
		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000B4C RID: 2892 RVA: 0x00009095 File Offset: 0x00007295
		// (set) Token: 0x06000B4D RID: 2893 RVA: 0x0000909C File Offset: 0x0000729C
		public static AudioManager instance { get; private set; }

		// Token: 0x06000B4E RID: 2894 RVA: 0x000090A4 File Offset: 0x000072A4
		private void Awake()
		{
			AudioManager.instance = this;
			this.akGameObj = base.GetComponent<AkGameObj>();
		}

		// Token: 0x06000B4F RID: 2895 RVA: 0x0004BA7C File Offset: 0x00049C7C
		static AudioManager()
		{
			RoR2Application.onPauseStartGlobal = (Action)Delegate.Combine(RoR2Application.onPauseStartGlobal, new Action(delegate()
			{
				AkSoundEngine.PostEvent("Pause_All", null);
			}));
			RoR2Application.onPauseEndGlobal = (Action)Delegate.Combine(RoR2Application.onPauseEndGlobal, new Action(delegate()
			{
				AkSoundEngine.PostEvent("Unpause_All", null);
			}));
		}

		// Token: 0x04000F5A RID: 3930
		private AkGameObj akGameObj;

		// Token: 0x04000F5C RID: 3932
		private static AudioManager.VolumeConVar cvVolumeMaster = new AudioManager.VolumeConVar("volume_master", ConVarFlags.Archive | ConVarFlags.Engine, "100", "The master volume of the game audio, from 0 to 100.", "Volume_Master");

		// Token: 0x04000F5D RID: 3933
		private static AudioManager.VolumeConVar cvVolumeSfx = new AudioManager.VolumeConVar("volume_sfx", ConVarFlags.Archive | ConVarFlags.Engine, "100", "The volume of sound effects, from 0 to 100.", "Volume_SFX");

		// Token: 0x04000F5E RID: 3934
		private static AudioManager.VolumeConVar cvVolumeMsx = new AudioManager.VolumeConVar("volume_msx", ConVarFlags.Archive | ConVarFlags.Engine, "100", "The music volume, from 0 to 100.", "Volume_MSX");

		// Token: 0x04000F5F RID: 3935
		private static readonly FieldInfo akInitializerMsInstanceField = typeof(AkInitializer).GetField("ms_Instance", BindingFlags.Static | BindingFlags.NonPublic);

		// Token: 0x02000260 RID: 608
		private class VolumeConVar : BaseConVar
		{
			// Token: 0x06000B51 RID: 2897 RVA: 0x000090B8 File Offset: 0x000072B8
			public VolumeConVar(string name, ConVarFlags flags, string defaultValue, string helpText, string rtpcName) : base(name, flags, defaultValue, helpText)
			{
				this.rtpcName = rtpcName;
			}

			// Token: 0x06000B52 RID: 2898 RVA: 0x0004BB4C File Offset: 0x00049D4C
			public override void SetString(string newValue)
			{
				float value;
				if (AkSoundEngine.IsInitialized() && TextSerialization.TryParseInvariant(newValue, out value))
				{
					AkSoundEngine.SetRTPCValue(this.rtpcName, Mathf.Clamp(value, 0f, 100f));
				}
			}

			// Token: 0x06000B53 RID: 2899 RVA: 0x0004BB88 File Offset: 0x00049D88
			public override string GetString()
			{
				int num = 1;
				float value;
				AKRESULT rtpcvalue = AkSoundEngine.GetRTPCValue(this.rtpcName, null, 0u, out value, ref num);
				if (rtpcvalue == AKRESULT.AK_Success)
				{
					return TextSerialization.ToStringInvariant(value);
				}
				return "ERROR: " + rtpcvalue;
			}

			// Token: 0x04000F60 RID: 3936
			private readonly string rtpcName;
		}

		// Token: 0x02000261 RID: 609
		private class AudioFocusedOnlyConVar : BaseConVar
		{
			// Token: 0x06000B54 RID: 2900 RVA: 0x000090CD File Offset: 0x000072CD
			public AudioFocusedOnlyConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06000B55 RID: 2901 RVA: 0x0004BBC4 File Offset: 0x00049DC4
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					AkSoundEngineController.s_MuteOnFocusLost = (num != 0);
				}
			}

			// Token: 0x06000B56 RID: 2902 RVA: 0x000090DA File Offset: 0x000072DA
			public override string GetString()
			{
				if (!AkSoundEngineController.s_MuteOnFocusLost)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x04000F61 RID: 3937
			private static AudioManager.AudioFocusedOnlyConVar instance = new AudioManager.AudioFocusedOnlyConVar("audio_focused_only", ConVarFlags.Archive | ConVarFlags.Engine, null, "Whether or not audio should mute when focus is lost.");
		}

		// Token: 0x02000262 RID: 610
		private class WwiseLogEnabledConVar : BaseConVar
		{
			// Token: 0x06000B58 RID: 2904 RVA: 0x000090CD File Offset: 0x000072CD
			private WwiseLogEnabledConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06000B59 RID: 2905 RVA: 0x0004BBE4 File Offset: 0x00049DE4
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					AkInitializer akInitializer = AudioManager.akInitializerMsInstanceField.GetValue(null) as AkInitializer;
					if (akInitializer)
					{
						AkWwiseInitializationSettings initializationSettings = akInitializer.InitializationSettings;
						AkCallbackManager.InitializationSettings initializationSettings2 = (initializationSettings != null) ? initializationSettings.CallbackManagerInitializationSettings : null;
						if (initializationSettings2 != null)
						{
							initializationSettings2.IsLoggingEnabled = (num != 0);
							return;
						}
						Debug.Log("Cannot set value. callbackManagerInitializationSettings is null.");
					}
				}
			}

			// Token: 0x06000B5A RID: 2906 RVA: 0x0004BC40 File Offset: 0x00049E40
			public override string GetString()
			{
				AkInitializer akInitializer = AudioManager.akInitializerMsInstanceField.GetValue(null) as AkInitializer;
				if (akInitializer)
				{
					AkWwiseInitializationSettings initializationSettings = akInitializer.InitializationSettings;
					if (((initializationSettings != null) ? initializationSettings.CallbackManagerInitializationSettings : null) != null)
					{
						if (!akInitializer.InitializationSettings.CallbackManagerInitializationSettings.IsLoggingEnabled)
						{
							return "0";
						}
						return "1";
					}
					else
					{
						Debug.Log("Cannot retrieve value. callbackManagerInitializationSettings is null.");
					}
				}
				return "1";
			}

			// Token: 0x04000F62 RID: 3938
			private static AudioManager.WwiseLogEnabledConVar instance = new AudioManager.WwiseLogEnabledConVar("wwise_log_enabled", ConVarFlags.Archive | ConVarFlags.Engine, null, "Wwise logging. 0 = disabled 1 = enabled");
		}
	}
}
