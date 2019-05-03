using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004C5 RID: 1221
	public struct Trajectory
	{
		// Token: 0x06001B81 RID: 7041 RVA: 0x00014616 File Offset: 0x00012816
		public static float CalculateApex(float initialSpeed)
		{
			return Trajectory.CalculateApex(initialSpeed, Physics.gravity.y);
		}

		// Token: 0x06001B82 RID: 7042 RVA: 0x00014628 File Offset: 0x00012828
		public static float CalculateApex(float initialSpeed, float gravity)
		{
			return initialSpeed * initialSpeed / (2f * -gravity);
		}

		// Token: 0x06001B83 RID: 7043 RVA: 0x00014636 File Offset: 0x00012836
		public static float CalculateGroundSpeed(float time, float distance)
		{
			return distance / time;
		}

		// Token: 0x06001B84 RID: 7044 RVA: 0x00014636 File Offset: 0x00012836
		public static float CalculateGroundTravelTime(float hSpeed, float hDistance)
		{
			return hDistance / hSpeed;
		}

		// Token: 0x06001B85 RID: 7045 RVA: 0x0001463B File Offset: 0x0001283B
		public static float CalculateInitialYSpeed(float timeToTarget, float destinationYOffset)
		{
			return Trajectory.CalculateInitialYSpeed(timeToTarget, destinationYOffset, Physics.gravity.y);
		}

		// Token: 0x06001B86 RID: 7046 RVA: 0x0001464E File Offset: 0x0001284E
		public static float CalculateInitialYSpeed(float timeToTarget, float destinationYOffset, float gravity)
		{
			return (destinationYOffset + 0.5f * -gravity * timeToTarget * timeToTarget) / timeToTarget;
		}

		// Token: 0x06001B87 RID: 7047 RVA: 0x00014660 File Offset: 0x00012860
		public static float CalculateInitialYSpeedForHeight(float height)
		{
			return Trajectory.CalculateInitialYSpeedForHeight(height, Physics.gravity.y);
		}

		// Token: 0x06001B88 RID: 7048 RVA: 0x00014672 File Offset: 0x00012872
		public static float CalculateInitialYSpeedForHeight(float height, float gravity)
		{
			return Mathf.Sqrt(height * (2f * -gravity));
		}

		// Token: 0x06001B89 RID: 7049 RVA: 0x00014683 File Offset: 0x00012883
		public static Vector3 CalculatePositionAtTime(Vector3 origin, Vector3 initialVelocity, float t)
		{
			return Trajectory.CalculatePositionAtTime(origin, initialVelocity, t, Physics.gravity.y);
		}

		// Token: 0x06001B8A RID: 7050 RVA: 0x00088D0C File Offset: 0x00086F0C
		public static Vector3 CalculatePositionAtTime(Vector3 origin, Vector3 initialVelocity, float t, float gravity)
		{
			Vector3 result = origin + initialVelocity * t;
			result.y += 0.5f * gravity * t * t;
			return result;
		}

		// Token: 0x06001B8B RID: 7051 RVA: 0x00014697 File Offset: 0x00012897
		public static float CalculateInitialYSpeedForFlightDuration(float duration)
		{
			return Trajectory.CalculateInitialYSpeedForFlightDuration(duration, Physics.gravity.y);
		}

		// Token: 0x06001B8C RID: 7052 RVA: 0x000146A9 File Offset: 0x000128A9
		public static float CalculateInitialYSpeedForFlightDuration(float duration, float gravity)
		{
			return duration * gravity * -0.5f;
		}
	}
}
