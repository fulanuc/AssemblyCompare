using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004D3 RID: 1235
	public struct Trajectory
	{
		// Token: 0x06001BE5 RID: 7141 RVA: 0x00014AE3 File Offset: 0x00012CE3
		public static float CalculateApex(float initialSpeed)
		{
			return Trajectory.CalculateApex(initialSpeed, Physics.gravity.y);
		}

		// Token: 0x06001BE6 RID: 7142 RVA: 0x00014AF5 File Offset: 0x00012CF5
		public static float CalculateApex(float initialSpeed, float gravity)
		{
			return initialSpeed * initialSpeed / (2f * -gravity);
		}

		// Token: 0x06001BE7 RID: 7143 RVA: 0x00014B03 File Offset: 0x00012D03
		public static float CalculateGroundSpeed(float time, float distance)
		{
			return distance / time;
		}

		// Token: 0x06001BE8 RID: 7144 RVA: 0x00014B03 File Offset: 0x00012D03
		public static float CalculateGroundTravelTime(float hSpeed, float hDistance)
		{
			return hDistance / hSpeed;
		}

		// Token: 0x06001BE9 RID: 7145 RVA: 0x00014B08 File Offset: 0x00012D08
		public static float CalculateInitialYSpeed(float timeToTarget, float destinationYOffset)
		{
			return Trajectory.CalculateInitialYSpeed(timeToTarget, destinationYOffset, Physics.gravity.y);
		}

		// Token: 0x06001BEA RID: 7146 RVA: 0x00014B1B File Offset: 0x00012D1B
		public static float CalculateInitialYSpeed(float timeToTarget, float destinationYOffset, float gravity)
		{
			return (destinationYOffset + 0.5f * -gravity * timeToTarget * timeToTarget) / timeToTarget;
		}

		// Token: 0x06001BEB RID: 7147 RVA: 0x00014B2D File Offset: 0x00012D2D
		public static float CalculateInitialYSpeedForHeight(float height)
		{
			return Trajectory.CalculateInitialYSpeedForHeight(height, Physics.gravity.y);
		}

		// Token: 0x06001BEC RID: 7148 RVA: 0x00014B3F File Offset: 0x00012D3F
		public static float CalculateInitialYSpeedForHeight(float height, float gravity)
		{
			return Mathf.Sqrt(height * (2f * -gravity));
		}

		// Token: 0x06001BED RID: 7149 RVA: 0x00014B50 File Offset: 0x00012D50
		public static Vector3 CalculatePositionAtTime(Vector3 origin, Vector3 initialVelocity, float t)
		{
			return Trajectory.CalculatePositionAtTime(origin, initialVelocity, t, Physics.gravity.y);
		}

		// Token: 0x06001BEE RID: 7150 RVA: 0x00089884 File Offset: 0x00087A84
		public static Vector3 CalculatePositionAtTime(Vector3 origin, Vector3 initialVelocity, float t, float gravity)
		{
			Vector3 result = origin + initialVelocity * t;
			result.y += 0.5f * gravity * t * t;
			return result;
		}

		// Token: 0x06001BEF RID: 7151 RVA: 0x00014B64 File Offset: 0x00012D64
		public static float CalculateInitialYSpeedForFlightDuration(float duration)
		{
			return Trajectory.CalculateInitialYSpeedForFlightDuration(duration, Physics.gravity.y);
		}

		// Token: 0x06001BF0 RID: 7152 RVA: 0x00014B76 File Offset: 0x00012D76
		public static float CalculateInitialYSpeedForFlightDuration(float duration, float gravity)
		{
			return duration * gravity * -0.5f;
		}
	}
}
