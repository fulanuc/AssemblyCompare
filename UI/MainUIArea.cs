using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200060B RID: 1547
	[RequireComponent(typeof(RectTransform))]
	[ExecuteInEditMode]
	public class MainUIArea : MonoBehaviour
	{
		// Token: 0x060022DD RID: 8925 RVA: 0x000196BD File Offset: 0x000178BD
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.parentRectTransform = this.rectTransform.parent.GetComponent<RectTransform>();
		}

		// Token: 0x060022DE RID: 8926 RVA: 0x000A71C0 File Offset: 0x000A53C0
		private void Update()
		{
			Rect rect = this.parentRectTransform.rect;
			float num = rect.width * 0.05f;
			float num2 = rect.height * 0.05f;
			this.rectTransform.offsetMin = new Vector2(num, num2);
			this.rectTransform.offsetMax = new Vector2(-num, -num2);
		}

		// Token: 0x040025AE RID: 9646
		private RectTransform rectTransform;

		// Token: 0x040025AF RID: 9647
		private RectTransform parentRectTransform;
	}
}
