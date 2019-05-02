using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000560 RID: 1376
	[RequireComponent(typeof(ProjectileTargetComponent))]
	public class ProjectileSteerTowardTarget : MonoBehaviour
	{
		// Token: 0x06001ED4 RID: 7892 RVA: 0x0001693D File Offset: 0x00014B3D
		private void Start()
		{
			if (!NetworkServer.active)
			{
				base.enabled = false;
				return;
			}
			this.transform = base.transform;
			this.targetComponent = base.GetComponent<ProjectileTargetComponent>();
		}

		// Token: 0x06001ED5 RID: 7893 RVA: 0x00097ADC File Offset: 0x00095CDC
		private void FixedUpdate()
		{
			if (this.targetComponent.target)
			{
				Vector3 vector = this.targetComponent.target.transform.position - this.transform.position;
				if (this.yAxisOnly)
				{
					vector.y = 0f;
				}
				if (vector != Vector3.zero)
				{
					this.transform.forward = Vector3.RotateTowards(this.transform.forward, vector, this.rotationSpeed * 0.0174532924f * Time.fixedDeltaTime, 0f);
				}
			}
		}

		// Token: 0x0400216A RID: 8554
		[Tooltip("Constrains rotation to the Y axis only.")]
		public bool yAxisOnly;

		// Token: 0x0400216B RID: 8555
		[Tooltip("How fast to rotate in degrees per second. Rotation is linear.")]
		public float rotationSpeed;

		// Token: 0x0400216C RID: 8556
		private new Transform transform;

		// Token: 0x0400216D RID: 8557
		private ProjectileTargetComponent targetComponent;
	}
}
