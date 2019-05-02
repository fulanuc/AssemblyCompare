using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000344 RID: 836
	public class ItemFollower : MonoBehaviour
	{
		// Token: 0x06001156 RID: 4438 RVA: 0x00065974 File Offset: 0x00063B74
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

		// Token: 0x06001157 RID: 4439 RVA: 0x00065A00 File Offset: 0x00063C00
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

		// Token: 0x06001158 RID: 4440 RVA: 0x0000D3EE File Offset: 0x0000B5EE
		private void OnDestroy()
		{
			if (this.followerInstance)
			{
				UnityEngine.Object.Destroy(this.followerInstance);
			}
		}

		// Token: 0x04001551 RID: 5457
		public GameObject followerPrefab;

		// Token: 0x04001552 RID: 5458
		public GameObject targetObject;

		// Token: 0x04001553 RID: 5459
		public BezierCurveLine followerCurve;

		// Token: 0x04001554 RID: 5460
		public float distanceDampTime;

		// Token: 0x04001555 RID: 5461
		public float distanceMaxSpeed;

		// Token: 0x04001556 RID: 5462
		private Vector3 velocityDistance;

		// Token: 0x04001557 RID: 5463
		private Vector3 v0;

		// Token: 0x04001558 RID: 5464
		private Vector3 v1;

		// Token: 0x04001559 RID: 5465
		[HideInInspector]
		public GameObject followerInstance;
	}
}
