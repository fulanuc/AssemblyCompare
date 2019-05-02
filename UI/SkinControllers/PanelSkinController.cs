using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x0200065C RID: 1628
	[RequireComponent(typeof(Image))]
	public class PanelSkinController : BaseSkinController
	{
		// Token: 0x0600245D RID: 9309 RVA: 0x0001A7D5 File Offset: 0x000189D5
		protected new void Awake()
		{
			this.image = base.GetComponent<Image>();
			base.Awake();
		}

		// Token: 0x0600245E RID: 9310 RVA: 0x000ACB20 File Offset: 0x000AAD20
		protected override void OnSkinUI()
		{
			switch (this.panelType)
			{
			case PanelSkinController.PanelType.Default:
				this.skinData.mainPanelStyle.Apply(this.image);
				return;
			case PanelSkinController.PanelType.Header:
				this.skinData.headerPanelStyle.Apply(this.image);
				return;
			case PanelSkinController.PanelType.Detail:
				this.skinData.detailPanelStyle.Apply(this.image);
				return;
			default:
				return;
			}
		}

		// Token: 0x04002750 RID: 10064
		public PanelSkinController.PanelType panelType;

		// Token: 0x04002751 RID: 10065
		private Image image;

		// Token: 0x0200065D RID: 1629
		public enum PanelType
		{
			// Token: 0x04002753 RID: 10067
			Default,
			// Token: 0x04002754 RID: 10068
			Header,
			// Token: 0x04002755 RID: 10069
			Detail
		}
	}
}
