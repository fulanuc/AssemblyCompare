using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000621 RID: 1569
	public class RectTransformDimensionsChangeEvent : MonoBehaviour
	{
		// Token: 0x14000059 RID: 89
		// (add) Token: 0x06002343 RID: 9027 RVA: 0x000A9020 File Offset: 0x000A7220
		// (remove) Token: 0x06002344 RID: 9028 RVA: 0x000A9058 File Offset: 0x000A7258
		public event Action onRectTransformDimensionsChange;

		// Token: 0x06002345 RID: 9029 RVA: 0x00019B53 File Offset: 0x00017D53
		private void OnRectTransformDimensionsChange()
		{
			if (this.onRectTransformDimensionsChange != null)
			{
				this.onRectTransformDimensionsChange();
			}
		}
	}
}
