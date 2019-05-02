using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000380 RID: 896
	public class PaintDetailsBelow : MonoBehaviour
	{
		// Token: 0x060012BA RID: 4794 RVA: 0x0000E52F File Offset: 0x0000C72F
		public void OnEnable()
		{
			PaintDetailsBelow.painterList.Add(this);
		}

		// Token: 0x060012BB RID: 4795 RVA: 0x0000E53C File Offset: 0x0000C73C
		public void OnDisable()
		{
			PaintDetailsBelow.painterList.Remove(this);
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x0000E54A File Offset: 0x0000C74A
		public static List<PaintDetailsBelow> GetPainters()
		{
			return PaintDetailsBelow.painterList;
		}

		// Token: 0x0400166B RID: 5739
		[Tooltip("Influence radius in world coordinates")]
		public float influenceOuter = 2f;

		// Token: 0x0400166C RID: 5740
		public float influenceInner = 1f;

		// Token: 0x0400166D RID: 5741
		[Tooltip("Which detail layer")]
		public int layer;

		// Token: 0x0400166E RID: 5742
		[Tooltip("Density, from 0-1")]
		public float density = 0.5f;

		// Token: 0x0400166F RID: 5743
		public float densityPower = 1f;

		// Token: 0x04001670 RID: 5744
		private int buffer = 1;

		// Token: 0x04001671 RID: 5745
		private float steepnessMax = 30f;

		// Token: 0x04001672 RID: 5746
		private static List<PaintDetailsBelow> painterList = new List<PaintDetailsBelow>();
	}
}
