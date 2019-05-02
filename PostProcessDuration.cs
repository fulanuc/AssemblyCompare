using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace RoR2
{
	// Token: 0x02000393 RID: 915
	public class PostProcessDuration : MonoBehaviour
	{
		// Token: 0x0600133F RID: 4927 RVA: 0x0000EBDE File Offset: 0x0000CDDE
		private void Update()
		{
			this.stopwatch += Time.deltaTime;
			this.UpdatePostProccess();
		}

		// Token: 0x06001340 RID: 4928 RVA: 0x0000EBF8 File Offset: 0x0000CDF8
		private void Awake()
		{
			this.UpdatePostProccess();
		}

		// Token: 0x06001341 RID: 4929 RVA: 0x0000EC00 File Offset: 0x0000CE00
		private void OnEnable()
		{
			this.stopwatch = 0f;
		}

		// Token: 0x06001342 RID: 4930 RVA: 0x0006BFB0 File Offset: 0x0006A1B0
		private void UpdatePostProccess()
		{
			float num = Mathf.Clamp01(this.stopwatch / this.maxDuration);
			this.ppVolume.weight = this.ppWeightCurve.Evaluate(num);
			if (num == 1f && this.destroyOnEnd)
			{
				UnityEngine.Object.Destroy(this.ppVolume.gameObject);
			}
		}

		// Token: 0x06001343 RID: 4931 RVA: 0x0000EC0D File Offset: 0x0000CE0D
		private void OnValidate()
		{
			if (this.maxDuration <= Mathf.Epsilon)
			{
				Debug.LogErrorFormat("{0} has PP of time zero!", new object[]
				{
					base.gameObject
				});
			}
		}

		// Token: 0x040016F0 RID: 5872
		public PostProcessVolume ppVolume;

		// Token: 0x040016F1 RID: 5873
		public AnimationCurve ppWeightCurve;

		// Token: 0x040016F2 RID: 5874
		public float maxDuration;

		// Token: 0x040016F3 RID: 5875
		public bool destroyOnEnd;

		// Token: 0x040016F4 RID: 5876
		private float stopwatch;
	}
}
