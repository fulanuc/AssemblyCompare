using System;
using System.Globalization;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace RoR2
{
	// Token: 0x02000494 RID: 1172
	public static class SettingsConVars
	{
		// Token: 0x02000495 RID: 1173
		private class VSyncCountConVar : BaseConVar
		{
			// Token: 0x06001A6C RID: 6764 RVA: 0x000090CD File Offset: 0x000072CD
			private VSyncCountConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A6D RID: 6765 RVA: 0x000864B0 File Offset: 0x000846B0
			public override void SetString(string newValue)
			{
				int vSyncCount;
				if (TextSerialization.TryParseInvariant(newValue, out vSyncCount))
				{
					QualitySettings.vSyncCount = vSyncCount;
				}
			}

			// Token: 0x06001A6E RID: 6766 RVA: 0x00013934 File Offset: 0x00011B34
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.vSyncCount);
			}

			// Token: 0x04001D78 RID: 7544
			private static SettingsConVars.VSyncCountConVar instance = new SettingsConVars.VSyncCountConVar("vsync_count", ConVarFlags.Archive | ConVarFlags.Engine, null, "Vsync count.");
		}

		// Token: 0x02000496 RID: 1174
		private class WindowModeConVar : BaseConVar
		{
			// Token: 0x06001A70 RID: 6768 RVA: 0x000090CD File Offset: 0x000072CD
			private WindowModeConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A71 RID: 6769 RVA: 0x000864D0 File Offset: 0x000846D0
			public override void SetString(string newValue)
			{
				try
				{
					switch ((SettingsConVars.WindowModeConVar.WindowMode)Enum.Parse(typeof(SettingsConVars.WindowModeConVar.WindowMode), newValue, true))
					{
					case SettingsConVars.WindowModeConVar.WindowMode.Fullscreen:
						Screen.fullScreen = true;
						break;
					case SettingsConVars.WindowModeConVar.WindowMode.Window:
						Screen.fullScreen = false;
						break;
					}
				}
				catch (ArgumentException)
				{
					Console.ShowHelpText(this.name);
				}
			}

			// Token: 0x06001A72 RID: 6770 RVA: 0x00013959 File Offset: 0x00011B59
			public override string GetString()
			{
				if (!Screen.fullScreen)
				{
					return "Window";
				}
				return "Fullscreen";
			}

			// Token: 0x04001D79 RID: 7545
			private static SettingsConVars.WindowModeConVar instance = new SettingsConVars.WindowModeConVar("window_mode", ConVarFlags.Archive | ConVarFlags.Engine, null, "The window mode. Choices are Fullscreen and Window.");

			// Token: 0x02000497 RID: 1175
			private enum WindowMode
			{
				// Token: 0x04001D7B RID: 7547
				Fullscreen,
				// Token: 0x04001D7C RID: 7548
				Window,
				// Token: 0x04001D7D RID: 7549
				WindowNoBorder
			}
		}

		// Token: 0x02000498 RID: 1176
		private class ResolutionConVar : BaseConVar
		{
			// Token: 0x06001A74 RID: 6772 RVA: 0x000090CD File Offset: 0x000072CD
			private ResolutionConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A75 RID: 6773 RVA: 0x00086538 File Offset: 0x00084738
			public override void SetString(string newValue)
			{
				string[] array = newValue.Split(new char[]
				{
					'x'
				});
				int width;
				if (array.Length < 1 || !TextSerialization.TryParseInvariant(array[0], out width))
				{
					throw new ConCommandException("Invalid resolution format. No width integer. Example: \"1920x1080x60\".");
				}
				int height;
				if (array.Length < 2 || !TextSerialization.TryParseInvariant(array[1], out height))
				{
					throw new ConCommandException("Invalid resolution format. No height integer. Example: \"1920x1080x60\".");
				}
				int preferredRefreshRate;
				if (array.Length < 3 || !TextSerialization.TryParseInvariant(array[2], out preferredRefreshRate))
				{
					throw new ConCommandException("Invalid resolution format. No refresh rate integer. Example: \"1920x1080x60\".");
				}
				Screen.SetResolution(width, height, Screen.fullScreen, preferredRefreshRate);
			}

			// Token: 0x06001A76 RID: 6774 RVA: 0x000865BC File Offset: 0x000847BC
			public override string GetString()
			{
				Resolution currentResolution = Screen.currentResolution;
				return string.Format(CultureInfo.InvariantCulture, "{0}x{1}x{2}", currentResolution.width, currentResolution.height, currentResolution.refreshRate);
			}

			// Token: 0x06001A77 RID: 6775 RVA: 0x00086604 File Offset: 0x00084804
			[ConCommand(commandName = "resolution_list", flags = ConVarFlags.None, helpText = "Prints a list of all possible resolutions for the current display.")]
			private static void CCResolutionList(ConCommandArgs args)
			{
				Resolution[] resolutions = Screen.resolutions;
				string[] array = new string[resolutions.Length];
				for (int i = 0; i < resolutions.Length; i++)
				{
					Resolution resolution = resolutions[i];
					array[i] = string.Format("{0}x{1}x{2}", resolution.width, resolution.height, resolution.refreshRate);
				}
				Debug.Log(string.Join("\n", array));
			}

			// Token: 0x04001D7E RID: 7550
			private static SettingsConVars.ResolutionConVar instance = new SettingsConVars.ResolutionConVar("resolution", ConVarFlags.Archive | ConVarFlags.Engine, null, "The resolution of the game window. Format example: 1920x1080x60");
		}

		// Token: 0x02000499 RID: 1177
		private class FpsMaxConVar : BaseConVar
		{
			// Token: 0x06001A79 RID: 6777 RVA: 0x000090CD File Offset: 0x000072CD
			private FpsMaxConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A7A RID: 6778 RVA: 0x00086678 File Offset: 0x00084878
			public override void SetString(string newValue)
			{
				int targetFrameRate;
				if (TextSerialization.TryParseInvariant(newValue, out targetFrameRate))
				{
					Application.targetFrameRate = targetFrameRate;
				}
			}

			// Token: 0x06001A7B RID: 6779 RVA: 0x0001399F File Offset: 0x00011B9F
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(Application.targetFrameRate);
			}

			// Token: 0x04001D7F RID: 7551
			private static SettingsConVars.FpsMaxConVar instance = new SettingsConVars.FpsMaxConVar("fps_max", ConVarFlags.Archive | ConVarFlags.Engine, null, "Maximum FPS. -1 is unlimited.");
		}

		// Token: 0x0200049A RID: 1178
		private class ShadowsConVar : BaseConVar
		{
			// Token: 0x06001A7D RID: 6781 RVA: 0x000090CD File Offset: 0x000072CD
			private ShadowsConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A7E RID: 6782 RVA: 0x00086698 File Offset: 0x00084898
			public override void SetString(string newValue)
			{
				try
				{
					QualitySettings.shadows = (ShadowQuality)Enum.Parse(typeof(ShadowQuality), newValue, true);
				}
				catch (ArgumentException)
				{
					Console.ShowHelpText(this.name);
				}
			}

			// Token: 0x06001A7F RID: 6783 RVA: 0x000866E0 File Offset: 0x000848E0
			public override string GetString()
			{
				return QualitySettings.shadows.ToString();
			}

			// Token: 0x04001D80 RID: 7552
			private static SettingsConVars.ShadowsConVar instance = new SettingsConVars.ShadowsConVar("r_shadows", ConVarFlags.Archive | ConVarFlags.Engine, null, "Shadow quality. Can be \"All\" \"HardOnly\" or \"Disable\"");
		}

		// Token: 0x0200049B RID: 1179
		private class SoftParticlesConVar : BaseConVar
		{
			// Token: 0x06001A81 RID: 6785 RVA: 0x000090CD File Offset: 0x000072CD
			private SoftParticlesConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A82 RID: 6786 RVA: 0x00086700 File Offset: 0x00084900
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					QualitySettings.softParticles = (num != 0);
				}
			}

			// Token: 0x06001A83 RID: 6787 RVA: 0x000139DD File Offset: 0x00011BDD
			public override string GetString()
			{
				if (!QualitySettings.softParticles)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x04001D81 RID: 7553
			private static SettingsConVars.SoftParticlesConVar instance = new SettingsConVars.SoftParticlesConVar("r_softparticles", ConVarFlags.Archive | ConVarFlags.Engine, null, "Whether or not soft particles are enabled.");
		}

		// Token: 0x0200049C RID: 1180
		private class FoliageWindConVar : BaseConVar
		{
			// Token: 0x06001A85 RID: 6789 RVA: 0x000090CD File Offset: 0x000072CD
			private FoliageWindConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A86 RID: 6790 RVA: 0x00086720 File Offset: 0x00084920
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					if (num >= 1)
					{
						Shader.EnableKeyword("ENABLE_WIND");
						return;
					}
					Shader.DisableKeyword("ENABLE_WIND");
				}
			}

			// Token: 0x06001A87 RID: 6791 RVA: 0x00013A0A File Offset: 0x00011C0A
			public override string GetString()
			{
				if (!Shader.IsKeywordEnabled("ENABLE_WIND"))
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x04001D82 RID: 7554
			private static SettingsConVars.FoliageWindConVar instance = new SettingsConVars.FoliageWindConVar("r_foliagewind", ConVarFlags.Archive, "1", "Whether or not foliage has wind.");
		}

		// Token: 0x0200049D RID: 1181
		private class LodBiasConVar : BaseConVar
		{
			// Token: 0x06001A89 RID: 6793 RVA: 0x000090CD File Offset: 0x000072CD
			private LodBiasConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A8A RID: 6794 RVA: 0x00086750 File Offset: 0x00084950
			public override void SetString(string newValue)
			{
				float lodBias;
				if (TextSerialization.TryParseInvariant(newValue, out lodBias))
				{
					QualitySettings.lodBias = lodBias;
				}
			}

			// Token: 0x06001A8B RID: 6795 RVA: 0x00013A3F File Offset: 0x00011C3F
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.lodBias);
			}

			// Token: 0x04001D83 RID: 7555
			private static SettingsConVars.LodBiasConVar instance = new SettingsConVars.LodBiasConVar("r_lod_bias", ConVarFlags.Archive | ConVarFlags.Engine, null, "LOD bias.");
		}

		// Token: 0x0200049E RID: 1182
		private class MaximumLodConVar : BaseConVar
		{
			// Token: 0x06001A8D RID: 6797 RVA: 0x000090CD File Offset: 0x000072CD
			private MaximumLodConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A8E RID: 6798 RVA: 0x00086770 File Offset: 0x00084970
			public override void SetString(string newValue)
			{
				int maximumLODLevel;
				if (TextSerialization.TryParseInvariant(newValue, out maximumLODLevel))
				{
					QualitySettings.maximumLODLevel = maximumLODLevel;
				}
			}

			// Token: 0x06001A8F RID: 6799 RVA: 0x00013A64 File Offset: 0x00011C64
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.maximumLODLevel);
			}

			// Token: 0x04001D84 RID: 7556
			private static SettingsConVars.MaximumLodConVar instance = new SettingsConVars.MaximumLodConVar("r_lod_max", ConVarFlags.Archive | ConVarFlags.Engine, null, "The maximum allowed LOD level.");
		}

		// Token: 0x0200049F RID: 1183
		private class MasterTextureLimitConVar : BaseConVar
		{
			// Token: 0x06001A91 RID: 6801 RVA: 0x000090CD File Offset: 0x000072CD
			private MasterTextureLimitConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A92 RID: 6802 RVA: 0x00086790 File Offset: 0x00084990
			public override void SetString(string newValue)
			{
				int masterTextureLimit;
				if (TextSerialization.TryParseInvariant(newValue, out masterTextureLimit))
				{
					QualitySettings.masterTextureLimit = masterTextureLimit;
				}
			}

			// Token: 0x06001A93 RID: 6803 RVA: 0x00013A89 File Offset: 0x00011C89
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.masterTextureLimit);
			}

			// Token: 0x04001D85 RID: 7557
			private static SettingsConVars.MasterTextureLimitConVar instance = new SettingsConVars.MasterTextureLimitConVar("master_texture_limit", ConVarFlags.Archive | ConVarFlags.Engine, null, "Reduction in texture quality. 0 is highest quality textures, 1 is half, 2 is quarter, etc.");
		}

		// Token: 0x020004A0 RID: 1184
		private class AnisotropicFilteringConVar : BaseConVar
		{
			// Token: 0x06001A95 RID: 6805 RVA: 0x000090CD File Offset: 0x000072CD
			private AnisotropicFilteringConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A96 RID: 6806 RVA: 0x000867B0 File Offset: 0x000849B0
			public override void SetString(string newValue)
			{
				try
				{
					QualitySettings.anisotropicFiltering = (AnisotropicFiltering)Enum.Parse(typeof(AnisotropicFiltering), newValue, true);
				}
				catch (ArgumentException)
				{
					Console.ShowHelpText(this.name);
				}
			}

			// Token: 0x06001A97 RID: 6807 RVA: 0x000867F8 File Offset: 0x000849F8
			public override string GetString()
			{
				return QualitySettings.anisotropicFiltering.ToString();
			}

			// Token: 0x04001D86 RID: 7558
			private static SettingsConVars.AnisotropicFilteringConVar instance = new SettingsConVars.AnisotropicFilteringConVar("anisotropic_filtering", ConVarFlags.Archive, "Disable", "The anisotropic filtering mode. Can be \"Disable\", \"Enable\" or \"ForceEnable\".");
		}

		// Token: 0x020004A1 RID: 1185
		private class ShadowResolutionConVar : BaseConVar
		{
			// Token: 0x06001A99 RID: 6809 RVA: 0x000090CD File Offset: 0x000072CD
			private ShadowResolutionConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A9A RID: 6810 RVA: 0x00086818 File Offset: 0x00084A18
			public override void SetString(string newValue)
			{
				try
				{
					QualitySettings.shadowResolution = (ShadowResolution)Enum.Parse(typeof(ShadowResolution), newValue, true);
				}
				catch (ArgumentException)
				{
					Console.ShowHelpText(this.name);
				}
			}

			// Token: 0x06001A9B RID: 6811 RVA: 0x00086860 File Offset: 0x00084A60
			public override string GetString()
			{
				return QualitySettings.shadowResolution.ToString();
			}

			// Token: 0x04001D87 RID: 7559
			private static SettingsConVars.ShadowResolutionConVar instance = new SettingsConVars.ShadowResolutionConVar("shadow_resolution", ConVarFlags.Archive | ConVarFlags.Engine, "Medium", "Default shadow resolution. Can be \"Low\", \"Medium\", \"High\" or \"VeryHigh\".");
		}

		// Token: 0x020004A2 RID: 1186
		private class ShadowCascadesConVar : BaseConVar
		{
			// Token: 0x06001A9D RID: 6813 RVA: 0x000090CD File Offset: 0x000072CD
			private ShadowCascadesConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A9E RID: 6814 RVA: 0x00086880 File Offset: 0x00084A80
			public override void SetString(string newValue)
			{
				int shadowCascades;
				if (TextSerialization.TryParseInvariant(newValue, out shadowCascades))
				{
					QualitySettings.shadowCascades = shadowCascades;
				}
			}

			// Token: 0x06001A9F RID: 6815 RVA: 0x00013AE7 File Offset: 0x00011CE7
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.shadowCascades);
			}

			// Token: 0x04001D88 RID: 7560
			private static SettingsConVars.ShadowCascadesConVar instance = new SettingsConVars.ShadowCascadesConVar("shadow_cascades", ConVarFlags.Archive | ConVarFlags.Engine, null, "The number of cascades to use for directional light shadows. low=0 high=4");
		}

		// Token: 0x020004A3 RID: 1187
		private class ShadowDistanceConvar : BaseConVar
		{
			// Token: 0x06001AA1 RID: 6817 RVA: 0x000090CD File Offset: 0x000072CD
			private ShadowDistanceConvar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001AA2 RID: 6818 RVA: 0x000868A0 File Offset: 0x00084AA0
			public override void SetString(string newValue)
			{
				float shadowDistance;
				if (TextSerialization.TryParseInvariant(newValue, out shadowDistance))
				{
					QualitySettings.shadowDistance = shadowDistance;
				}
			}

			// Token: 0x06001AA3 RID: 6819 RVA: 0x00013B0C File Offset: 0x00011D0C
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.shadowDistance);
			}

			// Token: 0x04001D89 RID: 7561
			private static SettingsConVars.ShadowDistanceConvar instance = new SettingsConVars.ShadowDistanceConvar("shadow_distance", ConVarFlags.Archive, "200", "The distance in meters to draw shadows.");
		}

		// Token: 0x020004A4 RID: 1188
		private class PpMotionBlurConVar : BaseConVar
		{
			// Token: 0x06001AA5 RID: 6821 RVA: 0x00013B34 File Offset: 0x00011D34
			private PpMotionBlurConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<MotionBlur>(out SettingsConVars.PpMotionBlurConVar.settings);
			}

			// Token: 0x06001AA6 RID: 6822 RVA: 0x000868C0 File Offset: 0x00084AC0
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num) && SettingsConVars.PpMotionBlurConVar.settings)
				{
					SettingsConVars.PpMotionBlurConVar.settings.active = (num == 0);
				}
			}

			// Token: 0x06001AA7 RID: 6823 RVA: 0x00013B5B File Offset: 0x00011D5B
			public override string GetString()
			{
				if (!SettingsConVars.PpMotionBlurConVar.settings)
				{
					return "1";
				}
				if (!SettingsConVars.PpMotionBlurConVar.settings.active)
				{
					return "1";
				}
				return "0";
			}

			// Token: 0x04001D8A RID: 7562
			private static MotionBlur settings;

			// Token: 0x04001D8B RID: 7563
			private static SettingsConVars.PpMotionBlurConVar instance = new SettingsConVars.PpMotionBlurConVar("pp_motionblur", ConVarFlags.Archive, "0", "Motion blur. 0 = disabled 1 = enabled");
		}

		// Token: 0x020004A5 RID: 1189
		private class PpSobelOutlineConVar : BaseConVar
		{
			// Token: 0x06001AA9 RID: 6825 RVA: 0x00013BA2 File Offset: 0x00011DA2
			private PpSobelOutlineConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<SobelOutline>(out SettingsConVars.PpSobelOutlineConVar.sobelOutlineSettings);
			}

			// Token: 0x06001AAA RID: 6826 RVA: 0x000868F4 File Offset: 0x00084AF4
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num) && SettingsConVars.PpSobelOutlineConVar.sobelOutlineSettings)
				{
					SettingsConVars.PpSobelOutlineConVar.sobelOutlineSettings.active = (num == 0);
				}
			}

			// Token: 0x06001AAB RID: 6827 RVA: 0x00013BC9 File Offset: 0x00011DC9
			public override string GetString()
			{
				if (!SettingsConVars.PpSobelOutlineConVar.sobelOutlineSettings)
				{
					return "1";
				}
				if (!SettingsConVars.PpSobelOutlineConVar.sobelOutlineSettings.active)
				{
					return "1";
				}
				return "0";
			}

			// Token: 0x04001D8C RID: 7564
			private static SobelOutline sobelOutlineSettings;

			// Token: 0x04001D8D RID: 7565
			private static SettingsConVars.PpSobelOutlineConVar instance = new SettingsConVars.PpSobelOutlineConVar("pp_sobel_outline", ConVarFlags.Archive, "1", "Whether or not to use the sobel rim light effect.");
		}

		// Token: 0x020004A6 RID: 1190
		private class PpBloomConVar : BaseConVar
		{
			// Token: 0x06001AAD RID: 6829 RVA: 0x00086928 File Offset: 0x00084B28
			private PpBloomConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				bool flag = RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<Bloom>(out SettingsConVars.PpBloomConVar.settings);
				Debug.LogFormat("Bloom: {0}", new object[]
				{
					flag
				});
			}

			// Token: 0x06001AAE RID: 6830 RVA: 0x00086974 File Offset: 0x00084B74
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num) && SettingsConVars.PpBloomConVar.settings)
				{
					SettingsConVars.PpBloomConVar.settings.active = (num == 0);
				}
			}

			// Token: 0x06001AAF RID: 6831 RVA: 0x00013C10 File Offset: 0x00011E10
			public override string GetString()
			{
				if (!SettingsConVars.PpBloomConVar.settings)
				{
					return "1";
				}
				if (!SettingsConVars.PpBloomConVar.settings.active)
				{
					return "1";
				}
				return "0";
			}

			// Token: 0x04001D8E RID: 7566
			private static Bloom settings;

			// Token: 0x04001D8F RID: 7567
			private static SettingsConVars.PpBloomConVar instance = new SettingsConVars.PpBloomConVar("pp_bloom", ConVarFlags.Archive | ConVarFlags.Engine, null, "Bloom postprocessing. 0 = disabled 1 = enabled");
		}

		// Token: 0x020004A7 RID: 1191
		private class PpAOConVar : BaseConVar
		{
			// Token: 0x06001AB1 RID: 6833 RVA: 0x00013C54 File Offset: 0x00011E54
			private PpAOConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<AmbientOcclusion>(out SettingsConVars.PpAOConVar.settings);
			}

			// Token: 0x06001AB2 RID: 6834 RVA: 0x000869A8 File Offset: 0x00084BA8
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num) && SettingsConVars.PpAOConVar.settings)
				{
					SettingsConVars.PpAOConVar.settings.active = (num == 0);
				}
			}

			// Token: 0x06001AB3 RID: 6835 RVA: 0x00013C7B File Offset: 0x00011E7B
			public override string GetString()
			{
				if (!SettingsConVars.PpAOConVar.settings)
				{
					return "1";
				}
				if (!SettingsConVars.PpAOConVar.settings.active)
				{
					return "1";
				}
				return "0";
			}

			// Token: 0x04001D90 RID: 7568
			private static AmbientOcclusion settings;

			// Token: 0x04001D91 RID: 7569
			private static SettingsConVars.PpAOConVar instance = new SettingsConVars.PpAOConVar("pp_ao", ConVarFlags.Archive | ConVarFlags.Engine, null, "SSAO postprocessing. 0 = disabled 1 = enabled");
		}

		// Token: 0x020004A8 RID: 1192
		private class PpGammaConVar : BaseConVar
		{
			// Token: 0x06001AB5 RID: 6837 RVA: 0x00013CBF File Offset: 0x00011EBF
			private PpGammaConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<ColorGrading>(out SettingsConVars.PpGammaConVar.colorGradingSettings);
			}

			// Token: 0x06001AB6 RID: 6838 RVA: 0x000869DC File Offset: 0x00084BDC
			public override void SetString(string newValue)
			{
				float w;
				if (TextSerialization.TryParseInvariant(newValue, out w) && SettingsConVars.PpGammaConVar.colorGradingSettings)
				{
					SettingsConVars.PpGammaConVar.colorGradingSettings.gamma.value.w = w;
				}
			}

			// Token: 0x06001AB7 RID: 6839 RVA: 0x00013CE6 File Offset: 0x00011EE6
			public override string GetString()
			{
				if (!SettingsConVars.PpGammaConVar.colorGradingSettings)
				{
					return "0";
				}
				return TextSerialization.ToStringInvariant(SettingsConVars.PpGammaConVar.colorGradingSettings.gamma.value.w);
			}

			// Token: 0x04001D92 RID: 7570
			private static ColorGrading colorGradingSettings;

			// Token: 0x04001D93 RID: 7571
			private static SettingsConVars.PpGammaConVar instance = new SettingsConVars.PpGammaConVar("gamma", ConVarFlags.Archive, "0", "Gamma boost, from -inf to inf.");
		}
	}
}
