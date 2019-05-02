using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200039F RID: 927
	public class PrintController : MonoBehaviour
	{
		// Token: 0x060013A0 RID: 5024 RVA: 0x0000F04C File Offset: 0x0000D24C
		private void Awake()
		{
			this.UpdatePrint(0f);
		}

		// Token: 0x060013A1 RID: 5025 RVA: 0x0006D2A4 File Offset: 0x0006B4A4
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

		// Token: 0x060013A2 RID: 5026 RVA: 0x0006D2E8 File Offset: 0x0006B4E8
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

		// Token: 0x060013A3 RID: 5027 RVA: 0x0000F059 File Offset: 0x0000D259
		private void Update()
		{
			this.UpdatePrint(Time.deltaTime);
		}

		// Token: 0x060013A4 RID: 5028 RVA: 0x0000F066 File Offset: 0x0000D266
		public void SetPaused(bool newPaused)
		{
			this.paused = newPaused;
		}

		// Token: 0x060013A5 RID: 5029 RVA: 0x0006D334 File Offset: 0x0006B534
		private void UpdatePrint(float deltaTime)
		{
			this.SetupPrint();
			if (this.printCurve == null)
			{
				return;
			}
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

		// Token: 0x060013A6 RID: 5030 RVA: 0x0006D470 File Offset: 0x0006B670
		private void SetupPrint()
		{
			if (this.hasSetupOnce)
			{
				return;
			}
			this.materialsList = new List<Material>();
			this.rendererList = new List<Renderer>();
			this.printShader = Resources.Load<Shader>("Shaders/ToonLitCustom");
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

		// Token: 0x060013A7 RID: 5031 RVA: 0x0006D5A8 File Offset: 0x0006B7A8
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

		// Token: 0x0400172E RID: 5934
		public float printTime;

		// Token: 0x0400172F RID: 5935
		public AnimationCurve printCurve;

		// Token: 0x04001730 RID: 5936
		public float startingPrintHeight;

		// Token: 0x04001731 RID: 5937
		public float maxPrintHeight;

		// Token: 0x04001732 RID: 5938
		public float startingPrintBias;

		// Token: 0x04001733 RID: 5939
		public float maxPrintBias;

		// Token: 0x04001734 RID: 5940
		public bool animateFlowmapPower;

		// Token: 0x04001735 RID: 5941
		public float startingFlowmapPower;

		// Token: 0x04001736 RID: 5942
		public float maxFlowmapPower;

		// Token: 0x04001737 RID: 5943
		public bool disableWhenFinished = true;

		// Token: 0x04001738 RID: 5944
		public bool paused;

		// Token: 0x04001739 RID: 5945
		private MaterialPropertyBlock _propBlock;

		// Token: 0x0400173A RID: 5946
		private CharacterModel characterModel;

		// Token: 0x0400173B RID: 5947
		private List<Material> materialsList;

		// Token: 0x0400173C RID: 5948
		private List<Renderer> rendererList;

		// Token: 0x0400173D RID: 5949
		public float age;

		// Token: 0x0400173E RID: 5950
		private Shader printShader;

		// Token: 0x0400173F RID: 5951
		private bool hasSetupOnce;
	}
}
