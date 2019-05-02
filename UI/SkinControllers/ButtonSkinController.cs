using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x0200066B RID: 1643
	[RequireComponent(typeof(Button))]
	public class ButtonSkinController : BaseSkinController
	{
		// Token: 0x060024E7 RID: 9447 RVA: 0x0001AE46 File Offset: 0x00019046
		protected new void Awake()
		{
			this.button = base.GetComponent<Button>();
			this.layoutElement = base.GetComponent<LayoutElement>();
			base.Awake();
		}

		// Token: 0x060024E8 RID: 9448 RVA: 0x0001AE66 File Offset: 0x00019066
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onUpdate += ButtonSkinController.StaticUpdate;
		}

		// Token: 0x060024E9 RID: 9449 RVA: 0x0001AE79 File Offset: 0x00019079
		private void OnEnable()
		{
			ButtonSkinController.instancesList.Add(this);
		}

		// Token: 0x060024EA RID: 9450 RVA: 0x0001AE86 File Offset: 0x00019086
		private void OnDisable()
		{
			ButtonSkinController.instancesList.Remove(this);
		}

		// Token: 0x060024EB RID: 9451 RVA: 0x000ADFF8 File Offset: 0x000AC1F8
		private static void StaticUpdate()
		{
			foreach (ButtonSkinController buttonSkinController in ButtonSkinController.instancesList)
			{
				buttonSkinController.UpdateLabelStyle(ref buttonSkinController.skinData.buttonStyle);
			}
		}

		// Token: 0x060024EC RID: 9452 RVA: 0x000AE054 File Offset: 0x000AC254
		private void UpdateLabelStyle(ref UISkinData.ButtonStyle buttonStyle)
		{
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

		// Token: 0x060024ED RID: 9453 RVA: 0x0001AE94 File Offset: 0x00019094
		protected override void OnSkinUI()
		{
			this.ApplyButtonStyle(ref this.skinData.buttonStyle);
		}

		// Token: 0x060024EE RID: 9454 RVA: 0x000AE0B0 File Offset: 0x000AC2B0
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
			this.UpdateLabelStyle(ref buttonStyle);
		}

		// Token: 0x0400279C RID: 10140
		private static readonly List<ButtonSkinController> instancesList = new List<ButtonSkinController>();

		// Token: 0x0400279D RID: 10141
		private Button button;

		// Token: 0x0400279E RID: 10142
		public bool useRecommendedButtonWidth = true;

		// Token: 0x0400279F RID: 10143
		public bool useRecommendedButtonHeight = true;

		// Token: 0x040027A0 RID: 10144
		public bool useRecommendedImage = true;

		// Token: 0x040027A1 RID: 10145
		public bool useRecommendedMaterial = true;

		// Token: 0x040027A2 RID: 10146
		public bool useRecommendedAlignment = true;

		// Token: 0x040027A3 RID: 10147
		public bool useRecommendedLabel = true;

		// Token: 0x040027A4 RID: 10148
		private LayoutElement layoutElement;
	}
}
