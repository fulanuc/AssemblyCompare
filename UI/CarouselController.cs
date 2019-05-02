using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B3 RID: 1459
	public class CarouselController : BaseSettingsControl
	{
		// Token: 0x060020B9 RID: 8377 RVA: 0x0009E92C File Offset: 0x0009CB2C
		protected override void OnUpdateControls()
		{
			string currentValue = base.GetCurrentValue();
			bool flag = false;
			for (int i = 0; i < this.choices.Length; i++)
			{
				if (this.choices[i].convarValue == currentValue)
				{
					flag = true;
					Debug.LogFormat("Matching value {0}, carousel entry index {1} detected for setting {2}", new object[]
					{
						currentValue,
						i,
						this.settingName
					});
					this.selectionIndex = i;
					this.UpdateFromSelectionIndex();
					break;
				}
			}
			if (!flag)
			{
				Debug.LogWarningFormat("Custom value of {0} detected for setting {1}", new object[]
				{
					currentValue,
					this.settingName
				});
				this.optionalText.GetComponent<LanguageTextMeshController>().token = "OPTION_CUSTOM";
			}
		}

		// Token: 0x060020BA RID: 8378 RVA: 0x0009E9DC File Offset: 0x0009CBDC
		public void MoveCarousel(int direction)
		{
			this.selectionIndex = Mathf.Clamp(this.selectionIndex + direction, 0, this.choices.Length - 1);
			this.UpdateFromSelectionIndex();
			base.SubmitSetting(this.choices[this.selectionIndex].convarValue);
		}

		// Token: 0x060020BB RID: 8379 RVA: 0x00017D5A File Offset: 0x00015F5A
		public void BoolCarousel()
		{
			this.selectionIndex = ((this.selectionIndex == 0) ? 1 : 0);
			this.UpdateFromSelectionIndex();
			base.SubmitSetting(this.choices[this.selectionIndex].convarValue);
		}

		// Token: 0x060020BC RID: 8380 RVA: 0x0009EA2C File Offset: 0x0009CC2C
		private void UpdateFromSelectionIndex()
		{
			CarouselController.Choice choice = this.choices[this.selectionIndex];
			if (this.optionalText)
			{
				this.optionalText.GetComponent<LanguageTextMeshController>().token = choice.suboptionDisplayToken;
			}
			if (this.optionalImage)
			{
				this.optionalImage.sprite = choice.customSprite;
			}
		}

		// Token: 0x060020BD RID: 8381 RVA: 0x0009EA8C File Offset: 0x0009CC8C
		private void Update()
		{
			bool active = true;
			bool active2 = true;
			if (this.selectionIndex == 0)
			{
				active = false;
			}
			else if (this.selectionIndex == this.choices.Length - 1)
			{
				active2 = false;
			}
			if (this.leftArrowButton)
			{
				this.leftArrowButton.SetActive(active);
			}
			if (this.rightArrowButton)
			{
				this.rightArrowButton.SetActive(active2);
			}
		}

		// Token: 0x04002334 RID: 9012
		public GameObject leftArrowButton;

		// Token: 0x04002335 RID: 9013
		public GameObject rightArrowButton;

		// Token: 0x04002336 RID: 9014
		public GameObject boolButton;

		// Token: 0x04002337 RID: 9015
		public Image optionalImage;

		// Token: 0x04002338 RID: 9016
		public TextMeshProUGUI optionalText;

		// Token: 0x04002339 RID: 9017
		public CarouselController.Choice[] choices;

		// Token: 0x0400233A RID: 9018
		private int selectionIndex;

		// Token: 0x020005B4 RID: 1460
		[Serializable]
		public struct Choice
		{
			// Token: 0x0400233B RID: 9019
			public string suboptionDisplayToken;

			// Token: 0x0400233C RID: 9020
			public string convarValue;

			// Token: 0x0400233D RID: 9021
			public Sprite customSprite;
		}
	}
}
