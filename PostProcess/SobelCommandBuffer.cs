using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace RoR2.PostProcess
{
	// Token: 0x02000561 RID: 1377
	[RequireComponent(typeof(Camera))]
	[ExecuteInEditMode]
	public class SobelCommandBuffer : MonoBehaviour
	{
		// Token: 0x06001ED7 RID: 7895 RVA: 0x00016966 File Offset: 0x00014B66
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
		}

		// Token: 0x06001ED8 RID: 7896 RVA: 0x00016974 File Offset: 0x00014B74
		private void OnPreRender()
		{
			this.SetupCommandBuffer();
		}

		// Token: 0x06001ED9 RID: 7897 RVA: 0x0001697C File Offset: 0x00014B7C
		private void OnDisable()
		{
			if (this.camera != null && this.sobelCommandBuffer != null)
			{
				this.camera.RemoveCommandBuffer(this.cameraEvent, this.sobelCommandBuffer);
				this.sobelCommandBuffer = null;
			}
		}

		// Token: 0x06001EDA RID: 7898 RVA: 0x00097B78 File Offset: 0x00095D78
		private void SetupCommandBuffer()
		{
			if (!this.camera)
			{
				return;
			}
			if (this.sobelCommandBuffer == null)
			{
				int nameID = Shader.PropertyToID("_SobelTex");
				this.sobelBufferMaterial = new Material(Shader.Find("Hidden/SobelBuffer"));
				this.sobelCommandBuffer = new CommandBuffer();
				this.sobelCommandBuffer.name = "Sobel Command Buffer";
				this.sobelCommandBuffer.GetTemporaryRT(nameID, -1, -1);
				RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(nameID);
				this.sobelCommandBuffer.SetRenderTarget(renderTargetIdentifier);
				this.sobelCommandBuffer.ClearRenderTarget(true, true, Color.clear);
				this.sobelCommandBuffer.Blit(new RenderTargetIdentifier(BuiltinRenderTextureType.ResolvedDepth), renderTargetIdentifier, this.sobelBufferMaterial);
				this.sobelCommandBuffer.SetGlobalTexture(nameID, renderTargetIdentifier);
				this.sobelCommandBuffer.SetRenderTarget(new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget));
				this.sobelCommandBuffer.ReleaseTemporaryRT(nameID);
				this.camera.AddCommandBuffer(this.cameraEvent, this.sobelCommandBuffer);
			}
		}

		// Token: 0x0400216E RID: 8558
		public CameraEvent cameraEvent = CameraEvent.BeforeLighting;

		// Token: 0x0400216F RID: 8559
		private RenderTexture sobelRT;

		// Token: 0x04002170 RID: 8560
		private CommandBuffer sobelCommandBuffer;

		// Token: 0x04002171 RID: 8561
		private Material sobelBufferMaterial;

		// Token: 0x04002172 RID: 8562
		private Camera camera;
	}
}
