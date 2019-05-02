using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000279 RID: 633
	public struct CameraState
	{
		// Token: 0x06000BE5 RID: 3045 RVA: 0x0004D520 File Offset: 0x0004B720
		public static CameraState Lerp(ref CameraState a, ref CameraState b, float t)
		{
			return new CameraState
			{
				position = Vector3.LerpUnclamped(a.position, b.position, t),
				rotation = Quaternion.SlerpUnclamped(a.rotation, b.rotation, t),
				fov = Mathf.LerpUnclamped(a.fov, b.fov, t)
			};
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x0004D584 File Offset: 0x0004B784
		public static CameraState SmoothDamp(CameraState current, CameraState target, ref Vector3 positionVelocity, ref float angleVelocity, ref float fovVelocity, float smoothTime)
		{
			return new CameraState
			{
				position = Vector3.SmoothDamp(current.position, target.position, ref positionVelocity, smoothTime),
				rotation = Util.SmoothDampQuaternion(current.rotation, target.rotation, ref angleVelocity, smoothTime),
				fov = Mathf.SmoothDamp(current.fov, target.fov, ref fovVelocity, smoothTime)
			};
		}

		// Token: 0x04000FCE RID: 4046
		public Vector3 position;

		// Token: 0x04000FCF RID: 4047
		public Quaternion rotation;

		// Token: 0x04000FD0 RID: 4048
		public float fov;
	}
}
