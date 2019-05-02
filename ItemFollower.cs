using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000346 RID: 838
	public class ItemFollower : MonoBehaviour
	{
		// Token: 0x0600116A RID: 4458 RVA: 0x00065BA8 File Offset: 0x00063DA8
		private void Start()
		{
			if (!this.followerInstance)
			{
				this.followerInstance = UnityEngine.Object.Instantiate<GameObject>(this.followerPrefab, this.targetObject.transform.position, Quaternion.identity);
				this.followerInstance.transform.localScale = base.transform.localScale;
			}
			if (this.followerCurve)
			{
				this.v0 = this.followerCurve.v0;
				this.v1 = this.followerCurve.v1;
			}
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x00065C34 File Offset: 0x00063E34
		private void Update()
		{
			Transform transform = this.followerInstance.transform;
			Transform transform2 = this.targetObject.transform;
			transform.position = Vector3.SmoothDamp(transform.position, transform2.position, ref this.velocityDistance, this.distanceDampTime);
			transform.rotation = transform2.rotation;
			if (this.followerCurve)
			{
				this.followerCurve.v0 = base.transform.TransformVector(this.v0);
				this.followerCurve.v1 = transform.TransformVector(this.v1);
				this.followerCurve.p1 = transform.position;
			}
		}

		// Token: 0x0600116C RID: 4460 RVA: 0x0000D4D7 File Offset: 0x0000B6D7
		private void OnDestroy()
		{
			if (this.followerInstance)
			{
				UnityEngine.Object.Destroy(this.followerInstance);
			}
		}

		// Token: 0x04001566 RID: 5478
		public GameObject followerPrefab;

		// Token: 0x04001567 RID: 5479
		public GameObject targetObject;

		// Token: 0x04001568 RID: 5480
		public BezierCurveLine followerCurve;

		// Token: 0x04001569 RID: 5481
		public float distanceDampTime;

		// Token: 0x0400156A RID: 5482
		public float distanceMaxSpeed;

		// Token: 0x0400156B RID: 5483
		private Vector3 velocityDistance;

		// Token: 0x0400156C RID: 5484
		private Vector3 v0;

		// Token: 0x0400156D RID: 5485
		private Vector3 v1;

		// Token: 0x0400156E RID: 5486
		[HideInInspector]
		public GameObject followerInstance;
	}
}
