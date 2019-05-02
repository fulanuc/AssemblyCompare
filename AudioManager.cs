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
		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000B49 RID: 2889 RVA: 0x00009070 File Offset: 0x00007270
		// (set) Token: 0x06000B4A RID: 2890 RVA: 0x00009077 File Offset: 0x00007277
		public static AudioManager instance { get; private set; }

		// Token: 0x06000B4B RID: 2891 RVA: 0x0000907F File Offset: 0x0000727F
		private void Awake()
		{
			AudioManager.instance = this;
			this.akGameObj = base.GetComponent<AkGameObj>();
		}

		// Token: 0x06000B4C RID: 2892 RVA: 0x0004B870 File Offset: 0x00049A70
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

		// Token: 0x04000F54 RID: 3924
		private AkGameObj akGameObj;

		// Token: 0x04000F56 RID: 3926
		private static AudioManager.VolumeConVar cvVolumeMaster = new AudioManager.VolumeConVar("volume_master", ConVarFlags.Archive | ConVarFlags.Engine, "100", "The master volume of the game audio, from 0 to 100.", "Volume_Master");

		// Token: 0x04000F57 RID: 3927
		private static AudioManager.VolumeConVar cvVolumeSfx = new AudioManager.VolumeConVar("volume_sfx", ConVarFlags.Archive | ConVarFlags.Engine, "100", "The volume of sound effects, from 0 to 100.", "Volume_SFX");

		// Token: 0x04000F58 RID: 3928
		private static AudioManager.VolumeConVar cvVolumeMsx = new AudioManager.VolumeConVar("volume_msx", ConVarFlags.Archive | ConVarFlags.Engine, "100", "The music volume, from 0 to 100.", "Volume_MSX");

		// Token: 0x04000F59 RID: 3929
		private static readonly FieldInfo akInitializerMsInstanceField = typeof(AkInitializer).GetField("ms_Instance", BindingFlags.Static | BindingFlags.NonPublic);

		// Token: 0x02000260 RID: 608
		private class VolumeConVar : BaseConVar
		{
			// Token: 0x06000B4E RID: 2894 RVA: 0x00009093 File Offset: 0x00007293
			public VolumeConVar(string name, ConVarFlags flags, string defaultValue, string helpText, string rtpcName) : base(name, flags, defaultValue, helpText)
			{
				this.rtpcName = rtpcName;
			}

			// Token: 0x06000B4F RID: 2895 RVA: 0x0004B940 File Offset: 0x00049B40
			public override void SetString(string newValue)
			{
				float value;
				if (AkSoundEngine.IsInitialized() && TextSerialization.TryParseInvariant(newValue, out value))
				{
					AkSoundEngine.SetRTPCValue(this.rtpcName, Mathf.Clamp(value, 0f, 100f));
				}
			}

			// Token: 0x06000B50 RID: 2896 RVA: 0x0004B97C File Offset: 0x00049B7C
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

			// Token: 0x04000F5A RID: 3930
			private readonly string rtpcName;
		}

		// Token: 0x02000261 RID: 609
		private class AudioFocusedOnlyConVar : BaseConVar
		{
			// Token: 0x06000B51 RID: 2897 RVA: 0x000090A8 File Offset: 0x000072A8
			public AudioFocusedOnlyConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06000B52 RID: 2898 RVA: 0x0004B9B8 File Offset: 0x00049BB8
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					AkSoundEngineController.s_MuteOnFocusLost = (num != 0);
				}
			}

			// Token: 0x06000B53 RID: 2899 RVA: 0x000090B5 File Offset: 0x000072B5
			public override string GetString()
			{
				if (!AkSoundEngineController.s_MuteOnFocusLost)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x04000F5B RID: 3931
			private static AudioManager.AudioFocusedOnlyConVar instance = new AudioManager.AudioFocusedOnlyConVar("audio_focused_only", ConVarFlags.Archive | ConVarFlags.Engine, null, "Whether or not audio should mute when focus is lost.");
		}

		// Token: 0x02000262 RID: 610
		private class WwiseLogEnabledConVar : BaseConVar
		{
			// Token: 0x06000B55 RID: 2901 RVA: 0x000090A8 File Offset: 0x000072A8
			private WwiseLogEnabledConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06000B56 RID: 2902 RVA: 0x0004B9D8 File Offset: 0x00049BD8
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

			// Token: 0x06000B57 RID: 2903 RVA: 0x0004BA34 File Offset: 0x00049C34
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

			// Token: 0x04000F5C RID: 3932
			private static AudioManager.WwiseLogEnabledConVar instance = new AudioManager.WwiseLogEnabledConVar("wwise_log_enabled", ConVarFlags.Archive | ConVarFlags.Engine, null, "Wwise logging. 0 = disabled 1 = enabled");
		}
	}
}
