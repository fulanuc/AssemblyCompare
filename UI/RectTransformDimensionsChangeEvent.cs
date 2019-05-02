using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000633 RID: 1587
	public class RectTransformDimensionsChangeEvent : MonoBehaviour
	{
		// Token: 0x1400005C RID: 92
		// (add) Token: 0x060023D3 RID: 9171 RVA: 0x000AA69C File Offset: 0x000A889C
		// (remove) Token: 0x060023D4 RID: 9172 RVA: 0x000AA6D4 File Offset: 0x000A88D4
		public event Action onRectTransformDimensionsChange;

		// Token: 0x060023D5 RID: 9173 RVA: 0x0001A221 File Offset: 0x00018421
		private void OnRectTransformDimensionsChange()
		{
			if (this.onRectTransformDimensionsChange != null)
			{
				this.onRectTransformDimensionsChange();
			}
		}
	}
}
