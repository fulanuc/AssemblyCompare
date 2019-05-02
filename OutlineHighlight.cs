using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RoR2
{
	// Token: 0x020004EC RID: 1260
	[RequireComponent(typeof(Camera))]
	[RequireComponent(typeof(SceneCamera))]
	public class OutlineHighlight : MonoBehaviour
	{
		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06001C9C RID: 7324 RVA: 0x00015045 File Offset: 0x00013245
		// (set) Token: 0x06001C9D RID: 7325 RVA: 0x0001504D File Offset: 0x0001324D
		public SceneCamera sceneCamera { get; private set; }

		// Token: 0x06001C9E RID: 7326 RVA: 0x0008D1E0 File Offset: 0x0008B3E0
		private void Awake()
		{
			this.sceneCamera = base.GetComponent<SceneCamera>();
			this.CreateBuffers();
			this.CreateMaterials();
			this.m_RTWidth = (int)((float)Screen.width / (float)this.m_resolution);
			this.m_RTHeight = (int)((float)Screen.height / (float)this.m_resolution);
		}

		// Token: 0x06001C9F RID: 7327 RVA: 0x00015056 File Offset: 0x00013256
		private void CreateBuffers()
		{
			this.commandBuffer = new CommandBuffer();
		}

		// Token: 0x06001CA0 RID: 7328 RVA: 0x00015063 File Offset: 0x00013263
		private void ClearCommandBuffers()
		{
			this.commandBuffer.Clear();
		}

		// Token: 0x06001CA1 RID: 7329 RVA: 0x00015070 File Offset: 0x00013270
		private void CreateMaterials()
		{
			this.highlightMaterial = new Material(Shader.Find("Custom/OutlineHighlight"));
		}

		// Token: 0x06001CA2 RID: 7330 RVA: 0x0008D230 File Offset: 0x0008B430
		private void RenderHighlights(RenderTexture rt)
		{
			RenderTargetIdentifier renderTarget = new RenderTargetIdentifier(rt);
			this.commandBuffer.SetRenderTarget(renderTarget);
			foreach (Highlight highlight in Highlight.readonlyHighlightList)
			{
				if (highlight.isOn && highlight.targetRenderer)
				{
					this.highlightQueue.Enqueue(new OutlineHighlight.HighlightInfo
					{
						color = highlight.GetColor() * highlight.strength,
						renderer = highlight.targetRenderer
					});
				}
			}
			Action<OutlineHighlight> action = OutlineHighlight.onPreRenderOutlineHighlight;
			if (action != null)
			{
				action(this);
			}
			while (this.highlightQueue.Count > 0)
			{
				OutlineHighlight.HighlightInfo highlightInfo = this.highlightQueue.Dequeue();
				if (highlightInfo.renderer)
				{
					this.highlightMaterial.SetColor("_Color", highlightInfo.color);
					this.commandBuffer.DrawRenderer(highlightInfo.renderer, this.highlightMaterial, 0, 0);
					RenderTexture.active = rt;
					Graphics.ExecuteCommandBuffer(this.commandBuffer);
					RenderTexture.active = null;
					this.ClearCommandBuffers();
				}
			}
		}

		// Token: 0x06001CA3 RID: 7331 RVA: 0x0008D364 File Offset: 0x0008B564
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			RenderTexture renderTexture = RenderTexture.active = RenderTexture.GetTemporary(this.m_RTWidth, this.m_RTHeight, 0, RenderTextureFormat.ARGB32);
			GL.Clear(true, true, Color.clear);
			RenderTexture.active = null;
			this.ClearCommandBuffers();
			this.RenderHighlights(renderTexture);
			this.highlightMaterial.SetTexture("_OutlineMap", renderTexture);
			this.highlightMaterial.SetColor("_Color", Color.white);
			Graphics.Blit(source, destination, this.highlightMaterial, 1);
			RenderTexture.ReleaseTemporary(renderTexture);
		}

		// Token: 0x04001E97 RID: 7831
		public OutlineHighlight.RTResolution m_resolution = OutlineHighlight.RTResolution.Full;

		// Token: 0x04001E98 RID: 7832
		public readonly Queue<OutlineHighlight.HighlightInfo> highlightQueue = new Queue<OutlineHighlight.HighlightInfo>();

		// Token: 0x04001E9A RID: 7834
		private Material highlightMaterial;

		// Token: 0x04001E9B RID: 7835
		private CommandBuffer commandBuffer;

		// Token: 0x04001E9C RID: 7836
		private int m_RTWidth = 512;

		// Token: 0x04001E9D RID: 7837
		private int m_RTHeight = 512;

		// Token: 0x04001E9E RID: 7838
		public static Action<OutlineHighlight> onPreRenderOutlineHighlight;

		// Token: 0x020004ED RID: 1261
		private enum Passes
		{
			// Token: 0x04001EA0 RID: 7840
			FillPass,
			// Token: 0x04001EA1 RID: 7841
			Blit
		}

		// Token: 0x020004EE RID: 1262
		public enum RTResolution
		{
			// Token: 0x04001EA3 RID: 7843
			Quarter = 4,
			// Token: 0x04001EA4 RID: 7844
			Half = 2,
			// Token: 0x04001EA5 RID: 7845
			Full = 1
		}

		// Token: 0x020004EF RID: 1263
		public struct HighlightInfo
		{
			// Token: 0x04001EA6 RID: 7846
			public Color color;

			// Token: 0x04001EA7 RID: 7847
			public Renderer renderer;
		}
	}
}
