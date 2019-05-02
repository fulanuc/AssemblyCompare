using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000643 RID: 1603
	public class SettingsSlider : BaseSettingsControl
	{
		// Token: 0x06002428 RID: 9256 RVA: 0x000AB7CC File Offset: 0x000A99CC
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

		// Token: 0x06002429 RID: 9257 RVA: 0x0001A60D File Offset: 0x0001880D
		private void OnSliderValueChanged(float newValue)
		{
			if (base.inUpdateControls)
			{
				return;
			}
			base.SubmitSetting(TextSerialization.ToStringInvariant(newValue));
		}

		// Token: 0x0600242A RID: 9258 RVA: 0x000AB854 File Offset: 0x000A9A54
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

		// Token: 0x0600242B RID: 9259 RVA: 0x0001A624 File Offset: 0x00018824
		protected new void OnEnable()
		{
			base.OnEnable();
			base.UpdateControls();
		}

		// Token: 0x0600242C RID: 9260 RVA: 0x000AB880 File Offset: 0x000A9A80
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

		// Token: 0x040026D1 RID: 9937
		public Slider slider;

		// Token: 0x040026D2 RID: 9938
		public TMP_InputField inputField;

		// Token: 0x040026D3 RID: 9939
		public float minValue;

		// Token: 0x040026D4 RID: 9940
		public float maxValue;

		// Token: 0x040026D5 RID: 9941
		public string formatString = "{0:0.00}";
	}
}
