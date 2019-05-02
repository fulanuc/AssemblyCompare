﻿using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002D4 RID: 724
	public class DirectorPlacementRule
	{
		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000E92 RID: 3730 RVA: 0x0000B3F2 File Offset: 0x000095F2
		public Vector3 targetPosition
		{
			get
			{
				if (!this.spawnOnTarget)
				{
					return this.position;
				}
				return this.spawnOnTarget.position;
			}
		}

		// Token: 0x04001287 RID: 4743
		public Transform spawnOnTarget;

		// Token: 0x04001288 RID: 4744
		public Vector3 position;

		// Token: 0x04001289 RID: 4745
		public DirectorPlacementRule.PlacementMode placementMode;

		// Token: 0x0400128A RID: 4746
		public float minDistance;

		// Token: 0x0400128B RID: 4747
		public float maxDistance;

		// Token: 0x0400128C RID: 4748
		public bool preventOverhead;

		// Token: 0x020002D5 RID: 725
		public enum PlacementMode
		{
			// Token: 0x0400128E RID: 4750
			Direct,
			// Token: 0x0400128F RID: 4751
			Approximate,
			// Token: 0x04001290 RID: 4752
			ApproximateSimple,
			// Token: 0x04001291 RID: 4753
			NearestNode,
			// Token: 0x04001292 RID: 4754
			Random
		}
	}
}
