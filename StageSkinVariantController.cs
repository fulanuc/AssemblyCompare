using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003F2 RID: 1010
	public class StageSkinVariantController : MonoBehaviour
	{
		// Token: 0x0600163F RID: 5695 RVA: 0x00075FE0 File Offset: 0x000741E0
		private void Awake()
		{
			if (SceneInfo.instance)
			{
				for (int i = 0; i < this.stageSkinVariants.Length; i++)
				{
					StageSkinVariantController.StageSkinVariant stageSkinVariant = this.stageSkinVariants[i];
					if (SceneInfo.instance.sceneDef.nameToken == stageSkinVariant.stageNameToken)
					{
						for (int j = 0; j < stageSkinVariant.childObjects.Length; j++)
						{
							stageSkinVariant.childObjects[j].SetActive(true);
						}
						if (stageSkinVariant.replacementRenderInfos.Length != 0)
						{
							this.characterModel.rendererInfos = stageSkinVariant.replacementRenderInfos;
						}
					}
					else
					{
						for (int k = 0; k < stageSkinVariant.childObjects.Length; k++)
						{
							stageSkinVariant.childObjects[k].SetActive(false);
						}
					}
				}
			}
		}

		// Token: 0x0400196C RID: 6508
		public StageSkinVariantController.StageSkinVariant[] stageSkinVariants;

		// Token: 0x0400196D RID: 6509
		public CharacterModel characterModel;

		// Token: 0x020003F3 RID: 1011
		[Serializable]
		public struct StageSkinVariant
		{
			// Token: 0x0400196E RID: 6510
			public string stageNameToken;

			// Token: 0x0400196F RID: 6511
			public CharacterModel.RendererInfo[] replacementRenderInfos;

			// Token: 0x04001970 RID: 6512
			public GameObject[] childObjects;
		}
	}
}
