using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000212 RID: 530
	public struct CharacterAnimatorWalkParamCalculator
	{
		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000A55 RID: 2645 RVA: 0x00008495 File Offset: 0x00006695
		// (set) Token: 0x06000A56 RID: 2646 RVA: 0x0000849D File Offset: 0x0000669D
		public Vector2 animatorWalkSpeed { get; private set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000A57 RID: 2647 RVA: 0x000084A6 File Offset: 0x000066A6
		// (set) Token: 0x06000A58 RID: 2648 RVA: 0x000084AE File Offset: 0x000066AE
		public float remainingTurnAngle { get; private set; }

		// Token: 0x06000A59 RID: 2649 RVA: 0x000482FC File Offset: 0x000464FC
		public void Update(Vector3 worldMoveVector, Vector3 animatorForward, in BodyAnimatorSmoothingParameters.SmoothingParameters smoothingParameters, float deltaTime)
		{
			ref Vector3 ptr = ref animatorForward;
			Vector3 rhs = Vector3.Cross(Vector3.up, ptr);
			float x = Vector3.Dot(worldMoveVector, ptr);
			float y = Vector3.Dot(worldMoveVector, rhs);
			Vector2 to = new Vector2(x, y);
			float magnitude = to.magnitude;
			float num = (magnitude > 0f) ? Vector2.SignedAngle(Vector2.right, to) : 0f;
			float magnitude2 = this.animatorWalkSpeed.magnitude;
			float current = (magnitude2 > 0f) ? Vector2.SignedAngle(Vector2.right, this.animatorWalkSpeed) : 0f;
			float d = Mathf.SmoothDamp(magnitude2, magnitude, ref this.animatorReferenceMagnitudeVelocity, smoothingParameters.walkMagnitudeSmoothDamp, float.PositiveInfinity, deltaTime);
			float num2 = Mathf.SmoothDampAngle(current, num, ref this.animatorReferenceAngleVelocity, smoothingParameters.walkAngleSmoothDamp, float.PositiveInfinity, deltaTime);
			this.remainingTurnAngle = num2 - num;
			this.animatorWalkSpeed = new Vector2(Mathf.Cos(num2 * 0.0174532924f), Mathf.Sin(num2 * 0.0174532924f)) * d;
		}

		// Token: 0x04000DC7 RID: 3527
		private float animatorReferenceMagnitudeVelocity;

		// Token: 0x04000DC8 RID: 3528
		private float animatorReferenceAngleVelocity;
	}
}
