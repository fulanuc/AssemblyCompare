using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x02000258 RID: 600
	public class AnimateImageAlpha : MonoBehaviour
	{
		// Token: 0x06000B2D RID: 2861 RVA: 0x00008FD5 File Offset: 0x000071D5
		private void OnEnable()
		{
			this.stopwatch = 0f;
		}

		// Token: 0x06000B2E RID: 2862 RVA: 0x00008FD5 File Offset: 0x000071D5
		public void ResetStopwatch()
		{
			this.stopwatch = 0f;
		}

		// Token: 0x06000B2F RID: 2863 RVA: 0x0004B38C File Offset: 0x0004958C
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

		// Token: 0x04000F35 RID: 3893
		public AnimationCurve alphaCurve;

		// Token: 0x04000F36 RID: 3894
		public Image[] images;

		// Token: 0x04000F37 RID: 3895
		public float timeMax = 5f;

		// Token: 0x04000F38 RID: 3896
		public float delayBetweenElements;

		// Token: 0x04000F39 RID: 3897
		[HideInInspector]
		public float stopwatch;
	}
}
