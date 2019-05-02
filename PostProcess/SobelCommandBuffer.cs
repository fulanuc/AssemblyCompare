using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace RoR2.PostProcess
{
	// Token: 0x02000570 RID: 1392
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class SobelCommandBuffer : MonoBehaviour
	{
		// Token: 0x06001F41 RID: 8001 RVA: 0x00016E45 File Offset: 0x00015045
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
		}

		// Token: 0x06001F42 RID: 8002 RVA: 0x00016E53 File Offset: 0x00015053
		private void OnPreRender()
		{
			this.SetupCommandBuffer();
		}

		// Token: 0x06001F43 RID: 8003 RVA: 0x00016E5B File Offset: 0x0001505B
		private void OnDisable()
		{
			if (this.camera != null && this.sobelCommandBuffer != null)
			{
				this.camera.RemoveCommandBuffer(this.cameraEvent, this.sobelCommandBuffer);
				this.sobelCommandBuffer = null;
			}
		}

		// Token: 0x06001F44 RID: 8004 RVA: 0x00098894 File Offset: 0x00096A94
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

		// Token: 0x040021AC RID: 8620
		public CameraEvent cameraEvent = CameraEvent.BeforeLighting;

		// Token: 0x040021AD RID: 8621
		private RenderTexture sobelRT;

		// Token: 0x040021AE RID: 8622
		private CommandBuffer sobelCommandBuffer;

		// Token: 0x040021AF RID: 8623
		private Material sobelBufferMaterial;

		// Token: 0x040021B0 RID: 8624
		private Camera camera;
	}
}
