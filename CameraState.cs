using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000279 RID: 633
	public struct CameraState
	{
		// Token: 0x06000BEE RID: 3054 RVA: 0x0004D72C File Offset: 0x0004B92C
		public static CameraState Lerp(ref CameraState a, ref CameraState b, float t)
		{
			return new CameraState
			{
				position = Vector3.LerpUnclamped(a.position, b.position, t),
				rotation = Quaternion.SlerpUnclamped(a.rotation, b.rotation, t),
				fov = Mathf.LerpUnclamped(a.fov, b.fov, t)
			};
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x0004D790 File Offset: 0x0004B990
		public static CameraState SmoothDamp(CameraState current, CameraState target, ref Vector3 positionVelocity, ref float angleVelocity, ref float fovVelocity, float smoothTime)
		{
			return new CameraState
			{
				position = Vector3.SmoothDamp(current.position, target.position, ref positionVelocity, smoothTime),
				rotation = Util.SmoothDampQuaternion(current.rotation, target.rotation, ref angleVelocity, smoothTime),
				fov = Mathf.SmoothDamp(current.fov, target.fov, ref fovVelocity, smoothTime)
			};
		}

		// Token: 0x04000FD4 RID: 4052
		public Vector3 position;

		// Token: 0x04000FD5 RID: 4053
		public Quaternion rotation;

		// Token: 0x04000FD6 RID: 4054
		public float fov;
	}
}
