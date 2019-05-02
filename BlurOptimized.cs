using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000231 RID: 561
	[RequireComponent(typeof(Camera))]
	public class BlurOptimized : MonoBehaviour
	{
		// Token: 0x06000ABF RID: 2751 RVA: 0x00008A97 File Offset: 0x00006C97
		public void Start()
		{
			this.blurMaterial = new Material(Shader.Find("Hidden/FastBlur"));
			base.enabled = false;
		}

		// Token: 0x06000AC0 RID: 2752 RVA: 0x000491A8 File Offset: 0x000473A8
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

		// Token: 0x04000E54 RID: 3668
		[Range(0f, 2f)]
		public int downsample = 1;

		// Token: 0x04000E55 RID: 3669
		[Range(0f, 10f)]
		public float blurSize = 3f;

		// Token: 0x04000E56 RID: 3670
		[Range(1f, 4f)]
		public int blurIterations = 2;

		// Token: 0x04000E57 RID: 3671
		public BlurOptimized.BlurType blurType;

		// Token: 0x04000E58 RID: 3672
		[HideInInspector]
		public Material blurMaterial;

		// Token: 0x02000232 RID: 562
		public enum BlurType
		{
			// Token: 0x04000E5A RID: 3674
			StandardGauss,
			// Token: 0x04000E5B RID: 3675
			SgxGauss
		}
	}
}
