using System;
using System.Globalization;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace RoR2
{
	// Token: 0x02000487 RID: 1159
	public static class SettingsConVars
	{
		// Token: 0x02000488 RID: 1160
		private class VSyncCountConVar : BaseConVar
		{
			// Token: 0x06001A08 RID: 6664 RVA: 0x000090A8 File Offset: 0x000072A8
			private VSyncCountConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A09 RID: 6665 RVA: 0x0008597C File Offset: 0x00083B7C
			public override void SetString(string newValue)
			{
				int vSyncCount;
				if (TextSerialization.TryParseInvariant(newValue, out vSyncCount))
				{
					QualitySettings.vSyncCount = vSyncCount;
				}
			}

			// Token: 0x06001A0A RID: 6666 RVA: 0x000133F7 File Offset: 0x000115F7
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.vSyncCount);
			}

			// Token: 0x04001D3F RID: 7487
			private static SettingsConVars.VSyncCountConVar instance = new SettingsConVars.VSyncCountConVar("vsync_count", ConVarFlags.Archive | ConVarFlags.Engine, null, "Vsync count.");
		}

		// Token: 0x02000489 RID: 1161
		private class WindowModeConVar : BaseConVar
		{
			// Token: 0x06001A0C RID: 6668 RVA: 0x000090A8 File Offset: 0x000072A8
			private WindowModeConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A0D RID: 6669 RVA: 0x0008599C File Offset: 0x00083B9C
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

			// Token: 0x06001A0E RID: 6670 RVA: 0x0001341C File Offset: 0x0001161C
			public override string GetString()
			{
				if (!Screen.fullScreen)
				{
					return "Window";
				}
				return "Fullscreen";
			}

			// Token: 0x04001D40 RID: 7488
			private static SettingsConVars.WindowModeConVar instance = new SettingsConVars.WindowModeConVar("window_mode", ConVarFlags.Archive | ConVarFlags.Engine, null, "The window mode. Choices are Fullscreen and Window.");

			// Token: 0x0200048A RID: 1162
			private enum WindowMode
			{
				// Token: 0x04001D42 RID: 7490
				Fullscreen,
				// Token: 0x04001D43 RID: 7491
				Window,
				// Token: 0x04001D44 RID: 7492
				WindowNoBorder
			}
		}

		// Token: 0x0200048B RID: 1163
		private class ResolutionConVar : BaseConVar
		{
			// Token: 0x06001A10 RID: 6672 RVA: 0x000090A8 File Offset: 0x000072A8
			private ResolutionConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A11 RID: 6673 RVA: 0x00085A04 File Offset: 0x00083C04
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

			// Token: 0x06001A12 RID: 6674 RVA: 0x00085A88 File Offset: 0x00083C88
			public override string GetString()
			{
				Resolution currentResolution = Screen.currentResolution;
				return string.Format(CultureInfo.InvariantCulture, "{0}x{1}x{2}", currentResolution.width, currentResolution.height, currentResolution.refreshRate);
			}

			// Token: 0x06001A13 RID: 6675 RVA: 0x00085AD0 File Offset: 0x00083CD0
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

			// Token: 0x04001D45 RID: 7493
			private static SettingsConVars.ResolutionConVar instance = new SettingsConVars.ResolutionConVar("resolution", ConVarFlags.Archive | ConVarFlags.Engine, null, "The resolution of the game window. Format example: 1920x1080x60");
		}

		// Token: 0x0200048C RID: 1164
		private class FpsMaxConVar : BaseConVar
		{
			// Token: 0x06001A15 RID: 6677 RVA: 0x000090A8 File Offset: 0x000072A8
			private FpsMaxConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A16 RID: 6678 RVA: 0x00085B44 File Offset: 0x00083D44
			public override void SetString(string newValue)
			{
				int targetFrameRate;
				if (TextSerialization.TryParseInvariant(newValue, out targetFrameRate))
				{
					Application.targetFrameRate = targetFrameRate;
				}
			}

			// Token: 0x06001A17 RID: 6679 RVA: 0x00013462 File Offset: 0x00011662
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(Application.targetFrameRate);
			}

			// Token: 0x04001D46 RID: 7494
			private static SettingsConVars.FpsMaxConVar instance = new SettingsConVars.FpsMaxConVar("fps_max", ConVarFlags.Archive | ConVarFlags.Engine, null, "Maximum FPS. -1 is unlimited.");
		}

		// Token: 0x0200048D RID: 1165
		private class ShadowsConVar : BaseConVar
		{
			// Token: 0x06001A19 RID: 6681 RVA: 0x000090A8 File Offset: 0x000072A8
			private ShadowsConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A1A RID: 6682 RVA: 0x00085B64 File Offset: 0x00083D64
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

			// Token: 0x06001A1B RID: 6683 RVA: 0x00085BAC File Offset: 0x00083DAC
			public override string GetString()
			{
				return QualitySettings.shadows.ToString();
			}

			// Token: 0x04001D47 RID: 7495
			private static SettingsConVars.ShadowsConVar instance = new SettingsConVars.ShadowsConVar("r_shadows", ConVarFlags.Archive | ConVarFlags.Engine, null, "Shadow quality. Can be \"All\" \"HardOnly\" or \"Disable\"");
		}

		// Token: 0x0200048E RID: 1166
		private class SoftParticlesConVar : BaseConVar
		{
			// Token: 0x06001A1D RID: 6685 RVA: 0x000090A8 File Offset: 0x000072A8
			private SoftParticlesConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A1E RID: 6686 RVA: 0x00085BCC File Offset: 0x00083DCC
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					QualitySettings.softParticles = (num != 0);
				}
			}

			// Token: 0x06001A1F RID: 6687 RVA: 0x000134A0 File Offset: 0x000116A0
			public override string GetString()
			{
				if (!QualitySettings.softParticles)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x04001D48 RID: 7496
			private static SettingsConVars.SoftParticlesConVar instance = new SettingsConVars.SoftParticlesConVar("r_softparticles", ConVarFlags.Archive | ConVarFlags.Engine, null, "Whether or not soft particles are enabled.");
		}

		// Token: 0x0200048F RID: 1167
		private class FoliageWindConVar : BaseConVar
		{
			// Token: 0x06001A21 RID: 6689 RVA: 0x000090A8 File Offset: 0x000072A8
			private FoliageWindConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A22 RID: 6690 RVA: 0x00085BEC File Offset: 0x00083DEC
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

			// Token: 0x06001A23 RID: 6691 RVA: 0x000134CD File Offset: 0x000116CD
			public override string GetString()
			{
				if (!Shader.IsKeywordEnabled("ENABLE_WIND"))
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x04001D49 RID: 7497
			private static SettingsConVars.FoliageWindConVar instance = new SettingsConVars.FoliageWindConVar("r_foliagewind", ConVarFlags.Archive, "1", "Whether or not foliage has wind.");
		}

		// Token: 0x02000490 RID: 1168
		private class LodBiasConVar : BaseConVar
		{
			// Token: 0x06001A25 RID: 6693 RVA: 0x000090A8 File Offset: 0x000072A8
			private LodBiasConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A26 RID: 6694 RVA: 0x00085C1C File Offset: 0x00083E1C
			public override void SetString(string newValue)
			{
				float lodBias;
				if (TextSerialization.TryParseInvariant(newValue, out lodBias))
				{
					QualitySettings.lodBias = lodBias;
				}
			}

			// Token: 0x06001A27 RID: 6695 RVA: 0x00013502 File Offset: 0x00011702
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.lodBias);
			}

			// Token: 0x04001D4A RID: 7498
			private static SettingsConVars.LodBiasConVar instance = new SettingsConVars.LodBiasConVar("r_lod_bias", ConVarFlags.Archive | ConVarFlags.Engine, null, "LOD bias.");
		}

		// Token: 0x02000491 RID: 1169
		private class MaximumLodConVar : BaseConVar
		{
			// Token: 0x06001A29 RID: 6697 RVA: 0x000090A8 File Offset: 0x000072A8
			private MaximumLodConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A2A RID: 6698 RVA: 0x00085C3C File Offset: 0x00083E3C
			public override void SetString(string newValue)
			{
				int maximumLODLevel;
				if (TextSerialization.TryParseInvariant(newValue, out maximumLODLevel))
				{
					QualitySettings.maximumLODLevel = maximumLODLevel;
				}
			}

			// Token: 0x06001A2B RID: 6699 RVA: 0x00013527 File Offset: 0x00011727
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.maximumLODLevel);
			}

			// Token: 0x04001D4B RID: 7499
			private static SettingsConVars.MaximumLodConVar instance = new SettingsConVars.MaximumLodConVar("r_lod_max", ConVarFlags.Archive | ConVarFlags.Engine, null, "The maximum allowed LOD level.");
		}

		// Token: 0x02000492 RID: 1170
		private class MasterTextureLimitConVar : BaseConVar
		{
			// Token: 0x06001A2D RID: 6701 RVA: 0x000090A8 File Offset: 0x000072A8
			private MasterTextureLimitConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A2E RID: 6702 RVA: 0x00085C5C File Offset: 0x00083E5C
			public override void SetString(string newValue)
			{
				int masterTextureLimit;
				if (TextSerialization.TryParseInvariant(newValue, out masterTextureLimit))
				{
					QualitySettings.masterTextureLimit = masterTextureLimit;
				}
			}

			// Token: 0x06001A2F RID: 6703 RVA: 0x0001354C File Offset: 0x0001174C
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.masterTextureLimit);
			}

			// Token: 0x04001D4C RID: 7500
			private static SettingsConVars.MasterTextureLimitConVar instance = new SettingsConVars.MasterTextureLimitConVar("master_texture_limit", ConVarFlags.Archive | ConVarFlags.Engine, null, "Reduction in texture quality. 0 is highest quality textures, 1 is half, 2 is quarter, etc.");
		}

		// Token: 0x02000493 RID: 1171
		private class AnisotropicFilteringConVar : BaseConVar
		{
			// Token: 0x06001A31 RID: 6705 RVA: 0x000090A8 File Offset: 0x000072A8
			private AnisotropicFilteringConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A32 RID: 6706 RVA: 0x00085C7C File Offset: 0x00083E7C
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

			// Token: 0x06001A33 RID: 6707 RVA: 0x00085CC4 File Offset: 0x00083EC4
			public override string GetString()
			{
				return QualitySettings.anisotropicFiltering.ToString();
			}

			// Token: 0x04001D4D RID: 7501
			private static SettingsConVars.AnisotropicFilteringConVar instance = new SettingsConVars.AnisotropicFilteringConVar("anisotropic_filtering", ConVarFlags.Archive, "Disable", "The anisotropic filtering mode. Can be \"Disable\", \"Enable\" or \"ForceEnable\".");
		}

		// Token: 0x02000494 RID: 1172
		private class ShadowResolutionConVar : BaseConVar
		{
			// Token: 0x06001A35 RID: 6709 RVA: 0x000090A8 File Offset: 0x000072A8
			private ShadowResolutionConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A36 RID: 6710 RVA: 0x00085CE4 File Offset: 0x00083EE4
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

			// Token: 0x06001A37 RID: 6711 RVA: 0x00085D2C File Offset: 0x00083F2C
			public override string GetString()
			{
				return QualitySettings.shadowResolution.ToString();
			}

			// Token: 0x04001D4E RID: 7502
			private static SettingsConVars.ShadowResolutionConVar instance = new SettingsConVars.ShadowResolutionConVar("shadow_resolution", ConVarFlags.Archive | ConVarFlags.Engine, "Medium", "Default shadow resolution. Can be \"Low\", \"Medium\", \"High\" or \"VeryHigh\".");
		}

		// Token: 0x02000495 RID: 1173
		private class ShadowCascadesConVar : BaseConVar
		{
			// Token: 0x06001A39 RID: 6713 RVA: 0x000090A8 File Offset: 0x000072A8
			private ShadowCascadesConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A3A RID: 6714 RVA: 0x00085D4C File Offset: 0x00083F4C
			public override void SetString(string newValue)
			{
				int shadowCascades;
				if (TextSerialization.TryParseInvariant(newValue, out shadowCascades))
				{
					QualitySettings.shadowCascades = shadowCascades;
				}
			}

			// Token: 0x06001A3B RID: 6715 RVA: 0x000135AA File Offset: 0x000117AA
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.shadowCascades);
			}

			// Token: 0x04001D4F RID: 7503
			private static SettingsConVars.ShadowCascadesConVar instance = new SettingsConVars.ShadowCascadesConVar("shadow_cascades", ConVarFlags.Archive | ConVarFlags.Engine, null, "The number of cascades to use for directional light shadows. low=0 high=4");
		}

		// Token: 0x02000496 RID: 1174
		private class ShadowDistanceConvar : BaseConVar
		{
			// Token: 0x06001A3D RID: 6717 RVA: 0x000090A8 File Offset: 0x000072A8
			private ShadowDistanceConvar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001A3E RID: 6718 RVA: 0x00085D6C File Offset: 0x00083F6C
			public override void SetString(string newValue)
			{
				float shadowDistance;
				if (TextSerialization.TryParseInvariant(newValue, out shadowDistance))
				{
					QualitySettings.shadowDistance = shadowDistance;
				}
			}

			// Token: 0x06001A3F RID: 6719 RVA: 0x000135CF File Offset: 0x000117CF
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.shadowDistance);
			}

			// Token: 0x04001D50 RID: 7504
			private static SettingsConVars.ShadowDistanceConvar instance = new SettingsConVars.ShadowDistanceConvar("shadow_distance", ConVarFlags.Archive, "200", "The distance in meters to draw shadows.");
		}

		// Token: 0x02000497 RID: 1175
		private class PpMotionBlurConVar : BaseConVar
		{
			// Token: 0x06001A41 RID: 6721 RVA: 0x000135F7 File Offset: 0x000117F7
			private PpMotionBlurConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<MotionBlur>(out SettingsConVars.PpMotionBlurConVar.settings);
			}

			// Token: 0x06001A42 RID: 6722 RVA: 0x00085D8C File Offset: 0x00083F8C
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num) && SettingsConVars.PpMotionBlurConVar.settings)
				{
					SettingsConVars.PpMotionBlurConVar.settings.active = (num == 0);
				}
			}

			// Token: 0x06001A43 RID: 6723 RVA: 0x0001361E File Offset: 0x0001181E
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

			// Token: 0x04001D51 RID: 7505
			private static MotionBlur settings;

			// Token: 0x04001D52 RID: 7506
			private static SettingsConVars.PpMotionBlurConVar instance = new SettingsConVars.PpMotionBlurConVar("pp_motionblur", ConVarFlags.Archive, "0", "Motion blur. 0 = disabled 1 = enabled");
		}

		// Token: 0x02000498 RID: 1176
		private class PpSobelOutlineConVar : BaseConVar
		{
			// Token: 0x06001A45 RID: 6725 RVA: 0x00013665 File Offset: 0x00011865
			private PpSobelOutlineConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<SobelOutline>(out SettingsConVars.PpSobelOutlineConVar.sobelOutlineSettings);
			}

			// Token: 0x06001A46 RID: 6726 RVA: 0x00085DC0 File Offset: 0x00083FC0
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num) && SettingsConVars.PpSobelOutlineConVar.sobelOutlineSettings)
				{
					SettingsConVars.PpSobelOutlineConVar.sobelOutlineSettings.active = (num == 0);
				}
			}

			// Token: 0x06001A47 RID: 6727 RVA: 0x0001368C File Offset: 0x0001188C
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

			// Token: 0x04001D53 RID: 7507
			private static SobelOutline sobelOutlineSettings;

			// Token: 0x04001D54 RID: 7508
			private static SettingsConVars.PpSobelOutlineConVar instance = new SettingsConVars.PpSobelOutlineConVar("pp_sobel_outline", ConVarFlags.Archive, "1", "Whether or not to use the sobel rim light effect.");
		}

		// Token: 0x02000499 RID: 1177
		private class PpBloomConVar : BaseConVar
		{
			// Token: 0x06001A49 RID: 6729 RVA: 0x000136D3 File Offset: 0x000118D3
			private PpBloomConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<Bloom>(out SettingsConVars.PpBloomConVar.settings);
			}

			// Token: 0x06001A4A RID: 6730 RVA: 0x00085DF4 File Offset: 0x00083FF4
			public override void SetString(string newValue)
			{
				int num;
				if (!TextSerialization.TryParseInvariant(newValue, out num) && SettingsConVars.PpBloomConVar.settings)
				{
					SettingsConVars.PpBloomConVar.settings.active = (num == 0);
				}
			}

			// Token: 0x06001A4B RID: 6731 RVA: 0x000136FA File Offset: 0x000118FA
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

			// Token: 0x04001D55 RID: 7509
			private static Bloom settings;

			// Token: 0x04001D56 RID: 7510
			private static SettingsConVars.PpBloomConVar instance = new SettingsConVars.PpBloomConVar("pp_bloom", ConVarFlags.Archive | ConVarFlags.Engine, null, "Bloom postprocessing. 0 = disabled 1 = enabled");
		}

		// Token: 0x0200049A RID: 1178
		private class PpAOConVar : BaseConVar
		{
			// Token: 0x06001A4D RID: 6733 RVA: 0x0001373E File Offset: 0x0001193E
			private PpAOConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<AmbientOcclusion>(out SettingsConVars.PpAOConVar.settings);
			}

			// Token: 0x06001A4E RID: 6734 RVA: 0x00085E28 File Offset: 0x00084028
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num) && SettingsConVars.PpAOConVar.settings)
				{
					SettingsConVars.PpAOConVar.settings.active = (num == 0);
				}
			}

			// Token: 0x06001A4F RID: 6735 RVA: 0x00013765 File Offset: 0x00011965
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

			// Token: 0x04001D57 RID: 7511
			private static AmbientOcclusion settings;

			// Token: 0x04001D58 RID: 7512
			private static SettingsConVars.PpAOConVar instance = new SettingsConVars.PpAOConVar("pp_ao", ConVarFlags.Archive | ConVarFlags.Engine, null, "SSAO postprocessing. 0 = disabled 1 = enabled");
		}

		// Token: 0x0200049B RID: 1179
		private class PpGammaConVar : BaseConVar
		{
			// Token: 0x06001A51 RID: 6737 RVA: 0x000137A9 File Offset: 0x000119A9
			private PpGammaConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<ColorGrading>(out SettingsConVars.PpGammaConVar.colorGradingSettings);
			}

			// Token: 0x06001A52 RID: 6738 RVA: 0x00085E5C File Offset: 0x0008405C
			public override void SetString(string newValue)
			{
				float w;
				if (TextSerialization.TryParseInvariant(newValue, out w) && SettingsConVars.PpGammaConVar.colorGradingSettings)
				{
					SettingsConVars.PpGammaConVar.colorGradingSettings.gamma.value.w = w;
				}
			}

			// Token: 0x06001A53 RID: 6739 RVA: 0x000137D0 File Offset: 0x000119D0
			public override string GetString()
			{
				if (!SettingsConVars.PpGammaConVar.colorGradingSettings)
				{
					return "0";
				}
				return TextSerialization.ToStringInvariant(SettingsConVars.PpGammaConVar.colorGradingSettings.gamma.value.w);
			}

			// Token: 0x04001D59 RID: 7513
			private static ColorGrading colorGradingSettings;

			// Token: 0x04001D5A RID: 7514
			private static SettingsConVars.PpGammaConVar instance = new SettingsConVars.PpGammaConVar("gamma", ConVarFlags.Archive, "0", "Gamma boost, from -inf to inf.");
		}
	}
}
