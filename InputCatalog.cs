using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Rewired;

namespace RoR2
{
	// Token: 0x0200044A RID: 1098
	public static class InputCatalog
	{
		// Token: 0x0600189D RID: 6301 RVA: 0x0007EB88 File Offset: 0x0007CD88
		static InputCatalog()
		{
			InputCatalog.<.cctor>g__Add|2_0("MoveHorizontal", "ACTION_MOVE_HORIZONTAL", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("MoveVertical", "ACTION_MOVE_VERTICAL", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("AimHorizontalMouse", "ACTION_AIM_HORIZONTAL_MOUSE", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("AimVerticalMouse", "ACTION_AIM_VERTICAL_MOUSE", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("AimHorizontalStick", "ACTION_AIM_HORIZONTAL_STICK", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("AimVerticalStick", "ACTION_AIM_VERTICAL_STICK", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("Jump", "ACTION_JUMP", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("Sprint", "ACTION_SPRINT", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("Interact", "ACTION_INTERACT", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("Equipment", "ACTION_EQUIPMENT", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("PrimarySkill", "ACTION_PRIMARY_SKILL", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("SecondarySkill", "ACTION_SECONDARY_SKILL", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("UtilitySkill", "ACTION_UTILITY_SKILL", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("SpecialSkill", "ACTION_SPECIAL_SKILL", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("Info", "ACTION_INFO", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("Ping", "ACTION_PING", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("MoveHorizontal", "ACTION_MOVE_HORIZONTAL_POSITIVE", AxisRange.Positive);
			InputCatalog.<.cctor>g__Add|2_0("MoveHorizontal", "ACTION_MOVE_HORIZONTAL_NEGATIVE", AxisRange.Negative);
			InputCatalog.<.cctor>g__Add|2_0("MoveVertical", "ACTION_MOVE_VERTICAL_POSITIVE", AxisRange.Positive);
			InputCatalog.<.cctor>g__Add|2_0("MoveVertical", "ACTION_MOVE_VERTICAL_NEGATIVE", AxisRange.Negative);
		}

		// Token: 0x0600189E RID: 6302 RVA: 0x0007ECE0 File Offset: 0x0007CEE0
		public static string GetActionNameToken(string actionName, AxisRange axisRange = AxisRange.Full)
		{
			string result;
			if (InputCatalog.actionToToken.TryGetValue(new InputCatalog.ActionAxisPair(actionName, axisRange), out result))
			{
				return result;
			}
			throw new ArgumentException(string.Format("Bad action/axis pair {0} {1}.", actionName, axisRange));
		}

		// Token: 0x04001BB3 RID: 7091
		private static readonly Dictionary<InputCatalog.ActionAxisPair, string> actionToToken = new Dictionary<InputCatalog.ActionAxisPair, string>();

		// Token: 0x0200044B RID: 1099
		private struct ActionAxisPair : IEquatable<InputCatalog.ActionAxisPair>
		{
			// Token: 0x060018A0 RID: 6304 RVA: 0x000127DF File Offset: 0x000109DF
			public ActionAxisPair([NotNull] string actionName, AxisRange axisRange)
			{
				this.actionName = actionName;
				this.axisRange = axisRange;
			}

			// Token: 0x060018A1 RID: 6305 RVA: 0x000127EF File Offset: 0x000109EF
			public bool Equals(InputCatalog.ActionAxisPair other)
			{
				return string.Equals(this.actionName, other.actionName) && this.axisRange == other.axisRange;
			}

			// Token: 0x060018A2 RID: 6306 RVA: 0x00012814 File Offset: 0x00010A14
			public override bool Equals(object obj)
			{
				return obj != null && obj is InputCatalog.ActionAxisPair && this.Equals((InputCatalog.ActionAxisPair)obj);
			}

			// Token: 0x060018A3 RID: 6307 RVA: 0x0007ED1C File Offset: 0x0007CF1C
			public override int GetHashCode()
			{
				return ((-1879861323 * -1521134295 + base.GetHashCode()) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.actionName)) * -1521134295 + this.axisRange.GetHashCode();
			}

			// Token: 0x04001BB4 RID: 7092
			[NotNull]
			private readonly string actionName;

			// Token: 0x04001BB5 RID: 7093
			private readonly AxisRange axisRange;
		}
	}
}
