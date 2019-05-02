using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000213 RID: 531
	public struct CharacterAnimParamAvailability
	{
		// Token: 0x06000A5A RID: 2650 RVA: 0x00048404 File Offset: 0x00046604
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

		// Token: 0x04000DC9 RID: 3529
		public bool isMoving;

		// Token: 0x04000DCA RID: 3530
		public bool turnAngle;

		// Token: 0x04000DCB RID: 3531
		public bool isGrounded;

		// Token: 0x04000DCC RID: 3532
		public bool mainRootPlaybackRate;

		// Token: 0x04000DCD RID: 3533
		public bool forwardSpeed;

		// Token: 0x04000DCE RID: 3534
		public bool rightSpeed;

		// Token: 0x04000DCF RID: 3535
		public bool upSpeed;

		// Token: 0x04000DD0 RID: 3536
		public bool walkSpeed;

		// Token: 0x04000DD1 RID: 3537
		public bool isSprinting;
	}
}
