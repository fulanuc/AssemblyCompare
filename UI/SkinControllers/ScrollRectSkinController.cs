using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x02000670 RID: 1648
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollRectSkinController : BaseSkinController
	{
		// Token: 0x060024F7 RID: 9463 RVA: 0x0001AF24 File Offset: 0x00019124
		protected new void Awake()
		{
			this.scrollRect = base.GetComponent<ScrollRect>();
			base.Awake();
		}

		// Token: 0x060024F8 RID: 9464 RVA: 0x000AE27C File Offset: 0x000AC47C
		protected override void OnSkinUI()
		{
			Image component = base.GetComponent<Image>();
			if (component)
			{
				this.skinData.scrollRectStyle.backgroundPanelStyle.Apply(component);
			}
			if (this.scrollRect.verticalScrollbar)
			{
				this.SkinScrollbar(this.scrollRect.verticalScrollbar);
			}
			if (this.scrollRect.horizontalScrollbar)
			{
				this.SkinScrollbar(this.scrollRect.horizontalScrollbar);
			}
		}

		// Token: 0x060024F9 RID: 9465 RVA: 0x000AE2F4 File Offset: 0x000AC4F4
		private void SkinScrollbar(Scrollbar scrollbar)
		{
			this.skinData.scrollRectStyle.scrollbarBackgroundStyle.Apply(scrollbar.GetComponent<Image>());
			scrollbar.colors = this.skinData.scrollRectStyle.scrollbarHandleColors;
			scrollbar.handleRect.GetComponent<Image>().sprite = this.skinData.scrollRectStyle.scrollbarHandleImage;
		}

		// Token: 0x040027B2 RID: 10162
		private ScrollRect scrollRect;
	}
}
