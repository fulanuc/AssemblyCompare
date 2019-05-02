using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000475 RID: 1141
	public class RemapLightIntensityToParticleAlpha : MonoBehaviour
	{
		// Token: 0x0600197D RID: 6525 RVA: 0x00083B48 File Offset: 0x00081D48
		private void LateUpdate()
		{
			ParticleSystem.MainModule main = this.particleSystem.main;
			ParticleSystem.MinMaxGradient startColor = main.startColor;
			Color color = startColor.color;
			color.a = Util.Remap(this.light.intensity, this.lowerLightIntensity, this.upperLightIntensity, this.lowerParticleAlpha, this.upperParticleAlpha);
			startColor.color = color;
			main.startColor = startColor;
		}

		// Token: 0x04001CE3 RID: 7395
		public Light light;

		// Token: 0x04001CE4 RID: 7396
		public ParticleSystem particleSystem;

		// Token: 0x04001CE5 RID: 7397
		public float lowerLightIntensity;

		// Token: 0x04001CE6 RID: 7398
		public float upperLightIntensity = 1f;

		// Token: 0x04001CE7 RID: 7399
		public float lowerParticleAlpha;

		// Token: 0x04001CE8 RID: 7400
		public float upperParticleAlpha = 1f;
	}
}
