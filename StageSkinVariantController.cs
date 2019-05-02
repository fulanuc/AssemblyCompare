using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003EC RID: 1004
	public class StageSkinVariantController : MonoBehaviour
	{
		// Token: 0x06001602 RID: 5634 RVA: 0x000759A8 File Offset: 0x00073BA8
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

		// Token: 0x04001943 RID: 6467
		public StageSkinVariantController.StageSkinVariant[] stageSkinVariants;

		// Token: 0x04001944 RID: 6468
		public CharacterModel characterModel;

		// Token: 0x020003ED RID: 1005
		[Serializable]
		public struct StageSkinVariant
		{
			// Token: 0x04001945 RID: 6469
			public string stageNameToken;

			// Token: 0x04001946 RID: 6470
			public CharacterModel.RendererInfo[] replacementRenderInfos;

			// Token: 0x04001947 RID: 6471
			public GameObject[] childObjects;
		}
	}
}
