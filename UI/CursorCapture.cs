using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005C7 RID: 1479
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	public class CursorCapture : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x0600213F RID: 8511 RVA: 0x000025F6 File Offset: 0x000007F6
		public void OnPointerClick(PointerEventData eventData)
		{
		}
	}
}
