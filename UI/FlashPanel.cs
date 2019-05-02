using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005E9 RID: 1513
	public class FlashPanel : MonoBehaviour
	{
		// Token: 0x06002218 RID: 8728 RVA: 0x00018D08 File Offset: 0x00016F08
		private void Start()
		{
			this.image = this.flashRectTransform.GetComponent<Image>();
		}

		// Token: 0x06002219 RID: 8729 RVA: 0x000A3744 File Offset: 0x000A1944
		private void Update()
		{
			this.flashRectTransform.anchorMin = new Vector2(0f, 0f);
			this.flashRectTransform.anchorMax = new Vector2(1f, 1f);
			if (this.alwaysFlash)
			{
				this.isFlashing = true;
			}
			if (this.isFlashing)
			{
				this.theta += Time.deltaTime * this.freq;
			}
			if (this.theta > 1f)
			{
				if (this.alwaysFlash)
				{
					this.theta -= this.theta - this.theta % 1f;
				}
				else
				{
					this.theta = 1f;
				}
				this.isFlashing = false;
			}
			float num = 1f - (1f + Mathf.Cos(this.theta * 3.14159274f * 0.5f + 1.57079637f));
			this.flashRectTransform.sizeDelta = new Vector2(1f + num * 20f * this.strength, 1f + num * 20f * this.strength);
			if (this.image)
			{
				Color color = this.image.color;
				color.a = (1f - num) * this.strength * this.flashAlpha;
				this.image.color = color;
			}
		}

		// Token: 0x0600221A RID: 8730 RVA: 0x00018D1B File Offset: 0x00016F1B
		public void Flash()
		{
			this.theta = 0f;
			this.isFlashing = true;
		}

		// Token: 0x04002489 RID: 9353
		public RectTransform flashRectTransform;

		// Token: 0x0400248A RID: 9354
		public float strength = 1f;

		// Token: 0x0400248B RID: 9355
		public float freq = 1f;

		// Token: 0x0400248C RID: 9356
		public float flashAlpha = 0.7f;

		// Token: 0x0400248D RID: 9357
		public bool alwaysFlash = true;

		// Token: 0x0400248E RID: 9358
		private bool isFlashing;

		// Token: 0x0400248F RID: 9359
		private float theta = 1f;

		// Token: 0x04002490 RID: 9360
		private Image image;
	}
}
