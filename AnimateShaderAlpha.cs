using System;
using ThreeEyedGames;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000259 RID: 601
	public class AnimateShaderAlpha : MonoBehaviour
	{
		// Token: 0x06000B31 RID: 2865 RVA: 0x0004B414 File Offset: 0x00049614
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

		// Token: 0x06000B32 RID: 2866 RVA: 0x0004B46C File Offset: 0x0004966C
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

		// Token: 0x04000F3A RID: 3898
		public AnimationCurve alphaCurve;

		// Token: 0x04000F3B RID: 3899
		private Renderer targetRenderer;

		// Token: 0x04000F3C RID: 3900
		private MaterialPropertyBlock _propBlock;

		// Token: 0x04000F3D RID: 3901
		private Material[] materials;

		// Token: 0x04000F3E RID: 3902
		public float timeMax = 5f;

		// Token: 0x04000F3F RID: 3903
		[Tooltip("Optional field if you want to animate Decal 'Fade' rather than renderer _ExternalAlpha.")]
		public Decal decal;

		// Token: 0x04000F40 RID: 3904
		public bool destroyOnEnd;

		// Token: 0x04000F41 RID: 3905
		public bool disableOnEnd;

		// Token: 0x04000F42 RID: 3906
		[HideInInspector]
		public float time;

		// Token: 0x04000F43 RID: 3907
		private float initialFade;
	}
}
