using System;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000408 RID: 1032
	public class TimerHologramContent : MonoBehaviour
	{
		// Token: 0x0600172A RID: 5930 RVA: 0x00078DD0 File Offset: 0x00076FD0
		private void FixedUpdate()
		{
			if (this.targetTextMesh)
			{
				int num = Mathf.FloorToInt(this.displayValue);
				int num2 = Mathf.FloorToInt((this.displayValue - (float)num) * 100f);
				this.targetTextMesh.text = string.Format("{0:D}.{1:D2}", num, num2);
			}
		}

		// Token: 0x04001A23 RID: 6691
		public float displayValue;

		// Token: 0x04001A24 RID: 6692
		public TextMeshPro targetTextMesh;
	}
}
