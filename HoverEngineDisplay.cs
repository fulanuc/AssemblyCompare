using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000316 RID: 790
	public class HoverEngineDisplay : MonoBehaviour
	{
		// Token: 0x0600106F RID: 4207 RVA: 0x000624D8 File Offset: 0x000606D8
		private void FixedUpdate()
		{
			ref Vector3 localEulerAngles = base.transform.localEulerAngles;
			float t = Mathf.Clamp01(this.hoverEngine.forceStrength / this.hoverEngine.hoverForce * this.forceScale);
			float target = Mathf.LerpAngle(this.minPitch, this.maxPitch, t);
			float x = Mathf.SmoothDampAngle(localEulerAngles.x, target, ref this.smoothVelocity, this.smoothTime);
			base.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
		}

		// Token: 0x04001460 RID: 5216
		public HoverEngine hoverEngine;

		// Token: 0x04001461 RID: 5217
		[Tooltip("The local pitch at zero engine strength")]
		public float minPitch = -20f;

		// Token: 0x04001462 RID: 5218
		[Tooltip("The local pitch at max engine strength")]
		public float maxPitch = 60f;

		// Token: 0x04001463 RID: 5219
		public float smoothTime = 0.2f;

		// Token: 0x04001464 RID: 5220
		public float forceScale = 1f;

		// Token: 0x04001465 RID: 5221
		private float smoothVelocity;
	}
}
