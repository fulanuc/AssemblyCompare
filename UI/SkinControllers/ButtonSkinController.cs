using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x02000659 RID: 1625
	[RequireComponent(typeof(Button))]
	public class ButtonSkinController : BaseSkinController
	{
		// Token: 0x06002456 RID: 9302 RVA: 0x0001A74D File Offset: 0x0001894D
		protected new void Awake()
		{
			this.button = base.GetComponent<Button>();
			this.layoutElement = base.GetComponent<LayoutElement>();
			base.Awake();
		}

		// Token: 0x06002457 RID: 9303 RVA: 0x0001A76D File Offset: 0x0001896D
		protected override void OnSkinUI()
		{
			this.ApplyButtonStyle(ref this.skinData.buttonStyle);
		}

		// Token: 0x06002458 RID: 9304 RVA: 0x000AC978 File Offset: 0x000AAB78
		private void ApplyButtonStyle(ref UISkinData.ButtonStyle buttonStyle)
		{
			if (this.useRecommendedMaterial)
			{
				this.button.image.material = buttonStyle.material;
			}
			this.button.colors = buttonStyle.colors;
			if (this.useRecommendedImage)
			{
				this.button.image.sprite = buttonStyle.sprite;
			}
			if (this.useRecommendedButtonWidth)
			{
				((RectTransform)base.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonStyle.recommendedWidth);
			}
			if (this.useRecommendedButtonHeight)
			{
				((RectTransform)base.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonStyle.recommendedHeight);
			}
			if (this.layoutElement)
			{
				if (this.useRecommendedButtonWidth)
				{
					this.layoutElement.preferredWidth = buttonStyle.recommendedWidth;
				}
				if (this.useRecommendedButtonHeight)
				{
					this.layoutElement.preferredHeight = buttonStyle.recommendedHeight;
				}
			}
			if (this.useRecommendedLabel)
			{
				TextMeshProUGUI componentInChildren = this.button.GetComponentInChildren<TextMeshProUGUI>();
				if (componentInChildren)
				{
					if (this.button.interactable)
					{
						buttonStyle.interactableTextStyle.Apply(componentInChildren, this.useRecommendedAlignment);
						return;
					}
					buttonStyle.disabledTextStyle.Apply(componentInChildren, this.useRecommendedAlignment);
				}
			}
		}

		// Token: 0x04002741 RID: 10049
		private Button button;

		// Token: 0x04002742 RID: 10050
		public bool useRecommendedButtonWidth = true;

		// Token: 0x04002743 RID: 10051
		public bool useRecommendedButtonHeight = true;

		// Token: 0x04002744 RID: 10052
		public bool useRecommendedImage = true;

		// Token: 0x04002745 RID: 10053
		public bool useRecommendedMaterial = true;

		// Token: 0x04002746 RID: 10054
		public bool useRecommendedAlignment = true;

		// Token: 0x04002747 RID: 10055
		public bool useRecommendedLabel = true;

		// Token: 0x04002748 RID: 10056
		private LayoutElement layoutElement;
	}
}
