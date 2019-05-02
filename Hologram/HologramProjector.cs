using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Hologram
{
	// Token: 0x0200056D RID: 1389
	public class HologramProjector : MonoBehaviour
	{
		// Token: 0x06001EFB RID: 7931 RVA: 0x00016AD9 File Offset: 0x00014CD9
		private void Awake()
		{
			this.contentProvider = base.GetComponent<IHologramContentProvider>();
		}

		// Token: 0x06001EFC RID: 7932 RVA: 0x00098204 File Offset: 0x00096404
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

		// Token: 0x06001EFD RID: 7933 RVA: 0x000982AC File Offset: 0x000964AC
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

		// Token: 0x06001EFE RID: 7934 RVA: 0x000983F4 File Offset: 0x000965F4
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

		// Token: 0x06001EFF RID: 7935 RVA: 0x00016AE7 File Offset: 0x00014CE7
		private void DestroyHologram()
		{
			if (this.hologramContentInstance)
			{
				UnityEngine.Object.Destroy(this.hologramContentInstance);
			}
			this.hologramContentInstance = null;
		}

		// Token: 0x0400219B RID: 8603
		[Tooltip("The range in meters at which the hologram begins to display.")]
		public float displayDistance = 15f;

		// Token: 0x0400219C RID: 8604
		[Tooltip("The position at which to display the hologram.")]
		public Transform hologramPivot;

		// Token: 0x0400219D RID: 8605
		[Tooltip("Whether or not the hologram will pivot to the player")]
		public bool disableHologramRotation;

		// Token: 0x0400219E RID: 8606
		private float transformDampVelocity;

		// Token: 0x0400219F RID: 8607
		private IHologramContentProvider contentProvider;

		// Token: 0x040021A0 RID: 8608
		private float viewerReselectTimer;

		// Token: 0x040021A1 RID: 8609
		private float viewerReselectInterval = 0.25f;

		// Token: 0x040021A2 RID: 8610
		private Transform cachedViewer;

		// Token: 0x040021A3 RID: 8611
		private Transform viewer;

		// Token: 0x040021A4 RID: 8612
		private GameObject hologramContentInstance;
	}
}
