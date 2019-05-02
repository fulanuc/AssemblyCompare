using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003AA RID: 938
	public class RandomizeSplatBias : MonoBehaviour
	{
		// Token: 0x060013F0 RID: 5104 RVA: 0x0000F300 File Offset: 0x0000D500
		private void Start()
		{
			this.materialsList = new List<Material>();
			this.rendererList = new List<Renderer>();
			this.printShader = Resources.Load<Shader>("Shaders/ToonLitCustom");
			this.Setup();
		}

		// Token: 0x060013F1 RID: 5105 RVA: 0x0006EA3C File Offset: 0x0006CC3C
		private void Setup()
		{
			this.hasSetupOnce = true;
			this.characterModel = base.GetComponent<CharacterModel>();
			if (this.characterModel)
			{
				for (int i = 0; i < this.characterModel.rendererInfos.Length; i++)
				{
					CharacterModel.RendererInfo rendererInfo = this.characterModel.rendererInfos[i];
					Material material = UnityEngine.Object.Instantiate<Material>(rendererInfo.defaultMaterial);
					if (material.shader == this.printShader)
					{
						this.materialsList.Add(material);
						this.rendererList.Add(rendererInfo.renderer);
						rendererInfo.defaultMaterial = material;
						this.characterModel.rendererInfos[i] = rendererInfo;
					}
					Renderer renderer = this.rendererList[i];
					this._propBlock = new MaterialPropertyBlock();
					renderer.GetPropertyBlock(this._propBlock);
					this._propBlock.SetFloat("_RedChannelBias", UnityEngine.Random.Range(this.minRedBias, this.maxRedBias));
					this._propBlock.SetFloat("_BlueChannelBias", UnityEngine.Random.Range(this.minBlueBias, this.maxBlueBias));
					this._propBlock.SetFloat("_GreenChannelBias", UnityEngine.Random.Range(this.minGreenBias, this.maxGreenBias));
					renderer.SetPropertyBlock(this._propBlock);
				}
				return;
			}
			Renderer componentInChildren = base.GetComponentInChildren<Renderer>();
			Material material2 = UnityEngine.Object.Instantiate<Material>(componentInChildren.material);
			this.materialsList.Add(material2);
			componentInChildren.material = material2;
			this._propBlock = new MaterialPropertyBlock();
			componentInChildren.GetPropertyBlock(this._propBlock);
			this._propBlock.SetFloat("_RedChannelBias", UnityEngine.Random.Range(this.minRedBias, this.maxRedBias));
			this._propBlock.SetFloat("_BlueChannelBias", UnityEngine.Random.Range(this.minBlueBias, this.maxBlueBias));
			this._propBlock.SetFloat("_GreenChannelBias", UnityEngine.Random.Range(this.minGreenBias, this.maxGreenBias));
			componentInChildren.SetPropertyBlock(this._propBlock);
		}

		// Token: 0x060013F2 RID: 5106 RVA: 0x0006EC30 File Offset: 0x0006CE30
		private void OnDestroy()
		{
			if (this.materialsList != null)
			{
				for (int i = 0; i < this.materialsList.Count; i++)
				{
					UnityEngine.Object.Destroy(this.materialsList[i]);
				}
			}
		}

		// Token: 0x04001787 RID: 6023
		public float minRedBias;

		// Token: 0x04001788 RID: 6024
		public float maxRedBias;

		// Token: 0x04001789 RID: 6025
		public float minGreenBias;

		// Token: 0x0400178A RID: 6026
		public float maxGreenBias;

		// Token: 0x0400178B RID: 6027
		public float minBlueBias;

		// Token: 0x0400178C RID: 6028
		public float maxBlueBias;

		// Token: 0x0400178D RID: 6029
		private MaterialPropertyBlock _propBlock;

		// Token: 0x0400178E RID: 6030
		private CharacterModel characterModel;

		// Token: 0x0400178F RID: 6031
		private List<Material> materialsList;

		// Token: 0x04001790 RID: 6032
		private List<Renderer> rendererList;

		// Token: 0x04001791 RID: 6033
		private Shader printShader;

		// Token: 0x04001792 RID: 6034
		private bool hasSetupOnce;
	}
}
