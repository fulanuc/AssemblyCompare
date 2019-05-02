using System;
using Rewired;
using RoR2.UI;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000407 RID: 1031
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class InputBindingDisplayController : MonoBehaviour
	{
		// Token: 0x06001700 RID: 5888 RVA: 0x0001138A File Offset: 0x0000F58A
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.guiLabel = base.GetComponent<TextMeshProUGUI>();
			this.label = base.GetComponent<TextMeshPro>();
		}

		// Token: 0x06001701 RID: 5889 RVA: 0x000790DC File Offset: 0x000772DC
		private void Refresh()
		{
			string glyphString;
			if (this.useExplicitInputSource)
			{
				glyphString = Glyphs.GetGlyphString(this.eventSystemLocator.eventSystem, this.actionName, this.axisRange, this.explicitInputSource);
			}
			else
			{
				glyphString = Glyphs.GetGlyphString(this.eventSystemLocator.eventSystem, this.actionName, AxisRange.Full);
			}
			if (this.guiLabel)
			{
				this.guiLabel.text = glyphString;
				return;
			}
			if (this.label)
			{
				this.label.text = glyphString;
			}
		}

		// Token: 0x06001702 RID: 5890 RVA: 0x000113B0 File Offset: 0x0000F5B0
		private void Update()
		{
			this.Refresh();
		}

		// Token: 0x04001A1D RID: 6685
		public string actionName;

		// Token: 0x04001A1E RID: 6686
		public AxisRange axisRange;

		// Token: 0x04001A1F RID: 6687
		public bool useExplicitInputSource;

		// Token: 0x04001A20 RID: 6688
		public MPEventSystem.InputSource explicitInputSource;

		// Token: 0x04001A21 RID: 6689
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04001A22 RID: 6690
		private TextMeshProUGUI guiLabel;

		// Token: 0x04001A23 RID: 6691
		private TextMeshPro label;
	}
}
