using System;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000402 RID: 1026
	public class TimerHologramContent : MonoBehaviour
	{
		// Token: 0x060016E7 RID: 5863 RVA: 0x00078810 File Offset: 0x00076A10
		private void FixedUpdate()
		{
			if (this.targetTextMesh)
			{
				int num = Mathf.FloorToInt(this.displayValue);
				int num2 = Mathf.FloorToInt((this.displayValue - (float)num) * 100f);
				this.targetTextMesh.text = string.Format("{0:D}.{1:D2}", num, num2);
			}
		}

		// Token: 0x040019FA RID: 6650
		public float displayValue;

		// Token: 0x040019FB RID: 6651
		public TextMeshPro targetTextMesh;
	}
}
