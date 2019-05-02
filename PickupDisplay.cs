using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200038A RID: 906
	public class PickupDisplay : MonoBehaviour
	{
		// Token: 0x060012ED RID: 4845 RVA: 0x0000E7A0 File Offset: 0x0000C9A0
		public void SetPickupIndex(PickupIndex newPickupIndex, bool newHidden = false)
		{
			if (this.pickupIndex == newPickupIndex && this.hidden == newHidden)
			{
				return;
			}
			this.pickupIndex = newPickupIndex;
			this.hidden = newHidden;
			this.RebuildModel();
		}

		// Token: 0x060012EE RID: 4846 RVA: 0x0000E7CE File Offset: 0x0000C9CE
		private void DestroyModel()
		{
			if (this.modelObject)
			{
				UnityEngine.Object.Destroy(this.modelObject);
				this.modelObject = null;
				this.modelRenderer = null;
			}
		}

		// Token: 0x060012EF RID: 4847 RVA: 0x0006AB24 File Offset: 0x00068D24
		private void RebuildModel()
		{
			GameObject y = this.hidden ? this.pickupIndex.GetHiddenPickupDisplayPrefab() : this.pickupIndex.GetPickupDisplayPrefab();
			if (this.modelPrefab == y)
			{
				return;
			}
			this.DestroyModel();
			this.modelPrefab = y;
			this.modelScale = base.transform.lossyScale.x;
			if (this.modelPrefab != null)
			{
				this.modelObject = UnityEngine.Object.Instantiate<GameObject>(this.modelPrefab);
				this.modelRenderer = this.modelObject.GetComponentInChildren<Renderer>();
				if (this.modelRenderer)
				{
					this.modelObject.transform.rotation = Quaternion.identity;
					Vector3 size = this.modelRenderer.bounds.size;
					float f = size.x * size.y * size.z;
					this.modelScale *= Mathf.Pow(PickupDisplay.idealVolume, 0.333333343f) / Mathf.Pow(f, 0.333333343f);
					if (this.highlight)
					{
						this.highlight.targetRenderer = this.modelRenderer;
						this.highlight.isOn = true;
						this.highlight.pickupIndex = this.pickupIndex;
					}
				}
				this.modelObject.transform.parent = base.transform;
				this.modelObject.transform.localPosition = this.localModelPivotPosition;
				this.modelObject.transform.localRotation = Quaternion.identity;
				this.modelObject.transform.localScale = new Vector3(this.modelScale, this.modelScale, this.modelScale);
			}
			if (this.pickupIndex.itemIndex != ItemIndex.None)
			{
				switch (ItemCatalog.GetItemDef(this.pickupIndex.itemIndex).tier)
				{
				case ItemTier.Tier1:
					if (this.tier1ParticleEffect)
					{
						this.tier1ParticleEffect.SetActive(true);
					}
					break;
				case ItemTier.Tier2:
					if (this.tier2ParticleEffect)
					{
						this.tier2ParticleEffect.SetActive(true);
					}
					break;
				case ItemTier.Tier3:
					if (this.tier3ParticleEffect)
					{
						this.tier3ParticleEffect.SetActive(true);
					}
					break;
				}
			}
			else if (this.pickupIndex.equipmentIndex != EquipmentIndex.None && this.equipmentParticleEffect)
			{
				this.equipmentParticleEffect.SetActive(true);
			}
			if (this.bossParticleEffect)
			{
				this.bossParticleEffect.SetActive(this.pickupIndex.IsBoss());
			}
			if (this.lunarParticleEffect)
			{
				this.lunarParticleEffect.SetActive(this.pickupIndex.IsLunar());
			}
			foreach (ParticleSystem particleSystem in this.coloredParticleSystems)
			{
				particleSystem.gameObject.SetActive(this.modelPrefab != null);
				particleSystem.main.startColor = this.pickupIndex.GetPickupColor();
			}
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060012F0 RID: 4848 RVA: 0x0000E7F6 File Offset: 0x0000C9F6
		// (set) Token: 0x060012F1 RID: 4849 RVA: 0x0000E7FE File Offset: 0x0000C9FE
		public Renderer modelRenderer { get; private set; }

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x060012F2 RID: 4850 RVA: 0x0000E807 File Offset: 0x0000CA07
		private Vector3 localModelPivotPosition
		{
			get
			{
				return Vector3.up * this.verticalWave.Evaluate(this.localTime);
			}
		}

		// Token: 0x060012F3 RID: 4851 RVA: 0x0000E824 File Offset: 0x0000CA24
		private void Start()
		{
			this.localTime = 0f;
		}

		// Token: 0x060012F4 RID: 4852 RVA: 0x0006AE20 File Offset: 0x00069020
		private void Update()
		{
			this.localTime += Time.deltaTime;
			if (this.modelObject)
			{
				Transform transform = this.modelObject.transform;
				Vector3 localEulerAngles = transform.localEulerAngles;
				localEulerAngles.y = this.spinSpeed * this.localTime;
				transform.localEulerAngles = localEulerAngles;
				transform.localPosition = this.localModelPivotPosition;
			}
		}

		// Token: 0x060012F5 RID: 4853 RVA: 0x0006AE84 File Offset: 0x00069084
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Matrix4x4 matrix = Gizmos.matrix;
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.lossyScale);
			Gizmos.DrawWireCube(Vector3.zero, PickupDisplay.idealModelBox);
			Gizmos.matrix = matrix;
		}

		// Token: 0x040016A6 RID: 5798
		[Tooltip("The vertical motion of the display model.")]
		public Wave verticalWave;

		// Token: 0x040016A7 RID: 5799
		[Tooltip("The speed in degrees/second at which the display model rotates about the y axis.")]
		public float spinSpeed = 75f;

		// Token: 0x040016A8 RID: 5800
		public GameObject tier1ParticleEffect;

		// Token: 0x040016A9 RID: 5801
		public GameObject tier2ParticleEffect;

		// Token: 0x040016AA RID: 5802
		public GameObject tier3ParticleEffect;

		// Token: 0x040016AB RID: 5803
		public GameObject equipmentParticleEffect;

		// Token: 0x040016AC RID: 5804
		public GameObject lunarParticleEffect;

		// Token: 0x040016AD RID: 5805
		public GameObject bossParticleEffect;

		// Token: 0x040016AE RID: 5806
		[Tooltip("The particle system to tint.")]
		public ParticleSystem[] coloredParticleSystems;

		// Token: 0x040016AF RID: 5807
		private PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x040016B0 RID: 5808
		private bool hidden;

		// Token: 0x040016B1 RID: 5809
		public Highlight highlight;

		// Token: 0x040016B2 RID: 5810
		private static readonly Vector3 idealModelBox = Vector3.one;

		// Token: 0x040016B3 RID: 5811
		private static readonly float idealVolume = PickupDisplay.idealModelBox.x * PickupDisplay.idealModelBox.y * PickupDisplay.idealModelBox.z;

		// Token: 0x040016B4 RID: 5812
		private GameObject modelObject;

		// Token: 0x040016B6 RID: 5814
		private GameObject modelPrefab;

		// Token: 0x040016B7 RID: 5815
		private float modelScale;

		// Token: 0x040016B8 RID: 5816
		private float localTime;
	}
}
