using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.UI
{
	// Token: 0x02000634 RID: 1588
	public class ResolutionControl : BaseSettingsControl
	{
		// Token: 0x060023D7 RID: 9175 RVA: 0x0001A236 File Offset: 0x00018436
		private static Vector2Int ResolutionToVector2Int(Resolution resolution)
		{
			return new Vector2Int(resolution.width, resolution.height);
		}

		// Token: 0x060023D8 RID: 9176 RVA: 0x0001A24B File Offset: 0x0001844B
		private ResolutionControl.ResolutionOption GetCurrentSelectedResolutionOption()
		{
			if (this.resolutionDropdown.value >= 0)
			{
				return this.resolutionOptions[this.resolutionDropdown.value];
			}
			return null;
		}

		// Token: 0x060023D9 RID: 9177 RVA: 0x000AA70C File Offset: 0x000A890C
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

		// Token: 0x060023DA RID: 9178 RVA: 0x000AA874 File Offset: 0x000A8A74
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

		// Token: 0x060023DB RID: 9179 RVA: 0x0001A26F File Offset: 0x0001846F
		protected new void Awake()
		{
			base.Awake();
			this.resolutionDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnResolutionDropdownValueChanged));
			this.refreshRateDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnRefreshRateDropdownValueChanged));
		}

		// Token: 0x060023DC RID: 9180 RVA: 0x0001A2AF File Offset: 0x000184AF
		protected new void OnEnable()
		{
			base.OnEnable();
			this.GenerateResolutionOptions();
		}

		// Token: 0x060023DD RID: 9181 RVA: 0x0001A2BD File Offset: 0x000184BD
		private void OnResolutionDropdownValueChanged(int newValue)
		{
			if (newValue < 0)
			{
				return;
			}
			this.GenerateRefreshRateOptions();
		}

		// Token: 0x060023DE RID: 9182 RVA: 0x0001A2CA File Offset: 0x000184CA
		private void OnRefreshRateDropdownValueChanged(int newValue)
		{
		}

		// Token: 0x060023DF RID: 9183 RVA: 0x000AA940 File Offset: 0x000A8B40
		public void SubmitCurrentValue()
		{
			if (this.resolutionDropdown.value == -1 || this.refreshRateDropdown.value == -1)
			{
				return;
			}
			ResolutionControl.ResolutionOption resolutionOption = this.resolutionOptions[this.resolutionDropdown.value];
			base.SubmitSetting(string.Format(CultureInfo.InvariantCulture, "{0}x{1}x{2}", resolutionOption.size.x, resolutionOption.size.y, resolutionOption.supportedRefreshRates[this.refreshRateDropdown.value]));
		}

		// Token: 0x0400268C RID: 9868
		public MPDropdown resolutionDropdown;

		// Token: 0x0400268D RID: 9869
		public MPDropdown refreshRateDropdown;

		// Token: 0x0400268E RID: 9870
		private Resolution[] resolutions;

		// Token: 0x0400268F RID: 9871
		private ResolutionControl.ResolutionOption[] resolutionOptions = Array.Empty<ResolutionControl.ResolutionOption>();

		// Token: 0x02000635 RID: 1589
		private class ResolutionOption
		{
			// Token: 0x060023E1 RID: 9185 RVA: 0x000AA9D0 File Offset: 0x000A8BD0
			public string GenerateDisplayString()
			{
				return string.Format("{0}x{1} <color=#7F7F7F>({2})</color>", this.size.x, this.size.y, string.Join("|", from v in this.supportedRefreshRates
				select v.ToString()));
			}

			// Token: 0x04002690 RID: 9872
			public Vector2Int size;

			// Token: 0x04002691 RID: 9873
			public readonly List<int> supportedRefreshRates = new List<int>();
		}
	}
}
