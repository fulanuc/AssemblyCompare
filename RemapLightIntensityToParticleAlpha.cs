using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000482 RID: 1154
	public class RemapLightIntensityToParticleAlpha : MonoBehaviour
	{
		// Token: 0x060019DF RID: 6623 RVA: 0x000845B4 File Offset: 0x000827B4
		private void LateUpdate()
		{
			ParticleSystem.MainModule main = this.particleSystem.main;
			ParticleSystem.MinMaxGradient startColor = main.startColor;
			Color color = startColor.color;
			color.a = Util.Remap(this.light.intensity, this.lowerLightIntensity, this.upperLightIntensity, this.lowerParticleAlpha, this.upperParticleAlpha);
			startColor.color = color;
			main.startColor = startColor;
		}

		// Token: 0x04001D1A RID: 7450
		public Light light;

		// Token: 0x04001D1B RID: 7451
		public ParticleSystem particleSystem;

		// Token: 0x04001D1C RID: 7452
		public float lowerLightIntensity;

		// Token: 0x04001D1D RID: 7453
		public float upperLightIntensity = 1f;

		// Token: 0x04001D1E RID: 7454
		public float lowerParticleAlpha;

		// Token: 0x04001D1F RID: 7455
		public float upperParticleAlpha = 1f;
	}
}
