using System;
using Rewired;
using RoR2.UI;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200040D RID: 1037
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class InputBindingDisplayController : MonoBehaviour
	{
		// Token: 0x06001743 RID: 5955 RVA: 0x000117B6 File Offset: 0x0000F9B6
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.guiLabel = base.GetComponent<TextMeshProUGUI>();
			this.label = base.GetComponent<TextMeshPro>();
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x0007969C File Offset: 0x0007789C
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

		// Token: 0x06001745 RID: 5957 RVA: 0x000117DC File Offset: 0x0000F9DC
		private void Update()
		{
			this.Refresh();
		}

		// Token: 0x04001A46 RID: 6726
		public string actionName;

		// Token: 0x04001A47 RID: 6727
		public AxisRange axisRange;

		// Token: 0x04001A48 RID: 6728
		public bool useExplicitInputSource;

		// Token: 0x04001A49 RID: 6729
		public MPEventSystem.InputSource explicitInputSource;

		// Token: 0x04001A4A RID: 6730
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04001A4B RID: 6731
		private TextMeshProUGUI guiLabel;

		// Token: 0x04001A4C RID: 6732
		private TextMeshPro label;
	}
}
