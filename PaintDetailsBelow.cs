using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000385 RID: 901
	public class PaintDetailsBelow : MonoBehaviour
	{
		// Token: 0x060012DA RID: 4826 RVA: 0x0000E6BA File Offset: 0x0000C8BA
		public void OnEnable()
		{
			PaintDetailsBelow.painterList.Add(this);
		}

		// Token: 0x060012DB RID: 4827 RVA: 0x0000E6C7 File Offset: 0x0000C8C7
		public void OnDisable()
		{
			PaintDetailsBelow.painterList.Remove(this);
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x0000E6D5 File Offset: 0x0000C8D5
		public static List<PaintDetailsBelow> GetPainters()
		{
			return PaintDetailsBelow.painterList;
		}

		// Token: 0x04001687 RID: 5767
		[Tooltip("Influence radius in world coordinates")]
		public float influenceOuter = 2f;

		// Token: 0x04001688 RID: 5768
		public float influenceInner = 1f;

		// Token: 0x04001689 RID: 5769
		[Tooltip("Which detail layer")]
		public int layer;

		// Token: 0x0400168A RID: 5770
		[Tooltip("Density, from 0-1")]
		public float density = 0.5f;

		// Token: 0x0400168B RID: 5771
		public float densityPower = 1f;

		// Token: 0x0400168C RID: 5772
		private int buffer = 1;

		// Token: 0x0400168D RID: 5773
		private float steepnessMax = 30f;

		// Token: 0x0400168E RID: 5774
		private static List<PaintDetailsBelow> painterList = new List<PaintDetailsBelow>();
	}
}
