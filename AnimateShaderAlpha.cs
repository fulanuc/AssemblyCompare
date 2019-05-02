using System;
using ThreeEyedGames;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000259 RID: 601
	public class AnimateShaderAlpha : MonoBehaviour
	{
		// Token: 0x06000B2E RID: 2862 RVA: 0x0004B208 File Offset: 0x00049408
		private void Start()
		{
			this.targetRenderer = base.GetComponent<Renderer>();
			if (this.targetRenderer)
			{
				this.materials = this.targetRenderer.materials;
			}
			if (this.decal)
			{
				this.initialFade = this.decal.Fade;
			}
		}

		// Token: 0x06000B2F RID: 2863 RVA: 0x0004B260 File Offset: 0x00049460
		private void Update()
		{
			this.time = Mathf.Min(this.timeMax, this.time + Time.deltaTime);
			float num = this.alphaCurve.Evaluate(this.time / this.timeMax);
			if (this.decal)
			{
				this.decal.Fade = num * this.initialFade;
			}
			else
			{
				foreach (Material material in this.materials)
				{
					this._propBlock = new MaterialPropertyBlock();
					this.targetRenderer.GetPropertyBlock(this._propBlock);
					this._propBlock.SetFloat("_ExternalAlpha", num);
					this.targetRenderer.SetPropertyBlock(this._propBlock);
				}
			}
			if (this.time >= this.timeMax)
			{
				if (this.disableOnEnd)
				{
					base.enabled = false;
				}
				if (this.destroyOnEnd)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04000F34 RID: 3892
		public AnimationCurve alphaCurve;

		// Token: 0x04000F35 RID: 3893
		private Renderer targetRenderer;

		// Token: 0x04000F36 RID: 3894
		private MaterialPropertyBlock _propBlock;

		// Token: 0x04000F37 RID: 3895
		private Material[] materials;

		// Token: 0x04000F38 RID: 3896
		public float timeMax = 5f;

		// Token: 0x04000F39 RID: 3897
		[Tooltip("Optional field if you want to animate Decal 'Fade' rather than renderer _ExternalAlpha.")]
		public Decal decal;

		// Token: 0x04000F3A RID: 3898
		public bool destroyOnEnd;

		// Token: 0x04000F3B RID: 3899
		public bool disableOnEnd;

		// Token: 0x04000F3C RID: 3900
		[HideInInspector]
		public float time;

		// Token: 0x04000F3D RID: 3901
		private float initialFade;
	}
}
