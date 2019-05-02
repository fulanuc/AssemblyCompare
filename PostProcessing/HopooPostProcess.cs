using System;
using UnityEngine;

namespace RoR2.PostProcessing
{
	// Token: 0x02000562 RID: 1378
	[ExecuteInEditMode]
	public class HopooPostProcess : MonoBehaviour
	{
		// Token: 0x06001EDC RID: 7900 RVA: 0x000025F6 File Offset: 0x000007F6
		private void Start()
		{
		}

		// Token: 0x06001EDD RID: 7901 RVA: 0x000169C1 File Offset: 0x00014BC1
		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination, this.mat);
		}

		// Token: 0x04002173 RID: 8563
		public Material mat;
	}
}
