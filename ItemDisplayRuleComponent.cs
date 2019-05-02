using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000343 RID: 835
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	public class ItemDisplayRuleComponent : MonoBehaviour
	{
		// Token: 0x1700017F RID: 383
		// (get) Token: 0x0600114C RID: 4428 RVA: 0x0000D355 File Offset: 0x0000B555
		// (set) Token: 0x0600114D RID: 4429 RVA: 0x0000D35D File Offset: 0x0000B55D
		public ItemDisplayRuleType ruleType
		{
			get
			{
				return this._ruleType;
			}
			set
			{
				this._ruleType = value;
				if (this._ruleType != ItemDisplayRuleType.ParentedPrefab)
				{
					this.prefab = null;
				}
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x0600114E RID: 4430 RVA: 0x0000D375 File Offset: 0x0000B575
		// (set) Token: 0x0600114F RID: 4431 RVA: 0x0000D37D File Offset: 0x0000B57D
		public GameObject prefab
		{
			get
			{
				return this._prefab;
			}
			set
			{
				if (!this.prefabInstance || this._prefab != value)
				{
					this._prefab = value;
					this.BuildPreview();
				}
			}
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x0000D3A7 File Offset: 0x0000B5A7
		private void Start()
		{
			this.BuildPreview();
		}

		// Token: 0x06001151 RID: 4433 RVA: 0x0000D3AF File Offset: 0x0000B5AF
		private void DestroyPreview()
		{
			if (this.prefabInstance)
			{
				UnityEngine.Object.DestroyImmediate(this.prefabInstance);
			}
			this.prefabInstance = null;
		}

		// Token: 0x06001152 RID: 4434 RVA: 0x00065868 File Offset: 0x00063A68
		private void BuildPreview()
		{
			this.DestroyPreview();
			if (this.prefab)
			{
				this.prefabInstance = UnityEngine.Object.Instantiate<GameObject>(this.prefab);
				this.prefabInstance.name = "Preview";
				this.prefabInstance.transform.parent = base.transform;
				this.prefabInstance.transform.localPosition = Vector3.zero;
				this.prefabInstance.transform.localRotation = Quaternion.identity;
				this.prefabInstance.transform.localScale = Vector3.one;
				ItemDisplayRuleComponent.SetPreviewFlags(this.prefabInstance.transform);
			}
		}

		// Token: 0x06001153 RID: 4435 RVA: 0x00065914 File Offset: 0x00063B14
		private static void SetPreviewFlags(Transform transform)
		{
			transform.gameObject.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset);
			foreach (object obj in transform)
			{
				ItemDisplayRuleComponent.SetPreviewFlags((Transform)obj);
			}
		}

		// Token: 0x06001154 RID: 4436 RVA: 0x0000D3D0 File Offset: 0x0000B5D0
		private void OnDestroy()
		{
			this.DestroyPreview();
		}

		// Token: 0x0400154A RID: 5450
		public ItemIndex itemIndex = ItemIndex.None;

		// Token: 0x0400154B RID: 5451
		public EquipmentIndex equipmentIndex = EquipmentIndex.None;

		// Token: 0x0400154C RID: 5452
		public LimbFlags limbMask;

		// Token: 0x0400154D RID: 5453
		[HideInInspector]
		[SerializeField]
		private ItemDisplayRuleType _ruleType;

		// Token: 0x0400154E RID: 5454
		public string nameInLocator;

		// Token: 0x0400154F RID: 5455
		[SerializeField]
		[HideInInspector]
		private GameObject _prefab;

		// Token: 0x04001550 RID: 5456
		private GameObject prefabInstance;
	}
}
