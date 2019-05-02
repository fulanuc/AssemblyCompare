using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x0200066E RID: 1646
	[RequireComponent(typeof(Image))]
	public class PanelSkinController : BaseSkinController
	{
		// Token: 0x060024F4 RID: 9460 RVA: 0x0001AF08 File Offset: 0x00019108
		protected new void Awake()
		{
			this.image = base.GetComponent<Image>();
			base.Awake();
		}

		// Token: 0x060024F5 RID: 9461 RVA: 0x000AE210 File Offset: 0x000AC410
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

		// Token: 0x040027AC RID: 10156
		public PanelSkinController.PanelType panelType;

		// Token: 0x040027AD RID: 10157
		private Image image;

		// Token: 0x0200066F RID: 1647
		public enum PanelType
		{
			// Token: 0x040027AF RID: 10159
			Default,
			// Token: 0x040027B0 RID: 10160
			Header,
			// Token: 0x040027B1 RID: 10161
			Detail
		}
	}
}
