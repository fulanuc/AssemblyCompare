using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005E5 RID: 1509
	[RequireComponent(typeof(RectTransform))]
	public class DragResize : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler
	{
		// Token: 0x06002206 RID: 8710 RVA: 0x00018C7C File Offset: 0x00016E7C
		private void OnAwake()
		{
			this.rectTransform = (RectTransform)base.transform;
		}

		// Token: 0x06002207 RID: 8711 RVA: 0x00018C8F File Offset: 0x00016E8F
		public void OnDrag(PointerEventData eventData)
		{
			this.UpdateDrag(eventData);
		}

		// Token: 0x06002208 RID: 8712 RVA: 0x00018C98 File Offset: 0x00016E98
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (this.targetTransform)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.targetTransform, eventData.position, eventData.pressEventCamera, out this.grabPoint);
			}
		}

		// Token: 0x06002209 RID: 8713 RVA: 0x000A31B0 File Offset: 0x000A13B0
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

		// Token: 0x0400246D RID: 9325
		public RectTransform targetTransform;

		// Token: 0x0400246E RID: 9326
		public Vector2 minSize;

		// Token: 0x0400246F RID: 9327
		private Vector2 grabPoint;

		// Token: 0x04002470 RID: 9328
		private RectTransform rectTransform;
	}
}
