using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002F9 RID: 761
	public class FollowerItemDisplayComponent : MonoBehaviour
	{
		// Token: 0x06000F6B RID: 3947 RVA: 0x0000BD64 File Offset: 0x00009F64
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06000F6C RID: 3948 RVA: 0x0005CE30 File Offset: 0x0005B030
		private void LateUpdate()
		{
			if (!this.target)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			Quaternion rotation = this.target.rotation;
			this.transform.position = this.target.position + rotation * this.localPosition;
			this.transform.rotation = rotation * this.localRotation;
			this.transform.localScale = this.localScale;
		}

		// Token: 0x04001388 RID: 5000
		public Transform target;

		// Token: 0x04001389 RID: 5001
		public Vector3 localPosition;

		// Token: 0x0400138A RID: 5002
		public Quaternion localRotation;

		// Token: 0x0400138B RID: 5003
		public Vector3 localScale;

		// Token: 0x0400138C RID: 5004
		private new Transform transform;
	}
}
