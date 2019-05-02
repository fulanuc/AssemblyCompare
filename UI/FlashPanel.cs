using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005D7 RID: 1495
	public class FlashPanel : MonoBehaviour
	{
		// Token: 0x06002187 RID: 8583 RVA: 0x0001860E File Offset: 0x0001680E
		private void Start()
		{
			this.image = this.flashRectTransform.GetComponent<Image>();
		}

		// Token: 0x06002188 RID: 8584 RVA: 0x000A2170 File Offset: 0x000A0370
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

		// Token: 0x06002189 RID: 8585 RVA: 0x00018621 File Offset: 0x00016821
		public void Flash()
		{
			this.theta = 0f;
			this.isFlashing = true;
		}

		// Token: 0x04002435 RID: 9269
		public RectTransform flashRectTransform;

		// Token: 0x04002436 RID: 9270
		public float strength = 1f;

		// Token: 0x04002437 RID: 9271
		public float freq = 1f;

		// Token: 0x04002438 RID: 9272
		public float flashAlpha = 0.7f;

		// Token: 0x04002439 RID: 9273
		public bool alwaysFlash = true;

		// Token: 0x0400243A RID: 9274
		private bool isFlashing;

		// Token: 0x0400243B RID: 9275
		private float theta = 1f;

		// Token: 0x0400243C RID: 9276
		private Image image;
	}
}
