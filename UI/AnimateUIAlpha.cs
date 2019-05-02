using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005A9 RID: 1449
	public class AnimateUIAlpha : MonoBehaviour
	{
		// Token: 0x06002085 RID: 8325 RVA: 0x0009E1C8 File Offset: 0x0009C3C8
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

		// Token: 0x06002086 RID: 8326 RVA: 0x00017B21 File Offset: 0x00015D21
		private void OnDisable()
		{
			this.time = 0f;
		}

		// Token: 0x06002087 RID: 8327 RVA: 0x00017B2E File Offset: 0x00015D2E
		private void Update()
		{
			this.UpdateAlphas(Time.unscaledDeltaTime);
		}

		// Token: 0x06002088 RID: 8328 RVA: 0x0009E23C File Offset: 0x0009C43C
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

		// Token: 0x04002303 RID: 8963
		public AnimationCurve alphaCurve;

		// Token: 0x04002304 RID: 8964
		public Image image;

		// Token: 0x04002305 RID: 8965
		public RawImage rawImage;

		// Token: 0x04002306 RID: 8966
		public SpriteRenderer spriteRenderer;

		// Token: 0x04002307 RID: 8967
		public float timeMax = 5f;

		// Token: 0x04002308 RID: 8968
		public bool destroyOnEnd;

		// Token: 0x04002309 RID: 8969
		public bool loopOnEnd;

		// Token: 0x0400230A RID: 8970
		public bool disableGameObjectOnEnd;

		// Token: 0x0400230B RID: 8971
		[HideInInspector]
		public float time;

		// Token: 0x0400230C RID: 8972
		private Color originalColor;
	}
}
