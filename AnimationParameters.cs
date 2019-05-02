using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001FE RID: 510
	public static class AnimationParameters
	{
		// Token: 0x04000D2F RID: 3375
		public static readonly int isMoving = Animator.StringToHash("isMoving");

		// Token: 0x04000D30 RID: 3376
		public static readonly int turnAngle = Animator.StringToHash("turnAngle");

		// Token: 0x04000D31 RID: 3377
		public static readonly int isGrounded = Animator.StringToHash("isGrounded");

		// Token: 0x04000D32 RID: 3378
		public static readonly int mainRootPlaybackRate = Animator.StringToHash("mainRootPlaybackRate");

		// Token: 0x04000D33 RID: 3379
		public static readonly int forwardSpeed = Animator.StringToHash("forwardSpeed");

		// Token: 0x04000D34 RID: 3380
		public static readonly int rightSpeed = Animator.StringToHash("rightSpeed");

		// Token: 0x04000D35 RID: 3381
		public static readonly int upSpeed = Animator.StringToHash("upSpeed");

		// Token: 0x04000D36 RID: 3382
		public static readonly int walkSpeed = Animator.StringToHash("walkSpeed");

		// Token: 0x04000D37 RID: 3383
		public static readonly int isSprinting = Animator.StringToHash("isSprinting");
	}
}
