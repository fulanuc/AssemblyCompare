using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000640 RID: 1600
	public class SelectableDescriptionUpdater : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x0600241C RID: 9244 RVA: 0x0001A5CC File Offset: 0x000187CC
		public void OnPointerExit(PointerEventData eventData)
		{
			this.languageTextMeshController.token = "";
		}

		// Token: 0x0600241D RID: 9245 RVA: 0x0001A5CC File Offset: 0x000187CC
		public void OnDeselect(BaseEventData eventData)
		{
			this.languageTextMeshController.token = "";
		}

		// Token: 0x0600241E RID: 9246 RVA: 0x0001A5DE File Offset: 0x000187DE
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.languageTextMeshController.token = this.selectableDescriptionToken;
		}

		// Token: 0x0600241F RID: 9247 RVA: 0x0001A5DE File Offset: 0x000187DE
		public void OnSelect(BaseEventData eventData)
		{
			this.languageTextMeshController.token = this.selectableDescriptionToken;
		}

		// Token: 0x040026CC RID: 9932
		public LanguageTextMeshController languageTextMeshController;

		// Token: 0x040026CD RID: 9933
		public string selectableDescriptionToken;
	}
}
