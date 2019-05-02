using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x0200065E RID: 1630
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollRectSkinController : BaseSkinController
	{
		// Token: 0x06002460 RID: 9312 RVA: 0x0001A7F1 File Offset: 0x000189F1
		protected new void Awake()
		{
			this.scrollRect = base.GetComponent<ScrollRect>();
			base.Awake();
		}

		// Token: 0x06002461 RID: 9313 RVA: 0x000ACB8C File Offset: 0x000AAD8C
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

		// Token: 0x06002462 RID: 9314 RVA: 0x000ACC04 File Offset: 0x000AAE04
		private void SkinScrollbar(Scrollbar scrollbar)
		{
			this.skinData.scrollRectStyle.scrollbarBackgroundStyle.Apply(scrollbar.GetComponent<Image>());
			scrollbar.colors = this.skinData.scrollRectStyle.scrollbarHandleColors;
			scrollbar.handleRect.GetComponent<Image>().sprite = this.skinData.scrollRectStyle.scrollbarHandleImage;
		}

		// Token: 0x04002756 RID: 10070
		private ScrollRect scrollRect;
	}
}
