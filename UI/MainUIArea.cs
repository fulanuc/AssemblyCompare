using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005F9 RID: 1529
	[RequireComponent(typeof(RectTransform))]
	[ExecuteInEditMode]
	public class MainUIArea : MonoBehaviour
	{
		// Token: 0x0600224D RID: 8781 RVA: 0x00019010 File Offset: 0x00017210
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.parentRectTransform = this.rectTransform.parent.GetComponent<RectTransform>();
		}

		// Token: 0x0600224E RID: 8782 RVA: 0x000A5B44 File Offset: 0x000A3D44
		private void Update()
		{
			Rect rect = this.parentRectTransform.rect;
			float num = rect.width * 0.05f;
			float num2 = rect.height * 0.05f;
			this.rectTransform.offsetMin = new Vector2(num, num2);
			this.rectTransform.offsetMax = new Vector2(-num, -num2);
		}

		// Token: 0x04002553 RID: 9555
		private RectTransform rectTransform;

		// Token: 0x04002554 RID: 9556
		private RectTransform parentRectTransform;
	}
}
