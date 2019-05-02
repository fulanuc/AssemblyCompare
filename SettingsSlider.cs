using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000631 RID: 1585
	public class SettingsSlider : BaseSettingsControl
	{
		// Token: 0x06002398 RID: 9112 RVA: 0x000AA150 File Offset: 0x000A8350
		protected new void Awake()
		{
			base.Awake();
			if (this.slider)
			{
				this.slider.minValue = this.minValue;
				this.slider.maxValue = this.maxValue;
				this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderValueChanged));
			}
			if (this.inputField)
			{
				this.inputField.onSubmit.AddListener(new UnityAction<string>(this.OnInputFieldSubmit));
			}
		}

		// Token: 0x06002399 RID: 9113 RVA: 0x00019F3F File Offset: 0x0001813F
		private void OnSliderValueChanged(float newValue)
		{
			if (base.inUpdateControls)
			{
				return;
			}
			base.SubmitSetting(TextSerialization.ToStringInvariant(newValue));
		}

		// Token: 0x0600239A RID: 9114 RVA: 0x000AA1D8 File Offset: 0x000A83D8
		private void OnInputFieldSubmit(string newString)
		{
			if (base.inUpdateControls)
			{
				return;
			}
			float value;
			if (TextSerialization.TryParseInvariant(newString, out value))
			{
				base.SubmitSetting(TextSerialization.ToStringInvariant(value));
			}
		}

		// Token: 0x0600239B RID: 9115 RVA: 0x00019F56 File Offset: 0x00018156
		protected new void OnEnable()
		{
			base.OnEnable();
			base.UpdateControls();
		}

		// Token: 0x0600239C RID: 9116 RVA: 0x000AA204 File Offset: 0x000A8404
		protected override void OnUpdateControls()
		{
			base.OnUpdateControls();
			float value;
			if (TextSerialization.TryParseInvariant(base.GetCurrentValue(), out value))
			{
				float num = Mathf.Clamp(value, this.minValue, this.maxValue);
				if (this.slider)
				{
					this.slider.value = num;
				}
				if (this.inputField)
				{
					this.inputField.text = string.Format(CultureInfo.InvariantCulture, this.formatString, num);
				}
			}
		}

		// Token: 0x04002676 RID: 9846
		public Slider slider;

		// Token: 0x04002677 RID: 9847
		public TMP_InputField inputField;

		// Token: 0x04002678 RID: 9848
		public float minValue;

		// Token: 0x04002679 RID: 9849
		public float maxValue;

		// Token: 0x0400267A RID: 9850
		public string formatString = "{0:0.00}";
	}
}
