using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A4 RID: 932
	[ExecuteInEditMode]
	public class RainPostProcess : MonoBehaviour
	{
		// Token: 0x060013CF RID: 5071 RVA: 0x000020E4 File Offset: 0x000002E4
		private void Start()
		{
			base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
		}

		// Token: 0x060013D0 RID: 5072 RVA: 0x000025F6 File Offset: 0x000007F6
		private void Update()
		{
		}

		// Token: 0x060013D1 RID: 5073 RVA: 0x0000F14D File Offset: 0x0000D34D
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination, this.mat);
		}

		// Token: 0x0400176B RID: 5995
		public Material mat;

		// Token: 0x0400176C RID: 5996
		private RenderTexture renderTex;
	}
}
