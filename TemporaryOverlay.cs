using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003FC RID: 1020
	public class TemporaryOverlay : MonoBehaviour
	{
		// Token: 0x060016C9 RID: 5833 RVA: 0x0001110B File Offset: 0x0000F30B
		private void Start()
		{
			this.SetupMaterial();
			if (this.inspectorCharacterModel)
			{
				this.AddToCharacerModel(this.inspectorCharacterModel);
			}
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x0001112C File Offset: 0x0000F32C
		private void SetupMaterial()
		{
			if (!this.materialInstance)
			{
				this.materialInstance = new Material(this.originalMaterial);
			}
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x0001114C File Offset: 0x0000F34C
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

		// Token: 0x060016CC RID: 5836 RVA: 0x00011176 File Offset: 0x0000F376
		public void RemoveFromCharacterModel()
		{
			if (this.assignedCharacterModel)
			{
				this.assignedCharacterModel.temporaryOverlays.Remove(this);
				this.isAssigned = false;
				this.assignedCharacterModel = null;
			}
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x000111A5 File Offset: 0x0000F3A5
		private void OnDestroy()
		{
			this.RemoveFromCharacterModel();
			if (this.materialInstance)
			{
				UnityEngine.Object.Destroy(this.materialInstance);
			}
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x00078404 File Offset: 0x00076604
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

		// Token: 0x040019D3 RID: 6611
		public Material originalMaterial;

		// Token: 0x040019D4 RID: 6612
		[HideInInspector]
		public Material materialInstance;

		// Token: 0x040019D5 RID: 6613
		private bool isAssigned;

		// Token: 0x040019D6 RID: 6614
		private CharacterModel assignedCharacterModel;

		// Token: 0x040019D7 RID: 6615
		public CharacterModel inspectorCharacterModel;

		// Token: 0x040019D8 RID: 6616
		public bool animateShaderAlpha;

		// Token: 0x040019D9 RID: 6617
		public AnimationCurve alphaCurve;

		// Token: 0x040019DA RID: 6618
		public float duration;

		// Token: 0x040019DB RID: 6619
		public bool destroyComponentOnEnd;

		// Token: 0x040019DC RID: 6620
		public bool destroyObjectOnEnd;

		// Token: 0x040019DD RID: 6621
		public GameObject destroyEffectPrefab;

		// Token: 0x040019DE RID: 6622
		public string destroyEffectChildString;

		// Token: 0x040019DF RID: 6623
		private float stopwatch;
	}
}
