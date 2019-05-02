using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005D3 RID: 1491
	[RequireComponent(typeof(RectTransform))]
	public class DragResize : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler
	{
		// Token: 0x06002175 RID: 8565 RVA: 0x00018582 File Offset: 0x00016782
		private void OnAwake()
		{
			this.rectTransform = (RectTransform)base.transform;
		}

		// Token: 0x06002176 RID: 8566 RVA: 0x00018595 File Offset: 0x00016795
		public void OnDrag(PointerEventData eventData)
		{
			this.UpdateDrag(eventData);
		}

		// Token: 0x06002177 RID: 8567 RVA: 0x0001859E File Offset: 0x0001679E
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (this.targetTransform)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.targetTransform, eventData.position, eventData.pressEventCamera, out this.grabPoint);
			}
		}

		// Token: 0x06002178 RID: 8568 RVA: 0x000A1BDC File Offset: 0x0009FDDC
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
			this.grabPoint = a;
			vector.y = -vector.y;
			this.targetTransform.sizeDelta = Vector2.Max(this.targetTransform.sizeDelta + vector, this.minSize);
		}

		// Token: 0x04002419 RID: 9241
		public RectTransform targetTransform;

		// Token: 0x0400241A RID: 9242
		public Vector2 minSize;

		// Token: 0x0400241B RID: 9243
		private Vector2 grabPoint;

		// Token: 0x0400241C RID: 9244
		private RectTransform rectTransform;
	}
}
