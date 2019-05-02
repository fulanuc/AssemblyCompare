using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x02000644 RID: 1604
	[RequireComponent(typeof(RectTransform))]
	public class TimerText : MonoBehaviour
	{
		// Token: 0x060023EB RID: 9195 RVA: 0x000AB344 File Offset: 0x000A9544
		private void SetDisplayData(float newDisplayData)
		{
			if (newDisplayData == this.currentDisplayData)
			{
				return;
			}
			this.currentDisplayData = newDisplayData;
			int num = Mathf.FloorToInt(newDisplayData * 0.0166666675f);
			int num2 = (int)newDisplayData - num * 60;
			int value = Mathf.FloorToInt((newDisplayData - (float)num2 - (float)num * 60f) * 100f);
			TimerText.sharedStringBuilder.Clear();
			TimerText.sharedStringBuilder.Append("<mspace=2.0em>");
			TimerText.sharedStringBuilder.AppendInt(num, 2u, uint.MaxValue);
			TimerText.sharedStringBuilder.Append("</mspace>:<mspace=2.0em>");
			TimerText.sharedStringBuilder.AppendUint((uint)num2, 2u, 2u);
			TimerText.sharedStringBuilder.Append("</mspace><voffset=0.4em><size=40%><mspace=2.0em>:");
			TimerText.sharedStringBuilder.AppendUint((uint)value, 2u, 2u);
			TimerText.sharedStringBuilder.Append("</size></voffset></mspace>");
			this.targetLabel.SetText(TimerText.sharedStringBuilder);
		}

		// Token: 0x060023EC RID: 9196 RVA: 0x000AB414 File Offset: 0x000A9614
		private void Update()
		{
			float displayData = 0f;
			Run instance = Run.instance;
			if (instance)
			{
				displayData = instance.time;
			}
			this.SetDisplayData(displayData);
		}

		// Token: 0x060023ED RID: 9197 RVA: 0x0001A246 File Offset: 0x00018446
		private void OnValidate()
		{
			if (!this.targetLabel)
			{
				Debug.LogErrorFormat(this, "TimerText does not specify a target label.", Array.Empty<object>());
			}
		}

		// Token: 0x040026C3 RID: 9923
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

		// Token: 0x040026C4 RID: 9924
		[FormerlySerializedAs("targetText")]
		public TextMeshProUGUI targetLabel;

		// Token: 0x040026C5 RID: 9925
		public bool showTimerTutorial;

		// Token: 0x040026C6 RID: 9926
		private float currentDisplayData;
	}
}
