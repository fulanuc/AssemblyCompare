using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x02000656 RID: 1622
	[RequireComponent(typeof(RectTransform))]
	public class TimerText : MonoBehaviour
	{
		// Token: 0x0600247B RID: 9339 RVA: 0x000AC9C4 File Offset: 0x000AABC4
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

		// Token: 0x0600247C RID: 9340 RVA: 0x000ACA94 File Offset: 0x000AAC94
		private void Update()
		{
			float displayData = 0f;
			Run instance = Run.instance;
			if (instance)
			{
				displayData = instance.GetRunStopwatch();
			}
			this.SetDisplayData(displayData);
		}

		// Token: 0x0600247D RID: 9341 RVA: 0x0001A914 File Offset: 0x00018B14
		private void OnValidate()
		{
			if (!this.targetLabel)
			{
				Debug.LogErrorFormat(this, "TimerText does not specify a target label.", Array.Empty<object>());
			}
		}

		// Token: 0x0400271E RID: 10014
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

		// Token: 0x0400271F RID: 10015
		[FormerlySerializedAs("targetText")]
		public TextMeshProUGUI targetLabel;

		// Token: 0x04002720 RID: 10016
		public bool showTimerTutorial;

		// Token: 0x04002721 RID: 10017
		private float currentDisplayData;
	}
}
