using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RoR2
{
	// Token: 0x020004FB RID: 1275
	[RequireComponent(typeof(Camera))]
	[RequireComponent(typeof(SceneCamera))]
	public class OutlineHighlight : MonoBehaviour
	{
		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06001D03 RID: 7427 RVA: 0x000154F4 File Offset: 0x000136F4
		// (set) Token: 0x06001D04 RID: 7428 RVA: 0x000154FC File Offset: 0x000136FC
		public SceneCamera sceneCamera { get; private set; }

		// Token: 0x06001D05 RID: 7429 RVA: 0x0008DF3C File Offset: 0x0008C13C
		private void Awake()
		{
			this.sceneCamera = base.GetComponent<SceneCamera>();
			this.CreateBuffers();
			this.CreateMaterials();
			this.m_RTWidth = (int)((float)Screen.width / (float)this.m_resolution);
			this.m_RTHeight = (int)((float)Screen.height / (float)this.m_resolution);
		}

		// Token: 0x06001D06 RID: 7430 RVA: 0x00015505 File Offset: 0x00013705
		private void CreateBuffers()
		{
			this.commandBuffer = new CommandBuffer();
		}

		// Token: 0x06001D07 RID: 7431 RVA: 0x00015512 File Offset: 0x00013712
		private void ClearCommandBuffers()
		{
			this.commandBuffer.Clear();
		}

		// Token: 0x06001D08 RID: 7432 RVA: 0x0001551F File Offset: 0x0001371F
		private void CreateMaterials()
		{
			this.highlightMaterial = new Material(Shader.Find("Custom/OutlineHighlight"));
		}

		// Token: 0x06001D09 RID: 7433 RVA: 0x0008DF8C File Offset: 0x0008C18C
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

		// Token: 0x06001D0A RID: 7434 RVA: 0x0008E0C0 File Offset: 0x0008C2C0
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

		// Token: 0x04001ED5 RID: 7893
		public OutlineHighlight.RTResolution m_resolution = OutlineHighlight.RTResolution.Full;

		// Token: 0x04001ED6 RID: 7894
		public readonly Queue<OutlineHighlight.HighlightInfo> highlightQueue = new Queue<OutlineHighlight.HighlightInfo>();

		// Token: 0x04001ED8 RID: 7896
		private Material highlightMaterial;

		// Token: 0x04001ED9 RID: 7897
		private CommandBuffer commandBuffer;

		// Token: 0x04001EDA RID: 7898
		private int m_RTWidth = 512;

		// Token: 0x04001EDB RID: 7899
		private int m_RTHeight = 512;

		// Token: 0x04001EDC RID: 7900
		public static Action<OutlineHighlight> onPreRenderOutlineHighlight;

		// Token: 0x020004FC RID: 1276
		private enum Passes
		{
			// Token: 0x04001EDE RID: 7902
			FillPass,
			// Token: 0x04001EDF RID: 7903
			Blit
		}

		// Token: 0x020004FD RID: 1277
		public enum RTResolution
		{
			// Token: 0x04001EE1 RID: 7905
			Quarter = 4,
			// Token: 0x04001EE2 RID: 7906
			Half = 2,
			// Token: 0x04001EE3 RID: 7907
			Full = 1
		}

		// Token: 0x020004FE RID: 1278
		public struct HighlightInfo
		{
			// Token: 0x04001EE4 RID: 7908
			public Color color;

			// Token: 0x04001EE5 RID: 7909
			public Renderer renderer;
		}
	}
}
