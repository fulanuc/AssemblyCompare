using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003D8 RID: 984
	[RequireComponent(typeof(WheelCollider))]
	public class SetWheelVisuals : MonoBehaviour
	{
		// Token: 0x06001561 RID: 5473 RVA: 0x000101CF File Offset: 0x0000E3CF
		private void Start()
		{
			this.wheelCollider = base.GetComponent<WheelCollider>();
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x000732F8 File Offset: 0x000714F8
		private void FixedUpdate()
		{
			Vector3 position;
			Quaternion rotation;
			this.wheelCollider.GetWorldPose(out position, out rotation);
			this.visualTransform.position = position;
			this.visualTransform.rotation = rotation;
		}

		// Token: 0x0400189E RID: 6302
		public Transform visualTransform;

		// Token: 0x0400189F RID: 6303
		private WheelCollider wheelCollider;
	}
}
