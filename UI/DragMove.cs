using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005E4 RID: 1508
	[RequireComponent(typeof(RectTransform))]
	public class DragMove : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler
	{
		// Token: 0x06002201 RID: 8705 RVA: 0x00018C33 File Offset: 0x00016E33
		private void OnAwake()
		{
			this.rectTransform = (RectTransform)base.transform;
		}

		// Token: 0x06002202 RID: 8706 RVA: 0x00018C46 File Offset: 0x00016E46
		public void OnDrag(PointerEventData eventData)
		{
			this.UpdateDrag(eventData);
		}

		// Token: 0x06002203 RID: 8707 RVA: 0x00018C4F File Offset: 0x00016E4F
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (this.targetTransform)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.targetTransform, eventData.position, eventData.pressEventCamera, out this.grabPoint);
			}
		}

		// Token: 0x06002204 RID: 8708 RVA: 0x000A3138 File Offset: 0x000A1338
		private void UpdateDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			if (!this.targetTransform)
			{
				return;
			}
			Vector2 a;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.targetTransform, eventData.position, eventData.pressEventCamera, out a);
			Vector2 vector = a - this.grabPoint;
			this.targetTransform.localPosition += new Vector3(vector.x, vector.y, 0f);
		}

		// Token: 0x0400246A RID: 9322
		public RectTransform targetTransform;

		// Token: 0x0400246B RID: 9323
		private Vector2 grabPoint;

		// Token: 0x0400246C RID: 9324
		private RectTransform rectTransform;
	}
}
