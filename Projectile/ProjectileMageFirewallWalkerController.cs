﻿using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000560 RID: 1376
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileMageFirewallWalkerController : MonoBehaviour
	{
		// Token: 0x06001ECF RID: 7887 RVA: 0x00096A30 File Offset: 0x00094C30
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.lastCenterPosition = base.transform.position;
			this.timer = this.dropInterval / 2f;
			this.moveSign = 1f;
		}

		// Token: 0x06001ED0 RID: 7888 RVA: 0x00096A84 File Offset: 0x00094C84
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

		// Token: 0x06001ED1 RID: 7889 RVA: 0x00096B14 File Offset: 0x00094D14
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

		// Token: 0x06001ED2 RID: 7890 RVA: 0x00096BB0 File Offset: 0x00094DB0
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

		// Token: 0x0400212C RID: 8492
		public float dropInterval = 0.15f;

		// Token: 0x0400212D RID: 8493
		public GameObject firePillarPrefab;

		// Token: 0x0400212E RID: 8494
		public float pillarAngle = 45f;

		// Token: 0x0400212F RID: 8495
		public bool curveToCenter = true;

		// Token: 0x04002130 RID: 8496
		private float moveSign;

		// Token: 0x04002131 RID: 8497
		private ProjectileController projectileController;

		// Token: 0x04002132 RID: 8498
		private ProjectileDamage projectileDamage;

		// Token: 0x04002133 RID: 8499
		private Vector3 lastCenterPosition;

		// Token: 0x04002134 RID: 8500
		private float timer;

		// Token: 0x04002135 RID: 8501
		private Vector3 currentPillarVector = Vector3.up;
	}
}
