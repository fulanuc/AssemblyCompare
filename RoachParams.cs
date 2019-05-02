using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200022D RID: 557
	[CreateAssetMenu]
	public class RoachParams : ScriptableObject
	{
		// Token: 0x04000E2E RID: 3630
		public float reorientTimerMin = 0.2f;

		// Token: 0x04000E2F RID: 3631
		public float reorientTimerMax = 0.5f;

		// Token: 0x04000E30 RID: 3632
		public float turnSpeed = 72f;

		// Token: 0x04000E31 RID: 3633
		public float acceleration = 400f;

		// Token: 0x04000E32 RID: 3634
		public float maxSpeed = 100f;

		// Token: 0x04000E33 RID: 3635
		public float backupDuration = 0.1f;

		// Token: 0x04000E34 RID: 3636
		public float wiggle = 720f;

		// Token: 0x04000E35 RID: 3637
		public float stepSize = 0.1f;

		// Token: 0x04000E36 RID: 3638
		public float minSimulationDuration = 3f;

		// Token: 0x04000E37 RID: 3639
		public float maxSimulationDuration = 7f;

		// Token: 0x04000E38 RID: 3640
		public float chanceToFinishOnBump = 0.5f;

		// Token: 0x04000E39 RID: 3641
		public float keyframeInterval = 0.06666667f;

		// Token: 0x04000E3A RID: 3642
		public float minReactionTime;

		// Token: 0x04000E3B RID: 3643
		public float maxReactionTime = 0.2f;

		// Token: 0x04000E3C RID: 3644
		public GameObject roachPrefab;
	}
}
