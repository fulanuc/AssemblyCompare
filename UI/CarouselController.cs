using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005C5 RID: 1477
	public class CarouselController : BaseSettingsControl
	{
		// Token: 0x0600214A RID: 8522 RVA: 0x0009FF00 File Offset: 0x0009E100
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

		// Token: 0x0600214B RID: 8523 RVA: 0x0009FFB0 File Offset: 0x0009E1B0
		public void MoveCarousel(int direction)
		{
			this.selectionIndex = Mathf.Clamp(this.selectionIndex + direction, 0, this.choices.Length - 1);
			this.UpdateFromSelectionIndex();
			base.SubmitSetting(this.choices[this.selectionIndex].convarValue);
		}

		// Token: 0x0600214C RID: 8524 RVA: 0x00018454 File Offset: 0x00016654
		public void BoolCarousel()
		{
			this.selectionIndex = ((this.selectionIndex == 0) ? 1 : 0);
			this.UpdateFromSelectionIndex();
			base.SubmitSetting(this.choices[this.selectionIndex].convarValue);
		}

		// Token: 0x0600214D RID: 8525 RVA: 0x000A0000 File Offset: 0x0009E200
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

		// Token: 0x0600214E RID: 8526 RVA: 0x000A0060 File Offset: 0x0009E260
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

		// Token: 0x04002388 RID: 9096
		public GameObject leftArrowButton;

		// Token: 0x04002389 RID: 9097
		public GameObject rightArrowButton;

		// Token: 0x0400238A RID: 9098
		public GameObject boolButton;

		// Token: 0x0400238B RID: 9099
		public Image optionalImage;

		// Token: 0x0400238C RID: 9100
		public TextMeshProUGUI optionalText;

		// Token: 0x0400238D RID: 9101
		public CarouselController.Choice[] choices;

		// Token: 0x0400238E RID: 9102
		private int selectionIndex;

		// Token: 0x020005C6 RID: 1478
		[Serializable]
		public struct Choice
		{
			// Token: 0x0400238F RID: 9103
			public string suboptionDisplayToken;

			// Token: 0x04002390 RID: 9104
			public string convarValue;

			// Token: 0x04002391 RID: 9105
			public Sprite customSprite;
		}
	}
}
