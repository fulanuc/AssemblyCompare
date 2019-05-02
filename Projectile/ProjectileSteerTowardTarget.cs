using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200056F RID: 1391
	[RequireComponent(typeof(ProjectileTargetComponent))]
	public class ProjectileSteerTowardTarget : MonoBehaviour
	{
		// Token: 0x06001F3E RID: 7998 RVA: 0x00016E1C File Offset: 0x0001501C
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

		// Token: 0x06001F3F RID: 7999 RVA: 0x000987F8 File Offset: 0x000969F8
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

		// Token: 0x040021A8 RID: 8616
		[Tooltip("Constrains rotation to the Y axis only.")]
		public bool yAxisOnly;

		// Token: 0x040021A9 RID: 8617
		[Tooltip("How fast to rotate in degrees per second. Rotation is linear.")]
		public float rotationSpeed;

		// Token: 0x040021AA RID: 8618
		private new Transform transform;

		// Token: 0x040021AB RID: 8619
		private ProjectileTargetComponent targetComponent;
	}
}
