using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000569 RID: 1385
	[RequireComponent(typeof(ProjectileDamage))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileParentTether : MonoBehaviour
	{
		// Token: 0x06001F12 RID: 7954 RVA: 0x00016C1C File Offset: 0x00014E1C
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.attackTimer = 0f;
			this.UpdateTetherGraphic();
		}

		// Token: 0x06001F13 RID: 7955 RVA: 0x00097A08 File Offset: 0x00095C08
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

		// Token: 0x06001F14 RID: 7956 RVA: 0x00097AF8 File Offset: 0x00095CF8
		private float GetRayDistance()
		{
			if (this.projectileController.owner)
			{
				return (this.projectileController.owner.transform.position - base.transform.position).magnitude;
			}
			return 0f;
		}

		// Token: 0x06001F15 RID: 7957 RVA: 0x00097B4C File Offset: 0x00095D4C
		private Ray GetAimRay()
		{
			Ray result = default(Ray);
			result.origin = base.transform.position;
			result.direction = this.projectileController.owner.transform.position - result.origin;
			return result;
		}

		// Token: 0x06001F16 RID: 7958 RVA: 0x00016C47 File Offset: 0x00014E47
		private bool ShouldIFire()
		{
			return !this.stickOnImpact || this.stickOnImpact.stuck;
		}

		// Token: 0x06001F17 RID: 7959 RVA: 0x00016C63 File Offset: 0x00014E63
		private void Update()
		{
			this.UpdateTetherGraphic();
		}

		// Token: 0x06001F18 RID: 7960 RVA: 0x00097B9C File Offset: 0x00095D9C
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

		// Token: 0x0400216E RID: 8558
		private ProjectileController projectileController;

		// Token: 0x0400216F RID: 8559
		private ProjectileDamage projectileDamage;

		// Token: 0x04002170 RID: 8560
		private TeamIndex myTeamIndex;

		// Token: 0x04002171 RID: 8561
		public float attackInterval = 1f;

		// Token: 0x04002172 RID: 8562
		public float maxTetherRange = 20f;

		// Token: 0x04002173 RID: 8563
		public float procCoefficient = 0.1f;

		// Token: 0x04002174 RID: 8564
		public float damageCoefficient = 1f;

		// Token: 0x04002175 RID: 8565
		public float raycastRadius;

		// Token: 0x04002176 RID: 8566
		public float lifetime;

		// Token: 0x04002177 RID: 8567
		public GameObject impactEffect;

		// Token: 0x04002178 RID: 8568
		public GameObject tetherEffectPrefab;

		// Token: 0x04002179 RID: 8569
		public ProjectileStickOnImpact stickOnImpact;

		// Token: 0x0400217A RID: 8570
		private GameObject tetherEffectInstance;

		// Token: 0x0400217B RID: 8571
		private GameObject tetherEffectInstanceEnd;

		// Token: 0x0400217C RID: 8572
		private float attackTimer;

		// Token: 0x0400217D RID: 8573
		private float lifetimeStopwatch;
	}
}
