using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace RoR2
{
	// Token: 0x0200038E RID: 910
	public class PostProcessDuration : MonoBehaviour
	{
		// Token: 0x06001321 RID: 4897 RVA: 0x0000EA28 File Offset: 0x0000CC28
		private void Update()
		{
			this.stopwatch += Time.deltaTime;
			this.UpdatePostProccess();
		}

		// Token: 0x06001322 RID: 4898 RVA: 0x0000EA42 File Offset: 0x0000CC42
		private void Awake()
		{
			this.UpdatePostProccess();
		}

		// Token: 0x06001323 RID: 4899 RVA: 0x0000EA4A File Offset: 0x0000CC4A
		private void OnEnable()
		{
			this.stopwatch = 0f;
		}

		// Token: 0x06001324 RID: 4900 RVA: 0x0006BD44 File Offset: 0x00069F44
		private void UpdatePostProccess()
		{
			float num = Mathf.Clamp01(this.stopwatch / this.maxDuration);
			this.ppVolume.weight = this.ppWeightCurve.Evaluate(num);
			if (num == 1f && this.destroyOnEnd)
			{
				UnityEngine.Object.Destroy(this.ppVolume.gameObject);
			}
		}

		// Token: 0x06001325 RID: 4901 RVA: 0x0000EA57 File Offset: 0x0000CC57
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

		// Token: 0x040016D4 RID: 5844
		public PostProcessVolume ppVolume;

		// Token: 0x040016D5 RID: 5845
		public AnimationCurve ppWeightCurve;

		// Token: 0x040016D6 RID: 5846
		public float maxDuration;

		// Token: 0x040016D7 RID: 5847
		public bool destroyOnEnd;

		// Token: 0x040016D8 RID: 5848
		private float stopwatch;
	}
}
