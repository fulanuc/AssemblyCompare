using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.UI
{
	// Token: 0x02000622 RID: 1570
	public class ResolutionControl : BaseSettingsControl
	{
		// Token: 0x06002347 RID: 9031 RVA: 0x00019B68 File Offset: 0x00017D68
		private static Vector2Int ResolutionToVector2Int(Resolution resolution)
		{
			return new Vector2Int(resolution.width, resolution.height);
		}

		// Token: 0x06002348 RID: 9032 RVA: 0x00019B7D File Offset: 0x00017D7D
		private ResolutionControl.ResolutionOption GetCurrentSelectedResolutionOption()
		{
			if (this.resolutionDropdown.value >= 0)
			{
				return this.resolutionOptions[this.resolutionDropdown.value];
			}
			return null;
		}

		// Token: 0x06002349 RID: 9033 RVA: 0x000A9090 File Offset: 0x000A7290
		private void GenerateResolutionOptions()
		{
			Resolution[] array = Screen.resolutions;
			this.resolutionOptions = (from v in array.Select(new Func<Resolution, Vector2Int>(ResolutionControl.ResolutionToVector2Int)).Distinct<Vector2Int>()
			select new ResolutionControl.ResolutionOption
			{
				size = v
			}).ToArray<ResolutionControl.ResolutionOption>();
			foreach (ResolutionControl.ResolutionOption resolutionOption in this.resolutionOptions)
			{
				foreach (Resolution resolution in array)
				{
					if (ResolutionControl.ResolutionToVector2Int(resolution) == resolutionOption.size)
					{
						resolutionOption.supportedRefreshRates.Add(resolution.refreshRate);
					}
				}
			}
			List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
			foreach (ResolutionControl.ResolutionOption resolutionOption2 in this.resolutionOptions)
			{
				list.Add(new TMP_Dropdown.OptionData
				{
					text = resolutionOption2.GenerateDisplayString()
				});
			}
			this.resolutionDropdown.ClearOptions();
			this.resolutionDropdown.AddOptions(list);
			int value = -1;
			Vector2Int lhs = ResolutionControl.ResolutionToVector2Int(Screen.currentResolution);
			for (int k = 0; k < this.resolutionOptions.Length; k++)
			{
				if (lhs == this.resolutionOptions[k].size)
				{
					value = k;
					break;
				}
			}
			this.resolutionDropdown.value = value;
		}

		// Token: 0x0600234A RID: 9034 RVA: 0x000A91F8 File Offset: 0x000A73F8
		private void GenerateRefreshRateOptions()
		{
			this.refreshRateDropdown.ClearOptions();
			ResolutionControl.ResolutionOption currentSelectedResolutionOption = this.GetCurrentSelectedResolutionOption();
			if (currentSelectedResolutionOption == null)
			{
				return;
			}
			List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
			foreach (int num in currentSelectedResolutionOption.supportedRefreshRates)
			{
				list.Add(new TMP_Dropdown.OptionData(num.ToString() + "Hz"));
			}
			this.refreshRateDropdown.AddOptions(list);
			int num2 = currentSelectedResolutionOption.supportedRefreshRates.IndexOf(Screen.currentResolution.refreshRate);
			if (num2 == -1)
			{
				num2 = currentSelectedResolutionOption.supportedRefreshRates.Count - 1;
			}
			this.refreshRateDropdown.value = num2;
		}

		// Token: 0x0600234B RID: 9035 RVA: 0x00019BA1 File Offset: 0x00017DA1
		protected new void Awake()
		{
			base.Awake();
			this.resolutionDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnResolutionDropdownValueChanged));
			this.refreshRateDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnRefreshRateDropdownValueChanged));
		}

		// Token: 0x0600234C RID: 9036 RVA: 0x00019BE1 File Offset: 0x00017DE1
		protected new void OnEnable()
		{
			base.OnEnable();
			this.GenerateResolutionOptions();
		}

		// Token: 0x0600234D RID: 9037 RVA: 0x00019BEF File Offset: 0x00017DEF
		private void OnResolutionDropdownValueChanged(int newValue)
		{
			if (newValue < 0)
			{
				return;
			}
			this.GenerateRefreshRateOptions();
		}

		// Token: 0x0600234E RID: 9038 RVA: 0x00019BFC File Offset: 0x00017DFC
		private void OnRefreshRateDropdownValueChanged(int newValue)
		{
		}

		// Token: 0x0600234F RID: 9039 RVA: 0x000A92C4 File Offset: 0x000A74C4
		public void SubmitCurrentValue()
		{
			if (this.resolutionDropdown.value == -1 || this.refreshRateDropdown.value == -1)
			{
				return;
			}
			ResolutionControl.ResolutionOption resolutionOption = this.resolutionOptions[this.resolutionDropdown.value];
			base.SubmitSetting(string.Format(CultureInfo.InvariantCulture, "{0}x{1}x{2}", resolutionOption.size.x, resolutionOption.size.y, resolutionOption.supportedRefreshRates[this.refreshRateDropdown.value]));
		}

		// Token: 0x04002631 RID: 9777
		public MPDropdown resolutionDropdown;

		// Token: 0x04002632 RID: 9778
		public MPDropdown refreshRateDropdown;

		// Token: 0x04002633 RID: 9779
		private Resolution[] resolutions;

		// Token: 0x04002634 RID: 9780
		private ResolutionControl.ResolutionOption[] resolutionOptions = Array.Empty<ResolutionControl.ResolutionOption>();

		// Token: 0x02000623 RID: 1571
		private class ResolutionOption
		{
			// Token: 0x06002351 RID: 9041 RVA: 0x000A9354 File Offset: 0x000A7554
			public string GenerateDisplayString()
			{
				return string.Format("{0}x{1} <color=#7F7F7F>({2})</color>", this.size.x, this.size.y, string.Join("|", from v in this.supportedRefreshRates
				select v.ToString()));
			}

			// Token: 0x04002635 RID: 9781
			public Vector2Int size;

			// Token: 0x04002636 RID: 9782
			public readonly List<int> supportedRefreshRates = new List<int>();
		}
	}
}
