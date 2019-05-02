using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x0200066C RID: 1644
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LabelSkinController : BaseSkinController
	{
		// Token: 0x060024F1 RID: 9457 RVA: 0x0001AEE5 File Offset: 0x000190E5
		protected new void Awake()
		{
			this.label = base.GetComponent<TextMeshProUGUI>();
			base.Awake();
		}

		// Token: 0x060024F2 RID: 9458 RVA: 0x000AE190 File Offset: 0x000AC390
		protected override void OnSkinUI()
		{
			switch (this.labelType)
			{
			case LabelSkinController.LabelType.Default:
				this.skinData.bodyTextStyle.Apply(this.label, this.useRecommendedAlignment);
				return;
			case LabelSkinController.LabelType.Header:
				this.skinData.headerTextStyle.Apply(this.label, this.useRecommendedAlignment);
				return;
			case LabelSkinController.LabelType.Detail:
				this.skinData.detailTextStyle.Apply(this.label, this.useRecommendedAlignment);
				return;
			default:
				return;
			}
		}

		// Token: 0x040027A5 RID: 10149
		public LabelSkinController.LabelType labelType;

		// Token: 0x040027A6 RID: 10150
		public bool useRecommendedAlignment = true;

		// Token: 0x040027A7 RID: 10151
		private TextMeshProUGUI label;

		// Token: 0x0200066D RID: 1645
		public enum LabelType
		{
			// Token: 0x040027A9 RID: 10153
			Default,
			// Token: 0x040027AA RID: 10154
			Header,
			// Token: 0x040027AB RID: 10155
			Detail
		}
	}
}
