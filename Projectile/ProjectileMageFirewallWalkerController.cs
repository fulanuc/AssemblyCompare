using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000551 RID: 1361
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileMageFirewallWalkerController : MonoBehaviour
	{
		// Token: 0x06001E65 RID: 7781 RVA: 0x00095D14 File Offset: 0x00093F14
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.lastCenterPosition = base.transform.position;
			this.timer = this.dropInterval / 2f;
			this.moveSign = 1f;
		}

		// Token: 0x06001E66 RID: 7782 RVA: 0x00095D68 File Offset: 0x00093F68
		private void Start()
		{
			if (this.projectileController.owner)
			{
				Vector3 position = this.projectileController.owner.transform.position;
				Vector3 vector = base.transform.position - position;
				vector.y = 0f;
				if (vector.x != 0f && vector.z != 0f)
				{
					this.moveSign = Mathf.Sign(Vector3.Dot(base.transform.right, vector));
				}
			}
			this.UpdateDirections();
		}

		// Token: 0x06001E67 RID: 7783 RVA: 0x00095DF8 File Offset: 0x00093FF8
		private void UpdateDirections()
		{
			if (!this.curveToCenter)
			{
				return;
			}
			Vector3 vector = base.transform.position - this.lastCenterPosition;
			vector.y = 0f;
			if (vector.x != 0f && vector.z != 0f)
			{
				vector.Normalize();
				Vector3 vector2 = Vector3.Cross(Vector3.up, vector);
				base.transform.forward = vector2 * this.moveSign;
				this.currentPillarVector = Quaternion.AngleAxis(this.pillarAngle, vector2) * Vector3.Cross(vector, vector2);
			}
		}

		// Token: 0x06001E68 RID: 7784 RVA: 0x00095E94 File Offset: 0x00094094
		private void FixedUpdate()
		{
			if (this.projectileController.owner)
			{
				this.lastCenterPosition = this.projectileController.owner.transform.position;
			}
			this.UpdateDirections();
			if (NetworkServer.active)
			{
				this.timer -= Time.fixedDeltaTime;
				if (this.timer <= 0f)
				{
					this.timer = this.dropInterval;
					if (this.firePillarPrefab)
					{
						ProjectileManager.instance.FireProjectile(this.firePillarPrefab, base.transform.position, Util.QuaternionSafeLookRotation(this.currentPillarVector), this.projectileController.owner, this.projectileDamage.damage, this.projectileDamage.force, this.projectileDamage.crit, this.projectileDamage.damageColorIndex, null, -1f);
					}
				}
			}
		}

		// Token: 0x040020EE RID: 8430
		public float dropInterval = 0.15f;

		// Token: 0x040020EF RID: 8431
		public GameObject firePillarPrefab;

		// Token: 0x040020F0 RID: 8432
		public float pillarAngle = 45f;

		// Token: 0x040020F1 RID: 8433
		public bool curveToCenter = true;

		// Token: 0x040020F2 RID: 8434
		private float moveSign;

		// Token: 0x040020F3 RID: 8435
		private ProjectileController projectileController;

		// Token: 0x040020F4 RID: 8436
		private ProjectileDamage projectileDamage;

		// Token: 0x040020F5 RID: 8437
		private Vector3 lastCenterPosition;

		// Token: 0x040020F6 RID: 8438
		private float timer;

		// Token: 0x040020F7 RID: 8439
		private Vector3 currentPillarVector = Vector3.up;
	}
}
