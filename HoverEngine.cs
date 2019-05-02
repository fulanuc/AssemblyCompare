using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000315 RID: 789
	public class HoverEngine : MonoBehaviour
	{
		// Token: 0x0600106C RID: 4204 RVA: 0x000622C0 File Offset: 0x000604C0
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.transform.TransformPoint(this.offsetVector), this.hoverRadius);
			Gizmos.DrawRay(this.castRay);
			if (this.isGrounded)
			{
				Gizmos.DrawSphere(this.raycastHit.point, this.hoverRadius);
			}
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x0006231C File Offset: 0x0006051C
		private void FixedUpdate()
		{
			float num = Mathf.Clamp01(Vector3.Dot(-base.transform.up, Vector3.down));
			this.castPosition = base.transform.TransformPoint(this.offsetVector);
			this.castRay = new Ray(this.castPosition, -base.transform.up);
			this.isGrounded = false;
			this.forceStrength = 0f;
			this.compression = 0f;
			Vector3 position = this.castRay.origin + this.castRay.direction * this.hoverHeight;
			if (Physics.SphereCast(this.castRay, this.hoverRadius, out this.raycastHit, this.hoverHeight, LayerIndex.world.mask))
			{
				this.isGrounded = true;
				float num2 = (this.hoverHeight - this.raycastHit.distance) / this.hoverHeight;
				Vector3 a = Vector3.up * (num2 * this.hoverForce);
				Vector3 b = Vector3.Project(this.engineRigidbody.GetPointVelocity(this.castPosition), -base.transform.up) * this.hoverDamping;
				this.forceStrength = (a - b).magnitude;
				this.engineRigidbody.AddForceAtPosition(Vector3.Project(a - b, -base.transform.up), this.castPosition, ForceMode.Acceleration);
				this.compression = Mathf.Clamp01(num2 * num);
				position = this.raycastHit.point;
			}
			this.wheelVisual.position = position;
			bool flag = this.isGrounded;
		}

		// Token: 0x04001453 RID: 5203
		public Rigidbody engineRigidbody;

		// Token: 0x04001454 RID: 5204
		public Transform wheelVisual;

		// Token: 0x04001455 RID: 5205
		public float hoverForce = 65f;

		// Token: 0x04001456 RID: 5206
		public float hoverHeight = 3.5f;

		// Token: 0x04001457 RID: 5207
		public float hoverDamping = 0.1f;

		// Token: 0x04001458 RID: 5208
		public float hoverRadius;

		// Token: 0x04001459 RID: 5209
		[HideInInspector]
		public float forceStrength;

		// Token: 0x0400145A RID: 5210
		private Ray castRay;

		// Token: 0x0400145B RID: 5211
		private Vector3 castPosition;

		// Token: 0x0400145C RID: 5212
		[HideInInspector]
		public RaycastHit raycastHit;

		// Token: 0x0400145D RID: 5213
		public float compression;

		// Token: 0x0400145E RID: 5214
		public Vector3 offsetVector = Vector3.zero;

		// Token: 0x0400145F RID: 5215
		public bool isGrounded;
	}
}
