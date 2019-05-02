using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200026C RID: 620
	public class BodyAnimatorSmoothingParameters : MonoBehaviour
	{
		// Token: 0x04000F8E RID: 3982
		public BodyAnimatorSmoothingParameters.SmoothingParameters smoothingParameters;

		// Token: 0x04000F8F RID: 3983
		public static BodyAnimatorSmoothingParameters.SmoothingParameters defaultParameters = new BodyAnimatorSmoothingParameters.SmoothingParameters
		{
			walkMagnitudeSmoothDamp = 0.2f,
			walkAngleSmoothDamp = 0.2f,
			forwardSpeedSmoothDamp = 0.1f,
			rightSpeedSmoothDamp = 0.1f,
			intoJumpTransitionTime = 0.05f,
			turnAngleSmoothDamp = 1f
		};

		// Token: 0x0200026D RID: 621
		[Serializable]
		public struct SmoothingParameters
		{
			// Token: 0x04000F90 RID: 3984
			public float walkMagnitudeSmoothDamp;

			// Token: 0x04000F91 RID: 3985
			public float walkAngleSmoothDamp;

			// Token: 0x04000F92 RID: 3986
			public float forwardSpeedSmoothDamp;

			// Token: 0x04000F93 RID: 3987
			public float rightSpeedSmoothDamp;

			// Token: 0x04000F94 RID: 3988
			public float intoJumpTransitionTime;

			// Token: 0x04000F95 RID: 3989
			public float turnAngleSmoothDamp;
		}
	}
}
