using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005D2 RID: 1490
	[RequireComponent(typeof(RectTransform))]
	public class DragMove : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler
	{
		// Token: 0x06002170 RID: 8560 RVA: 0x00018539 File Offset: 0x00016739
		private void OnAwake()
		{
			this.rectTransform = (RectTransform)base.transform;
		}

		// Token: 0x06002171 RID: 8561 RVA: 0x0001854C File Offset: 0x0001674C
		public void OnDrag(PointerEventData eventData)
		{
			this.UpdateDrag(eventData);
		}

		// Token: 0x06002172 RID: 8562 RVA: 0x00018555 File Offset: 0x00016755
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (this.targetTransform)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.targetTransform, eventData.position, eventData.pressEventCamera, out this.grabPoint);
			}
		}

		// Token: 0x06002173 RID: 8563 RVA: 0x000A1B64 File Offset: 0x0009FD64
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

		// Token: 0x04002416 RID: 9238
		public RectTransform targetTransform;

		// Token: 0x04002417 RID: 9239
		private Vector2 grabPoint;

		// Token: 0x04002418 RID: 9240
		private RectTransform rectTransform;
	}
}
