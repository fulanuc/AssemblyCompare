using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200055A RID: 1370
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileParentTether : MonoBehaviour
	{
		// Token: 0x06001EA8 RID: 7848 RVA: 0x0001673D File Offset: 0x0001493D
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.attackTimer = 0f;
			this.UpdateTetherGraphic();
		}

		// Token: 0x06001EA9 RID: 7849 RVA: 0x00096CEC File Offset: 0x00094EEC
		private void UpdateTetherGraphic()
		{
			if (this.ShouldIFire())
			{
				if (this.tetherEffectPrefab && !this.tetherEffectInstance)
				{
					this.tetherEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.tetherEffectPrefab, base.transform.position, base.transform.rotation);
					this.tetherEffectInstance.transform.parent = base.transform;
					ChildLocator component = this.tetherEffectInstance.GetComponent<ChildLocator>();
					this.tetherEffectInstanceEnd = component.FindChild("LaserEnd").gameObject;
				}
				if (this.tetherEffectInstance)
				{
					Ray aimRay = this.GetAimRay();
					this.tetherEffectInstance.transform.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
					this.tetherEffectInstanceEnd.transform.position = aimRay.origin + aimRay.direction * this.GetRayDistance();
				}
			}
		}

		// Token: 0x06001EAA RID: 7850 RVA: 0x00096DDC File Offset: 0x00094FDC
		private float GetRayDistance()
		{
			if (this.projectileController.owner)
			{
				return (this.projectileController.owner.transform.position - base.transform.position).magnitude;
			}
			return 0f;
		}

		// Token: 0x06001EAB RID: 7851 RVA: 0x00096E30 File Offset: 0x00095030
		private Ray GetAimRay()
		{
			Ray result = default(Ray);
			result.origin = base.transform.position;
			result.direction = this.projectileController.owner.transform.position - result.origin;
			return result;
		}

		// Token: 0x06001EAC RID: 7852 RVA: 0x00016768 File Offset: 0x00014968
		private bool ShouldIFire()
		{
			return !this.stickOnImpact || this.stickOnImpact.stuck;
		}

		// Token: 0x06001EAD RID: 7853 RVA: 0x00016784 File Offset: 0x00014984
		private void Update()
		{
			this.UpdateTetherGraphic();
		}

		// Token: 0x06001EAE RID: 7854 RVA: 0x00096E80 File Offset: 0x00095080
		private void FixedUpdate()
		{
			if (this.ShouldIFire())
			{
				this.lifetimeStopwatch += Time.fixedDeltaTime;
			}
			if (this.lifetimeStopwatch > this.lifetime)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (this.projectileController.owner.transform && this.ShouldIFire())
			{
				this.myTeamIndex = (this.projectileController.teamFilter ? this.projectileController.teamFilter.teamIndex : TeamIndex.Neutral);
				this.attackTimer -= Time.fixedDeltaTime;
				if (this.attackTimer <= 0f)
				{
					Ray aimRay = this.GetAimRay();
					this.attackTimer = this.attackInterval;
					if (aimRay.direction.magnitude < this.maxTetherRange && NetworkServer.active)
					{
						new BulletAttack
						{
							owner = this.projectileController.owner,
							origin = aimRay.origin,
							aimVector = aimRay.direction,
							minSpread = 0f,
							damage = this.damageCoefficient * this.projectileDamage.damage,
							force = 0f,
							hitEffectPrefab = this.impactEffect,
							isCrit = this.projectileDamage.crit,
							radius = this.raycastRadius,
							falloffModel = BulletAttack.FalloffModel.None,
							stopperMask = 0,
							hitMask = LayerIndex.entityPrecise.mask,
							procCoefficient = this.procCoefficient,
							maxDistance = this.GetRayDistance()
						}.Fire();
					}
				}
			}
		}

		// Token: 0x04002130 RID: 8496
		private ProjectileController projectileController;

		// Token: 0x04002131 RID: 8497
		private ProjectileDamage projectileDamage;

		// Token: 0x04002132 RID: 8498
		private TeamIndex myTeamIndex;

		// Token: 0x04002133 RID: 8499
		public float attackInterval = 1f;

		// Token: 0x04002134 RID: 8500
		public float maxTetherRange = 20f;

		// Token: 0x04002135 RID: 8501
		public float procCoefficient = 0.1f;

		// Token: 0x04002136 RID: 8502
		public float damageCoefficient = 1f;

		// Token: 0x04002137 RID: 8503
		public float raycastRadius;

		// Token: 0x04002138 RID: 8504
		public float lifetime;

		// Token: 0x04002139 RID: 8505
		public GameObject impactEffect;

		// Token: 0x0400213A RID: 8506
		public GameObject tetherEffectPrefab;

		// Token: 0x0400213B RID: 8507
		public ProjectileStickOnImpact stickOnImpact;

		// Token: 0x0400213C RID: 8508
		private GameObject tetherEffectInstance;

		// Token: 0x0400213D RID: 8509
		private GameObject tetherEffectInstanceEnd;

		// Token: 0x0400213E RID: 8510
		private float attackTimer;

		// Token: 0x0400213F RID: 8511
		private float lifetimeStopwatch;
	}
}
