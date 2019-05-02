using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x02000258 RID: 600
	public class AnimateImageAlpha : MonoBehaviour
	{
		// Token: 0x06000B2A RID: 2858 RVA: 0x00008FB0 File Offset: 0x000071B0
		private void OnEnable()
		{
			this.stopwatch = 0f;
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x00008FB0 File Offset: 0x000071B0
		public void ResetStopwatch()
		{
			this.stopwatch = 0f;
		}

		// Token: 0x06000B2C RID: 2860 RVA: 0x0004B180 File Offset: 0x00049380
		private void LateUpdate()
		{
			this.stopwatch += Time.unscaledDeltaTime;
			int num = 0;
			foreach (Image image in this.images)
			{
				num++;
				float a = this.alphaCurve.Evaluate((this.stopwatch + this.delayBetweenElements * (float)num) / this.timeMax);
				Color color = image.color;
				image.color = new Color(color.r, color.g, color.b, a);
			}
		}

		// Token: 0x04000F2F RID: 3887
		public AnimationCurve alphaCurve;

		// Token: 0x04000F30 RID: 3888
		public Image[] images;

		// Token: 0x04000F31 RID: 3889
		public float timeMax = 5f;

		// Token: 0x04000F32 RID: 3890
		public float delayBetweenElements;

		// Token: 0x04000F33 RID: 3891
		[HideInInspector]
		public float stopwatch;
	}
}
