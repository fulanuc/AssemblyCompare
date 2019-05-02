using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000345 RID: 837
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	public class ItemDisplayRuleComponent : MonoBehaviour
	{
		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06001160 RID: 4448 RVA: 0x0000D43E File Offset: 0x0000B63E
		// (set) Token: 0x06001161 RID: 4449 RVA: 0x0000D446 File Offset: 0x0000B646
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

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06001162 RID: 4450 RVA: 0x0000D45E File Offset: 0x0000B65E
		// (set) Token: 0x06001163 RID: 4451 RVA: 0x0000D466 File Offset: 0x0000B666
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

		// Token: 0x06001164 RID: 4452 RVA: 0x0000D490 File Offset: 0x0000B690
		private void Start()
		{
			this.BuildPreview();
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x0000D498 File Offset: 0x0000B698
		private void DestroyPreview()
		{
			if (this.prefabInstance)
			{
				UnityEngine.Object.DestroyImmediate(this.prefabInstance);
			}
			this.prefabInstance = null;
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x00065A9C File Offset: 0x00063C9C
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

		// Token: 0x06001167 RID: 4455 RVA: 0x00065B48 File Offset: 0x00063D48
		private static void SetPreviewFlags(Transform transform)
		{
			transform.gameObject.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset);
			foreach (object obj in transform)
			{
				ItemDisplayRuleComponent.SetPreviewFlags((Transform)obj);
			}
		}

		// Token: 0x06001168 RID: 4456 RVA: 0x0000D4B9 File Offset: 0x0000B6B9
		private void OnDestroy()
		{
			this.DestroyPreview();
		}

		// Token: 0x0400155F RID: 5471
		public ItemIndex itemIndex = ItemIndex.None;

		// Token: 0x04001560 RID: 5472
		public EquipmentIndex equipmentIndex = EquipmentIndex.None;

		// Token: 0x04001561 RID: 5473
		public LimbFlags limbMask;

		// Token: 0x04001562 RID: 5474
		[SerializeField]
		[HideInInspector]
		private ItemDisplayRuleType _ruleType;

		// Token: 0x04001563 RID: 5475
		public string nameInLocator;

		// Token: 0x04001564 RID: 5476
		[SerializeField]
		[HideInInspector]
		private GameObject _prefab;

		// Token: 0x04001565 RID: 5477
		private GameObject prefabInstance;
	}
}
