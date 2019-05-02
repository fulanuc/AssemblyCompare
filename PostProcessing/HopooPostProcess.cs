using System;
using UnityEngine;

namespace RoR2.PostProcessing
{
	// Token: 0x02000571 RID: 1393
	[ExecuteInEditMode]
	public class HopooPostProcess : MonoBehaviour
	{
		// Token: 0x06001F46 RID: 8006 RVA: 0x000025DA File Offset: 0x000007DA
		private void Start()
		{
		}

		// Token: 0x06001F47 RID: 8007 RVA: 0x00016EA0 File Offset: 0x000150A0
		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination, this.mat);
		}

		// Token: 0x040021B1 RID: 8625
		public Material mat;
	}
}
