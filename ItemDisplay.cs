using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace RoR2
{
	// Token: 0x02000342 RID: 834
	public class ItemDisplay : MonoBehaviour
	{
		// Token: 0x0600114A RID: 4426 RVA: 0x00065710 File Offset: 0x00063910
		public void SetVisibilityLevel(VisibilityLevel newVisibilityLevel)
		{
			if (this.visibilityLevel != newVisibilityLevel)
			{
				this.visibilityLevel = newVisibilityLevel;
				switch (this.visibilityLevel)
				{
				case VisibilityLevel.Invisible:
					for (int i = 0; i < this.rendererInfos.Length; i++)
					{
						CharacterModel.RendererInfo rendererInfo = this.rendererInfos[i];
						rendererInfo.renderer.enabled = false;
						rendererInfo.renderer.shadowCastingMode = ShadowCastingMode.Off;
					}
					return;
				case VisibilityLevel.Cloaked:
					for (int j = 0; j < this.rendererInfos.Length; j++)
					{
						CharacterModel.RendererInfo rendererInfo2 = this.rendererInfos[j];
						rendererInfo2.renderer.enabled = true;
						rendererInfo2.renderer.shadowCastingMode = ShadowCastingMode.Off;
						rendererInfo2.renderer.material = CharacterModel.cloakedMaterial;
					}
					return;
				case VisibilityLevel.Revealed:
					for (int k = 0; k < this.rendererInfos.Length; k++)
					{
						CharacterModel.RendererInfo rendererInfo3 = this.rendererInfos[k];
						rendererInfo3.renderer.enabled = true;
						rendererInfo3.renderer.shadowCastingMode = ShadowCastingMode.Off;
						rendererInfo3.renderer.material = CharacterModel.revealedMaterial;
					}
					return;
				case VisibilityLevel.Visible:
					for (int l = 0; l < this.rendererInfos.Length; l++)
					{
						CharacterModel.RendererInfo rendererInfo4 = this.rendererInfos[l];
						rendererInfo4.renderer.enabled = true;
						rendererInfo4.renderer.shadowCastingMode = ShadowCastingMode.On;
						rendererInfo4.renderer.material = rendererInfo4.defaultMaterial;
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x04001548 RID: 5448
		public CharacterModel.RendererInfo[] rendererInfos;

		// Token: 0x04001549 RID: 5449
		private VisibilityLevel visibilityLevel = VisibilityLevel.Visible;
	}
}
