using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005BB RID: 1467
	public class AnimateUIAlpha : MonoBehaviour
	{
		// Token: 0x06002116 RID: 8470 RVA: 0x0009F79C File Offset: 0x0009D99C
		private void Start()
		{
			if (this.image)
			{
				this.originalColor = this.image.color;
			}
			if (this.rawImage)
			{
				this.originalColor = this.rawImage.color;
			}
			else if (this.spriteRenderer)
			{
				this.originalColor = this.spriteRenderer.color;
			}
			this.UpdateAlphas(0f);
		}

		// Token: 0x06002117 RID: 8471 RVA: 0x0001821B File Offset: 0x0001641B
		private void OnDisable()
		{
			this.time = 0f;
		}

		// Token: 0x06002118 RID: 8472 RVA: 0x00018228 File Offset: 0x00016428
		private void Update()
		{
			this.UpdateAlphas(Time.unscaledDeltaTime);
		}

		// Token: 0x06002119 RID: 8473 RVA: 0x0009F810 File Offset: 0x0009DA10
		private void UpdateAlphas(float deltaTime)
		{
			this.time = Mathf.Min(this.timeMax, this.time + deltaTime);
			float num = this.alphaCurve.Evaluate(this.time / this.timeMax);
			Color color = new Color(this.originalColor.r, this.originalColor.g, this.originalColor.b, this.originalColor.a * num);
			if (this.image)
			{
				this.image.color = color;
			}
			if (this.rawImage)
			{
				this.rawImage.color = color;
			}
			else if (this.spriteRenderer)
			{
				this.spriteRenderer.color = color;
			}
			if (this.loopOnEnd && this.time >= this.timeMax)
			{
				this.time -= this.timeMax;
			}
			if (this.destroyOnEnd && this.time >= this.timeMax)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (this.disableGameObjectOnEnd && this.time >= this.timeMax)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x04002357 RID: 9047
		public AnimationCurve alphaCurve;

		// Token: 0x04002358 RID: 9048
		public Image image;

		// Token: 0x04002359 RID: 9049
		public RawImage rawImage;

		// Token: 0x0400235A RID: 9050
		public SpriteRenderer spriteRenderer;

		// Token: 0x0400235B RID: 9051
		public float timeMax = 5f;

		// Token: 0x0400235C RID: 9052
		public bool destroyOnEnd;

		// Token: 0x0400235D RID: 9053
		public bool loopOnEnd;

		// Token: 0x0400235E RID: 9054
		public bool disableGameObjectOnEnd;

		// Token: 0x0400235F RID: 9055
		[HideInInspector]
		public float time;

		// Token: 0x04002360 RID: 9056
		private Color originalColor;
	}
}
