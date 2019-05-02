using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003DE RID: 990
	[RequireComponent(typeof(WheelCollider))]
	public class SetWheelVisuals : MonoBehaviour
	{
		// Token: 0x0600159F RID: 5535 RVA: 0x000105D8 File Offset: 0x0000E7D8
		private void Start()
		{
			this.wheelCollider = base.GetComponent<WheelCollider>();
		}

		// Token: 0x060015A0 RID: 5536 RVA: 0x00073978 File Offset: 0x00071B78
		private void FixedUpdate()
		{
			Vector3 position;
			Quaternion rotation;
			this.wheelCollider.GetWorldPose(out position, out rotation);
			this.visualTransform.position = position;
			this.visualTransform.rotation = rotation;
		}

		// Token: 0x040018C7 RID: 6343
		public Transform visualTransform;

		// Token: 0x040018C8 RID: 6344
		private WheelCollider wheelCollider;
	}
}
