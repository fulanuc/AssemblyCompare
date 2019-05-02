using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002F5 RID: 757
	public class FlickerLight : MonoBehaviour
	{
		// Token: 0x06000F58 RID: 3928 RVA: 0x0005CB44 File Offset: 0x0005AD44
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

		// Token: 0x06000F59 RID: 3929 RVA: 0x0005CBA8 File Offset: 0x0005ADA8
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

		// Token: 0x0400136C RID: 4972
		public Light light;

		// Token: 0x0400136D RID: 4973
		public Wave[] sinWaves;

		// Token: 0x0400136E RID: 4974
		private float initialLightIntensity;

		// Token: 0x0400136F RID: 4975
		private float stopwatch;

		// Token: 0x04001370 RID: 4976
		private float randomPhase;
	}
}
