using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002F8 RID: 760
	public class FlickerLight : MonoBehaviour
	{
		// Token: 0x06000F68 RID: 3944 RVA: 0x0005CD64 File Offset: 0x0005AF64
		private void Start()
		{
			this.initialLightIntensity = this.light.intensity;
			this.randomPhase = UnityEngine.Random.Range(0f, 6.28318548f);
			for (int i = 0; i < this.sinWaves.Length; i++)
			{
				Wave[] array = this.sinWaves;
				int num = i;
				array[num].cycleOffset = array[num].cycleOffset + this.randomPhase;
			}
		}

		// Token: 0x06000F69 RID: 3945 RVA: 0x0005CDC8 File Offset: 0x0005AFC8
		private void Update()
		{
			this.stopwatch += Time.deltaTime;
			float num = this.initialLightIntensity;
			for (int i = 0; i < this.sinWaves.Length; i++)
			{
				num *= 1f + this.sinWaves[i].Evaluate(this.stopwatch);
			}
			this.light.intensity = num;
		}

		// Token: 0x04001383 RID: 4995
		public Light light;

		// Token: 0x04001384 RID: 4996
		public Wave[] sinWaves;

		// Token: 0x04001385 RID: 4997
		private float initialLightIntensity;

		// Token: 0x04001386 RID: 4998
		private float stopwatch;

		// Token: 0x04001387 RID: 4999
		private float randomPhase;
	}
}
