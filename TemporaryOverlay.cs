using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000402 RID: 1026
	public class TemporaryOverlay : MonoBehaviour
	{
		// Token: 0x06001709 RID: 5897 RVA: 0x00011530 File Offset: 0x0000F730
		private void Start()
		{
			this.SetupMaterial();
			if (this.inspectorCharacterModel)
			{
				this.AddToCharacerModel(this.inspectorCharacterModel);
			}
		}

		// Token: 0x0600170A RID: 5898 RVA: 0x00011551 File Offset: 0x0000F751
		private void SetupMaterial()
		{
			if (!this.materialInstance)
			{
				this.materialInstance = new Material(this.originalMaterial);
			}
		}

		// Token: 0x0600170B RID: 5899 RVA: 0x00011571 File Offset: 0x0000F771
		public void AddToCharacerModel(CharacterModel characterModel)
		{
			this.SetupMaterial();
			if (characterModel)
			{
				characterModel.temporaryOverlays.Add(this);
				this.isAssigned = true;
				this.assignedCharacterModel = characterModel;
			}
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x0001159B File Offset: 0x0000F79B
		public void RemoveFromCharacterModel()
		{
			if (this.assignedCharacterModel)
			{
				this.assignedCharacterModel.temporaryOverlays.Remove(this);
				this.isAssigned = false;
				this.assignedCharacterModel = null;
			}
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x000115CA File Offset: 0x0000F7CA
		private void OnDestroy()
		{
			this.RemoveFromCharacterModel();
			if (this.materialInstance)
			{
				UnityEngine.Object.Destroy(this.materialInstance);
			}
		}

		// Token: 0x0600170E RID: 5902 RVA: 0x00078990 File Offset: 0x00076B90
		private void Update()
		{
			if (this.animateShaderAlpha)
			{
				this.stopwatch += Time.deltaTime;
				float value = this.alphaCurve.Evaluate(this.stopwatch / this.duration);
				this.materialInstance.SetFloat("_ExternalAlpha", value);
				if (this.stopwatch >= this.duration && (this.destroyComponentOnEnd || this.destroyObjectOnEnd))
				{
					if (this.destroyEffectPrefab)
					{
						ChildLocator component = base.GetComponent<ChildLocator>();
						if (component)
						{
							Transform transform = component.FindChild(this.destroyEffectChildString);
							if (transform)
							{
								EffectManager.instance.SpawnEffect(this.destroyEffectPrefab, new EffectData
								{
									origin = transform.position,
									rotation = transform.rotation
								}, true);
							}
						}
					}
					if (this.destroyObjectOnEnd)
					{
						UnityEngine.Object.Destroy(base.gameObject);
						return;
					}
					UnityEngine.Object.Destroy(this);
				}
			}
		}

		// Token: 0x040019FC RID: 6652
		public Material originalMaterial;

		// Token: 0x040019FD RID: 6653
		[HideInInspector]
		public Material materialInstance;

		// Token: 0x040019FE RID: 6654
		private bool isAssigned;

		// Token: 0x040019FF RID: 6655
		private CharacterModel assignedCharacterModel;

		// Token: 0x04001A00 RID: 6656
		public CharacterModel inspectorCharacterModel;

		// Token: 0x04001A01 RID: 6657
		public bool animateShaderAlpha;

		// Token: 0x04001A02 RID: 6658
		public AnimationCurve alphaCurve;

		// Token: 0x04001A03 RID: 6659
		public float duration;

		// Token: 0x04001A04 RID: 6660
		public bool destroyComponentOnEnd;

		// Token: 0x04001A05 RID: 6661
		public bool destroyObjectOnEnd;

		// Token: 0x04001A06 RID: 6662
		public GameObject destroyEffectPrefab;

		// Token: 0x04001A07 RID: 6663
		public string destroyEffectChildString;

		// Token: 0x04001A08 RID: 6664
		private float stopwatch;
	}
}
