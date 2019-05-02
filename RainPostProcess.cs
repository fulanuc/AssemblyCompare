using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A9 RID: 937
	[ExecuteInEditMode]
	public class RainPostProcess : MonoBehaviour
	{
		// Token: 0x060013EC RID: 5100 RVA: 0x000020C8 File Offset: 0x000002C8
		private void Start()
		{
			base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
		}

		// Token: 0x060013ED RID: 5101 RVA: 0x000025DA File Offset: 0x000007DA
		private void Update()
		{
		}

		// Token: 0x060013EE RID: 5102 RVA: 0x0000F2F1 File Offset: 0x0000D4F1
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination, this.mat);
		}

		// Token: 0x04001785 RID: 6021
		public Material mat;

		// Token: 0x04001786 RID: 6022
		private RenderTexture renderTex;
	}
}
