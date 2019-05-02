using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000231 RID: 561
	[RequireComponent(typeof(Camera))]
	public class BlurOptimized : MonoBehaviour
	{
		// Token: 0x06000ABB RID: 2747 RVA: 0x00008A72 File Offset: 0x00006C72
		public void Start()
		{
			this.blurMaterial = new Material(Shader.Find("Hidden/FastBlur"));
			base.enabled = false;
		}

		// Token: 0x06000ABC RID: 2748 RVA: 0x00048EEC File Offset: 0x000470EC
		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			float num = 1f / (1f * (float)(1 << this.downsample));
			this.blurMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num, -this.blurSize * num, 0f, 0f));
			source.filterMode = FilterMode.Bilinear;
			int width = source.width >> this.downsample;
			int height = source.height >> this.downsample;
			RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, renderTexture, this.blurMaterial, 0);
			int num2 = (this.blurType == BlurOptimized.BlurType.StandardGauss) ? 0 : 2;
			for (int i = 0; i < this.blurIterations; i++)
			{
				float num3 = (float)i * 1f;
				this.blurMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num + num3, -this.blurSize * num - num3, 0f, 0f));
				RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, this.blurMaterial, 1 + num2);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
				temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, this.blurMaterial, 2 + num2);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
			}
			Graphics.Blit(renderTexture, destination);
			RenderTexture.ReleaseTemporary(renderTexture);
		}

		// Token: 0x04000E4F RID: 3663
		[Range(0f, 2f)]
		public int downsample = 1;

		// Token: 0x04000E50 RID: 3664
		[Range(0f, 10f)]
		public float blurSize = 3f;

		// Token: 0x04000E51 RID: 3665
		[Range(1f, 4f)]
		public int blurIterations = 2;

		// Token: 0x04000E52 RID: 3666
		public BlurOptimized.BlurType blurType;

		// Token: 0x04000E53 RID: 3667
		[HideInInspector]
		public Material blurMaterial;

		// Token: 0x02000232 RID: 562
		public enum BlurType
		{
			// Token: 0x04000E55 RID: 3669
			StandardGauss,
			// Token: 0x04000E56 RID: 3670
			SgxGauss
		}
	}
}
