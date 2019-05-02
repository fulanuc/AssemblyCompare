﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Rewired;
using RoR2.UI;

namespace RoR2
{
	// Token: 0x02000434 RID: 1076
	public static class Glyphs
	{
		// Token: 0x060017F9 RID: 6137 RVA: 0x00011E2D File Offset: 0x0001002D
		private static void AddGlyph(string controllerName, int elementIndex, string assetName, string glyphName)
		{
			Glyphs.glyphMap[new Glyphs.GlyphKey(controllerName, elementIndex)] = string.Format(CultureInfo.InvariantCulture, "<sprite=\"{0}\" name=\"{1}\">", assetName, glyphName);
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x0007D2F0 File Offset: 0x0007B4F0
		private static void RegisterXBoxController(string controllerName)
		{
			Glyphs.AddGlyph(controllerName, 4, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_5");
			Glyphs.AddGlyph(controllerName, 5, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_9");
			Glyphs.AddGlyph(controllerName, 10, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_2");
			Glyphs.AddGlyph(controllerName, 11, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_6");
			Glyphs.AddGlyph(controllerName, 6, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_0");
			Glyphs.AddGlyph(controllerName, 7, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_1");
			Glyphs.AddGlyph(controllerName, 8, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_7");
			Glyphs.AddGlyph(controllerName, 9, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_11");
			Glyphs.AddGlyph(controllerName, 12, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_10");
			Glyphs.AddGlyph(controllerName, 13, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_3");
			Glyphs.AddGlyph(controllerName, 14, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_4");
			Glyphs.AddGlyph(controllerName, 15, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_8");
		}

		// Token: 0x060017FB RID: 6139 RVA: 0x0007D3D0 File Offset: 0x0007B5D0
		private static void RegisterDS4Controller(string controllerName)
		{
			Glyphs.AddGlyph(controllerName, 4, "tmpsprSteamGlyphs", "texSteamGlyphs_110");
			Glyphs.AddGlyph(controllerName, 5, "tmpsprSteamGlyphs", "texSteamGlyphs_112");
			Glyphs.AddGlyph(controllerName, 10, "tmpsprSteamGlyphs", "texSteamGlyphs_84");
			Glyphs.AddGlyph(controllerName, 11, "tmpsprSteamGlyphs", "texSteamGlyphs_85");
			Glyphs.AddGlyph(controllerName, 6, "tmpsprSteamGlyphs", "texSteamGlyphs_49");
			Glyphs.AddGlyph(controllerName, 7, "tmpsprSteamGlyphs", "texSteamGlyphs_39");
			Glyphs.AddGlyph(controllerName, 8, "tmpsprSteamGlyphs", "texSteamGlyphs_46");
			Glyphs.AddGlyph(controllerName, 9, "tmpsprSteamGlyphs", "texSteamGlyphs_47");
		}

		// Token: 0x060017FC RID: 6140 RVA: 0x00011E51 File Offset: 0x00010051
		private static void RegisterMouse(string controllerName)
		{
			Glyphs.AddGlyph(controllerName, 3, "tmpsprSteamGlyphs", "texSteamGlyphs_17");
			Glyphs.AddGlyph(controllerName, 4, "tmpsprSteamGlyphs", "texSteamGlyphs_18");
			Glyphs.AddGlyph(controllerName, 5, "tmpsprSteamGlyphs", "texSteamGlyphs_19");
		}

		// Token: 0x060017FD RID: 6141 RVA: 0x000025F6 File Offset: 0x000007F6
		private static void RegisterKeyboard(string controllerName)
		{
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x0007D468 File Offset: 0x0007B668
		static Glyphs()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Left Mouse Button"] = "M1";
			dictionary["Right Mouse Button"] = "M2";
			dictionary["Mouse Button 3"] = "M3";
			dictionary["Mouse Button 4"] = "M4";
			dictionary["Mouse Button 5"] = "M5";
			dictionary["Mouse Button 6"] = "M6";
			dictionary["Mouse Button 7"] = "M7";
			dictionary["Mouse Wheel"] = "MW";
			dictionary["Mouse Wheel +"] = "MW+";
			dictionary["Mouse Wheel -"] = "MW-";
			Glyphs.mouseElementRenameMap = dictionary;
			Glyphs.resultsList = new List<ActionElementMap>();
			Glyphs.RegisterXBoxController("Xbox 360 Controller");
			Glyphs.RegisterXBoxController("Xbox One Controller");
			for (int i = 0; i < 4; i++)
			{
				Glyphs.RegisterXBoxController("XInput Gamepad " + i);
			}
			Glyphs.RegisterDS4Controller("Sony DualShock 4");
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x0007D57C File Offset: 0x0007B77C
		private static string GetKeyboardGlyphString(string actionName)
		{
			string text;
			if (!Glyphs.keyboardRawNameToGlyphName.TryGetValue(actionName, out text))
			{
				if (!(actionName == "Left Shift"))
				{
					if (actionName == "Left Control")
					{
						actionName = "Ctrl";
					}
				}
				else
				{
					actionName = "Shift";
				}
				text = actionName;
				Glyphs.keyboardRawNameToGlyphName[actionName] = text;
			}
			return text;
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x0007D5D4 File Offset: 0x0007B7D4
		public static string GetGlyphString(MPEventSystemLocator eventSystemLocator, string actionName)
		{
			MPEventSystem eventSystem = eventSystemLocator.eventSystem;
			if (eventSystem)
			{
				return Glyphs.GetGlyphString(eventSystem, actionName, AxisRange.Full);
			}
			return "UNKNOWN";
		}

		// Token: 0x06001801 RID: 6145 RVA: 0x00011E86 File Offset: 0x00010086
		public static string GetGlyphString(MPEventSystem eventSystem, string actionName, AxisRange axisRange = AxisRange.Full)
		{
			return Glyphs.GetGlyphString(eventSystem, actionName, axisRange, eventSystem.currentInputSource);
		}

		// Token: 0x06001802 RID: 6146 RVA: 0x0007D600 File Offset: 0x0007B800
		public static string GetGlyphString(MPEventSystem eventSystem, string actionName, AxisRange axisRange, MPEventSystem.InputSource currentInputSource)
		{
			Glyphs.<>c__DisplayClass18_0 CS$<>8__locals1;
			CS$<>8__locals1.inputPlayer = eventSystem.player;
			InputAction action = ReInput.mapping.GetAction(actionName);
			CS$<>8__locals1.inputActionId = action.id;
			CS$<>8__locals1.controllerName = "Xbox One Controller";
			CS$<>8__locals1.controllerType = (ControllerType)(-1);
			CS$<>8__locals1.axisContributionMatters = (axisRange > AxisRange.Full);
			CS$<>8__locals1.axisContribution = Pole.Positive;
			if (axisRange == AxisRange.Negative)
			{
				CS$<>8__locals1.axisContribution = Pole.Negative;
			}
			if (currentInputSource != MPEventSystem.InputSource.Keyboard)
			{
				if (currentInputSource != MPEventSystem.InputSource.Gamepad)
				{
					throw new ArgumentOutOfRangeException();
				}
				CS$<>8__locals1.controllerType = ControllerType.Joystick;
				Glyphs.<GetGlyphString>g__SetController|18_0(CS$<>8__locals1.inputPlayer.controllers.GetLastActiveController(ControllerType.Joystick), ref CS$<>8__locals1);
				if (CS$<>8__locals1.actionElementMap == null)
				{
					foreach (Controller controller in CS$<>8__locals1.inputPlayer.controllers.Controllers)
					{
						if (controller.type == ControllerType.Joystick)
						{
							Glyphs.<GetGlyphString>g__SetController|18_0(controller, ref CS$<>8__locals1);
							if (CS$<>8__locals1.actionElementMap != null)
							{
								break;
							}
						}
					}
				}
				if (CS$<>8__locals1.actionElementMap == null && eventSystem.localUser != null)
				{
					using (IEnumerator<ActionElementMap> enumerator2 = eventSystem.localUser.userProfile.joystickMap.ElementMapsWithAction(CS$<>8__locals1.inputActionId).GetEnumerator())
					{
						if (enumerator2.MoveNext())
						{
							ActionElementMap actionElementMap = enumerator2.Current;
							CS$<>8__locals1.actionElementMap = actionElementMap;
						}
					}
				}
				if (CS$<>8__locals1.actionElementMap == null)
				{
					return "[NO GAMEPAD BINDING]";
				}
			}
			else
			{
				Glyphs.<GetGlyphString>g__SetController|18_0(CS$<>8__locals1.inputPlayer.controllers.Keyboard, ref CS$<>8__locals1);
				if (CS$<>8__locals1.actionElementMap == null)
				{
					Glyphs.<GetGlyphString>g__SetController|18_0(CS$<>8__locals1.inputPlayer.controllers.Mouse, ref CS$<>8__locals1);
				}
				if (CS$<>8__locals1.actionElementMap == null)
				{
					return "[NO KB/M BINDING]";
				}
			}
			int elementIdentifierId = CS$<>8__locals1.actionElementMap.elementIdentifierId;
			Glyphs.GlyphKey key = new Glyphs.GlyphKey(CS$<>8__locals1.controllerName, elementIdentifierId);
			string result;
			if (Glyphs.glyphMap.TryGetValue(key, out result))
			{
				return result;
			}
			if (CS$<>8__locals1.controllerType == ControllerType.Keyboard)
			{
				return Glyphs.GetKeyboardGlyphString(CS$<>8__locals1.actionElementMap.elementIdentifierName);
			}
			if (CS$<>8__locals1.controllerType == ControllerType.Mouse)
			{
				string text = CS$<>8__locals1.actionElementMap.elementIdentifierName;
				string text2;
				if (Glyphs.mouseElementRenameMap.TryGetValue(text, out text2))
				{
					text = text2;
				}
				return text;
			}
			return "UNKNOWN";
		}

		// Token: 0x04001B3F RID: 6975
		private static readonly Dictionary<Glyphs.GlyphKey, string> glyphMap = new Dictionary<Glyphs.GlyphKey, string>();

		// Token: 0x04001B40 RID: 6976
		private const string xbox360ControllerName = "Xbox 360 Controller";

		// Token: 0x04001B41 RID: 6977
		private const string xboxOneControllerName = "Xbox One Controller";

		// Token: 0x04001B42 RID: 6978
		private const string dualshock4ControllerName = "Sony DualShock 4";

		// Token: 0x04001B43 RID: 6979
		private const string defaultControllerName = "Xbox One Controller";

		// Token: 0x04001B44 RID: 6980
		private static readonly Dictionary<string, string> keyboardRawNameToGlyphName = new Dictionary<string, string>();

		// Token: 0x04001B45 RID: 6981
		private static readonly Dictionary<string, string> mouseElementRenameMap;

		// Token: 0x04001B46 RID: 6982
		private static readonly List<ActionElementMap> resultsList;

		// Token: 0x02000435 RID: 1077
		private struct GlyphKey : IEquatable<Glyphs.GlyphKey>
		{
			// Token: 0x06001804 RID: 6148 RVA: 0x00011E96 File Offset: 0x00010096
			public GlyphKey(string deviceName, int elementId)
			{
				this.deviceName = deviceName;
				this.elementId = elementId;
			}

			// Token: 0x06001805 RID: 6149 RVA: 0x00011EA6 File Offset: 0x000100A6
			public bool Equals(Glyphs.GlyphKey other)
			{
				return string.Equals(this.deviceName, other.deviceName) && this.elementId == other.elementId;
			}

			// Token: 0x06001806 RID: 6150 RVA: 0x00011ECB File Offset: 0x000100CB
			public override bool Equals(object obj)
			{
				return obj != null && obj is Glyphs.GlyphKey && this.Equals((Glyphs.GlyphKey)obj);
			}

			// Token: 0x06001807 RID: 6151 RVA: 0x00011EE8 File Offset: 0x000100E8
			public override int GetHashCode()
			{
				return ((this.deviceName != null) ? this.deviceName.GetHashCode() : 0) * 397 ^ this.elementId;
			}

			// Token: 0x04001B47 RID: 6983
			public readonly string deviceName;

			// Token: 0x04001B48 RID: 6984
			public readonly int elementId;
		}
	}
}
