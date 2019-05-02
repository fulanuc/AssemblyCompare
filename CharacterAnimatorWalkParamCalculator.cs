﻿using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000212 RID: 530
	public struct CharacterAnimatorWalkParamCalculator
	{
		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000A51 RID: 2641 RVA: 0x00008471 File Offset: 0x00006671
		// (set) Token: 0x06000A52 RID: 2642 RVA: 0x00008479 File Offset: 0x00006679
		public Vector2 animatorWalkSpeed { get; private set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000A53 RID: 2643 RVA: 0x00008482 File Offset: 0x00006682
		// (set) Token: 0x06000A54 RID: 2644 RVA: 0x0000848A File Offset: 0x0000668A
		public float remainingTurnAngle { get; private set; }

		// Token: 0x06000A55 RID: 2645 RVA: 0x00048040 File Offset: 0x00046240
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

		// Token: 0x04000DC3 RID: 3523
		private float animatorReferenceMagnitudeVelocity;

		// Token: 0x04000DC4 RID: 3524
		private float animatorReferenceAngleVelocity;
	}
}
