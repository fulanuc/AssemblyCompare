using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000475 RID: 1141
	[Serializable]
	public struct PitchYawPair
	{
		// Token: 0x060019B8 RID: 6584 RVA: 0x000132C3 File Offset: 0x000114C3
		public PitchYawPair(float pitch, float yaw)
		{
			this.pitch = pitch;
			this.yaw = yaw;
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x00083F24 File Offset: 0x00082124
		public static PitchYawPair Lerp(PitchYawPair a, PitchYawPair b, float t)
		{
			float num = Mathf.LerpAngle(a.pitch, b.pitch, t);
			float num2 = Mathf.LerpAngle(a.yaw, b.yaw, t);
			return new PitchYawPair(num, num2);
		}

		// Token: 0x060019BA RID: 6586 RVA: 0x00083F5C File Offset: 0x0008215C
		public static PitchYawPair SmoothDamp(PitchYawPair current, PitchYawPair target, ref PitchYawPair velocity, float smoothTime, float maxSpeed)
		{
			float num = Mathf.SmoothDampAngle(current.pitch, target.pitch, ref velocity.pitch, smoothTime, maxSpeed);
			float num2 = Mathf.SmoothDampAngle(current.yaw, target.yaw, ref velocity.yaw, smoothTime, maxSpeed);
			return new PitchYawPair(num, num2);
		}

		// Token: 0x04001CEC RID: 7404
		public static readonly PitchYawPair zero = new PitchYawPair(0f, 0f);

		// Token: 0x04001CED RID: 7405
		public float pitch;

		// Token: 0x04001CEE RID: 7406
		public float yaw;
	}
}
