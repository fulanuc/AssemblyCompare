using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000213 RID: 531
	public struct CharacterAnimParamAvailability
	{
		// Token: 0x06000A56 RID: 2646 RVA: 0x00048148 File Offset: 0x00046348
		public static CharacterAnimParamAvailability FromAnimator(Animator animator)
		{
			return new CharacterAnimParamAvailability
			{
				isMoving = Util.HasAnimationParameter(AnimationParameters.isMoving, animator),
				turnAngle = Util.HasAnimationParameter(AnimationParameters.turnAngle, animator),
				isGrounded = Util.HasAnimationParameter(AnimationParameters.isGrounded, animator),
				mainRootPlaybackRate = Util.HasAnimationParameter(AnimationParameters.mainRootPlaybackRate, animator),
				forwardSpeed = Util.HasAnimationParameter(AnimationParameters.forwardSpeed, animator),
				rightSpeed = Util.HasAnimationParameter(AnimationParameters.rightSpeed, animator),
				upSpeed = Util.HasAnimationParameter(AnimationParameters.upSpeed, animator),
				walkSpeed = Util.HasAnimationParameter(AnimationParameters.walkSpeed, animator),
				isSprinting = Util.HasAnimationParameter(AnimationParameters.isSprinting, animator)
			};
		}

		// Token: 0x04000DC5 RID: 3525
		public bool isMoving;

		// Token: 0x04000DC6 RID: 3526
		public bool turnAngle;

		// Token: 0x04000DC7 RID: 3527
		public bool isGrounded;

		// Token: 0x04000DC8 RID: 3528
		public bool mainRootPlaybackRate;

		// Token: 0x04000DC9 RID: 3529
		public bool forwardSpeed;

		// Token: 0x04000DCA RID: 3530
		public bool rightSpeed;

		// Token: 0x04000DCB RID: 3531
		public bool upSpeed;

		// Token: 0x04000DCC RID: 3532
		public bool walkSpeed;

		// Token: 0x04000DCD RID: 3533
		public bool isSprinting;
	}
}
