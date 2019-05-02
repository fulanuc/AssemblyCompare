using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x0200062E RID: 1582
	public class SelectableDescriptionUpdater : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x0600238C RID: 9100 RVA: 0x00019EFE File Offset: 0x000180FE
		public void OnPointerExit(PointerEventData eventData)
		{
			this.languageTextMeshController.token = "";
		}

		// Token: 0x0600238D RID: 9101 RVA: 0x00019EFE File Offset: 0x000180FE
		public void OnDeselect(BaseEventData eventData)
		{
			this.languageTextMeshController.token = "";
		}

		// Token: 0x0600238E RID: 9102 RVA: 0x00019F10 File Offset: 0x00018110
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.languageTextMeshController.token = this.selectableDescriptionToken;
		}

		// Token: 0x0600238F RID: 9103 RVA: 0x00019F10 File Offset: 0x00018110
		public void OnSelect(BaseEventData eventData)
		{
			this.languageTextMeshController.token = this.selectableDescriptionToken;
		}

		// Token: 0x04002671 RID: 9841
		public LanguageTextMeshController languageTextMeshController;

		// Token: 0x04002672 RID: 9842
		public string selectableDescriptionToken;
	}
}
