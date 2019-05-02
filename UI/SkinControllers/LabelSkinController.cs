using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x0200065A RID: 1626
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LabelSkinController : BaseSkinController
	{
		// Token: 0x0600245A RID: 9306 RVA: 0x0001A7B2 File Offset: 0x000189B2
		protected new void Awake()
		{
			this.label = base.GetComponent<TextMeshProUGUI>();
			base.Awake();
		}

		// Token: 0x0600245B RID: 9307 RVA: 0x000ACAA0 File Offset: 0x000AACA0
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

		// Token: 0x04002749 RID: 10057
		public LabelSkinController.LabelType labelType;

		// Token: 0x0400274A RID: 10058
		public bool useRecommendedAlignment = true;

		// Token: 0x0400274B RID: 10059
		private TextMeshProUGUI label;

		// Token: 0x0200065B RID: 1627
		public enum LabelType
		{
			// Token: 0x0400274D RID: 10061
			Default,
			// Token: 0x0400274E RID: 10062
			Header,
			// Token: 0x0400274F RID: 10063
			Detail
		}
	}
}
