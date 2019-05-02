using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200046A RID: 1130
	[Serializable]
	public struct PitchYawPair
	{
		// Token: 0x0600195B RID: 6491 RVA: 0x00012DA9 File Offset: 0x00010FA9
		public PitchYawPair(float pitch, float yaw)
		{
			this.pitch = pitch;
			this.yaw = yaw;
		}

		// Token: 0x0600195C RID: 6492 RVA: 0x0008357C File Offset: 0x0008177C
		public static PitchYawPair Lerp(PitchYawPair a, PitchYawPair b, float t)
		{
			float num = Mathf.LerpAngle(a.pitch, b.pitch, t);
			float num2 = Mathf.LerpAngle(a.yaw, b.yaw, t);
			return new PitchYawPair(num, num2);
		}

		// Token: 0x0600195D RID: 6493 RVA: 0x000835B4 File Offset: 0x000817B4
		public static PitchYawPair SmoothDamp(PitchYawPair current, PitchYawPair target, ref PitchYawPair velocity, float smoothTime, float maxSpeed)
		{
			float num = Mathf.SmoothDampAngle(current.pitch, target.pitch, ref velocity.pitch, smoothTime, maxSpeed);
			float num2 = Mathf.SmoothDampAngle(current.yaw, target.yaw, ref velocity.yaw, smoothTime, maxSpeed);
			return new PitchYawPair(num, num2);
		}

		// Token: 0x04001CB8 RID: 7352
		public static readonly PitchYawPair zero = new PitchYawPair(0f, 0f);

		// Token: 0x04001CB9 RID: 7353
		public float pitch;

		// Token: 0x04001CBA RID: 7354
		public float yaw;
	}
}
