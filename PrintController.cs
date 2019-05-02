using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200039A RID: 922
	public class PrintController : MonoBehaviour
	{
		// Token: 0x06001383 RID: 4995 RVA: 0x0006D08C File Offset: 0x0006B28C
		private void Awake()
		{
			if (!this.hasSetupOnce)
			{
				this.materialsList = new List<Material>();
				this.rendererList = new List<Renderer>();
				this.printShader = Resources.Load<Shader>("Shaders/ToonLitCustom");
				this.SetupPrint();
			}
			this.UpdatePrint(0f);
		}

		// Token: 0x06001384 RID: 4996 RVA: 0x0006D0D8 File Offset: 0x0006B2D8
		private void OnDisable()
		{
			if (this.hasSetupOnce)
			{
				for (int i = 0; i < this.materialsList.Count; i++)
				{
					this.materialsList[i].DisableKeyword("PRINT_CUTOFF");
				}
			}
		}

		// Token: 0x06001385 RID: 4997 RVA: 0x0006D11C File Offset: 0x0006B31C
		private void OnEnable()
		{
			if (this.hasSetupOnce)
			{
				for (int i = 0; i < this.materialsList.Count; i++)
				{
					this.materialsList[i].EnableKeyword("PRINT_CUTOFF");
				}
			}
			this.age = 0f;
		}

		// Token: 0x06001386 RID: 4998 RVA: 0x0000EE8F File Offset: 0x0000D08F
		private void Update()
		{
			this.UpdatePrint(Time.deltaTime);
		}

		// Token: 0x06001387 RID: 4999 RVA: 0x0000EE9C File Offset: 0x0000D09C
		public void SetPaused(bool newPaused)
		{
			this.paused = newPaused;
		}

		// Token: 0x06001388 RID: 5000 RVA: 0x0006D168 File Offset: 0x0006B368
		private void UpdatePrint(float deltaTime)
		{
			if (!this.paused)
			{
				this.age += deltaTime;
			}
			float num = this.printCurve.Evaluate(this.age / this.printTime);
			for (int i = 0; i < this.rendererList.Count; i++)
			{
				Renderer renderer = this.rendererList[i];
				this._propBlock = new MaterialPropertyBlock();
				renderer.GetPropertyBlock(this._propBlock);
				this._propBlock.SetFloat("_SliceHeight", num * this.maxPrintHeight + (1f - num) * this.startingPrintHeight);
				this._propBlock.SetFloat("_PrintBias", num * this.maxPrintBias + (1f - num) * this.startingPrintBias);
				if (this.animateFlowmapPower)
				{
					this._propBlock.SetFloat("_FlowHeightPower", num * this.maxFlowmapPower + (1f - num) * this.startingFlowmapPower);
				}
				renderer.SetPropertyBlock(this._propBlock);
			}
			if (this.age >= this.printTime && this.disableWhenFinished)
			{
				base.enabled = false;
				this.age = 0f;
			}
		}

		// Token: 0x06001389 RID: 5001 RVA: 0x0006D294 File Offset: 0x0006B494
		private void SetupPrint()
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
						material.EnableKeyword("PRINT_CUTOFF");
						this.materialsList.Add(material);
						this.rendererList.Add(rendererInfo.renderer);
						rendererInfo.defaultMaterial = material;
						this.characterModel.rendererInfos[i] = rendererInfo;
					}
				}
			}
			else
			{
				foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>())
				{
					this.rendererList.Add(renderer);
					this.materialsList.Add(renderer.material);
				}
			}
			this.age = 0f;
		}

		// Token: 0x0600138A RID: 5002 RVA: 0x0006D3A0 File Offset: 0x0006B5A0
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

		// Token: 0x04001712 RID: 5906
		public float printTime;

		// Token: 0x04001713 RID: 5907
		public AnimationCurve printCurve;

		// Token: 0x04001714 RID: 5908
		public float startingPrintHeight;

		// Token: 0x04001715 RID: 5909
		public float maxPrintHeight;

		// Token: 0x04001716 RID: 5910
		public float startingPrintBias;

		// Token: 0x04001717 RID: 5911
		public float maxPrintBias;

		// Token: 0x04001718 RID: 5912
		public bool animateFlowmapPower;

		// Token: 0x04001719 RID: 5913
		public float startingFlowmapPower;

		// Token: 0x0400171A RID: 5914
		public float maxFlowmapPower;

		// Token: 0x0400171B RID: 5915
		public bool disableWhenFinished = true;

		// Token: 0x0400171C RID: 5916
		public bool paused;

		// Token: 0x0400171D RID: 5917
		private MaterialPropertyBlock _propBlock;

		// Token: 0x0400171E RID: 5918
		private CharacterModel characterModel;

		// Token: 0x0400171F RID: 5919
		private List<Material> materialsList;

		// Token: 0x04001720 RID: 5920
		private List<Renderer> rendererList;

		// Token: 0x04001721 RID: 5921
		public float age;

		// Token: 0x04001722 RID: 5922
		private Shader printShader;

		// Token: 0x04001723 RID: 5923
		private bool hasSetupOnce;
	}
}
