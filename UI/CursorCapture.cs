using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005D9 RID: 1497
	[RequireComponent(typeof(Graphic))]
	[RequireComponent(typeof(RectTransform))]
	public class CursorCapture : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x060021D0 RID: 8656 RVA: 0x000025DA File Offset: 0x000007DA
		public void OnPointerClick(PointerEventData eventData)
		{
		}
	}
}
