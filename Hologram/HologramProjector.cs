using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Hologram
{
	// Token: 0x0200057C RID: 1404
	public class HologramProjector : MonoBehaviour
	{
		// Token: 0x06001F65 RID: 8037 RVA: 0x00016FB8 File Offset: 0x000151B8
		private void Awake()
		{
			this.contentProvider = base.GetComponent<IHologramContentProvider>();
		}

		// Token: 0x06001F66 RID: 8038 RVA: 0x00098F20 File Offset: 0x00097120
		private Transform FindViewer(Vector3 position)
		{
			if (this.viewerReselectTimer > 0f)
			{
				return this.cachedViewer;
			}
			this.viewerReselectTimer = this.viewerReselectInterval;
			this.cachedViewer = null;
			float num = float.PositiveInfinity;
			ReadOnlyCollection<PlayerCharacterMasterController> instances = PlayerCharacterMasterController.instances;
			int i = 0;
			int count = instances.Count;
			while (i < count)
			{
				GameObject bodyObject = instances[i].master.GetBodyObject();
				if (bodyObject)
				{
					float sqrMagnitude = (bodyObject.transform.position - position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						this.cachedViewer = bodyObject.transform;
					}
				}
				i++;
			}
			return this.cachedViewer;
		}

		// Token: 0x06001F67 RID: 8039 RVA: 0x00098FC8 File Offset: 0x000971C8
		private void Update()
		{
			this.viewerReselectTimer -= Time.deltaTime;
			Vector3 vector = this.hologramPivot ? this.hologramPivot.position : base.transform.position;
			this.viewer = this.FindViewer(vector);
			Vector3 b = this.viewer ? this.viewer.position : base.transform.position;
			bool flag = false;
			Vector3 forward = Vector3.zero;
			if (this.viewer)
			{
				forward = vector - b;
				if (forward.sqrMagnitude <= this.displayDistance * this.displayDistance)
				{
					flag = true;
				}
			}
			if (flag)
			{
				flag = this.contentProvider.ShouldDisplayHologram(this.viewer.gameObject);
			}
			if (flag)
			{
				if (!this.hologramContentInstance)
				{
					this.BuildHologram();
				}
				if (this.hologramContentInstance && this.contentProvider != null)
				{
					this.contentProvider.UpdateHologramContent(this.hologramContentInstance);
					if (!this.disableHologramRotation)
					{
						this.hologramContentInstance.transform.rotation = Util.SmoothDampQuaternion(this.hologramContentInstance.transform.rotation, Util.QuaternionSafeLookRotation(forward), ref this.transformDampVelocity, 0.2f);
						return;
					}
				}
			}
			else
			{
				this.DestroyHologram();
			}
		}

		// Token: 0x06001F68 RID: 8040 RVA: 0x00099110 File Offset: 0x00097310
		private void BuildHologram()
		{
			this.DestroyHologram();
			if (this.contentProvider != null)
			{
				GameObject hologramContentPrefab = this.contentProvider.GetHologramContentPrefab();
				if (hologramContentPrefab)
				{
					this.hologramContentInstance = UnityEngine.Object.Instantiate<GameObject>(hologramContentPrefab);
					this.hologramContentInstance.transform.parent = (this.hologramPivot ? this.hologramPivot : base.transform);
					this.hologramContentInstance.transform.localPosition = Vector3.zero;
					this.hologramContentInstance.transform.localRotation = Quaternion.identity;
					this.hologramContentInstance.transform.localScale = Vector3.one;
					if (this.viewer && !this.disableHologramRotation)
					{
						Vector3 a = this.hologramPivot ? this.hologramPivot.position : base.transform.position;
						Vector3 position = this.viewer.position;
						Vector3 forward = a - this.viewer.position;
						this.hologramContentInstance.transform.rotation = Util.QuaternionSafeLookRotation(forward);
					}
					this.contentProvider.UpdateHologramContent(this.hologramContentInstance);
				}
			}
		}

		// Token: 0x06001F69 RID: 8041 RVA: 0x00016FC6 File Offset: 0x000151C6
		private void DestroyHologram()
		{
			if (this.hologramContentInstance)
			{
				UnityEngine.Object.Destroy(this.hologramContentInstance);
			}
			this.hologramContentInstance = null;
		}

		// Token: 0x040021D9 RID: 8665
		[Tooltip("The range in meters at which the hologram begins to display.")]
		public float displayDistance = 15f;

		// Token: 0x040021DA RID: 8666
		[Tooltip("The position at which to display the hologram.")]
		public Transform hologramPivot;

		// Token: 0x040021DB RID: 8667
		[Tooltip("Whether or not the hologram will pivot to the player")]
		public bool disableHologramRotation;

		// Token: 0x040021DC RID: 8668
		private float transformDampVelocity;

		// Token: 0x040021DD RID: 8669
		private IHologramContentProvider contentProvider;

		// Token: 0x040021DE RID: 8670
		private float viewerReselectTimer;

		// Token: 0x040021DF RID: 8671
		private float viewerReselectInterval = 0.25f;

		// Token: 0x040021E0 RID: 8672
		private Transform cachedViewer;

		// Token: 0x040021E1 RID: 8673
		private Transform viewer;

		// Token: 0x040021E2 RID: 8674
		private GameObject hologramContentInstance;
	}
}
