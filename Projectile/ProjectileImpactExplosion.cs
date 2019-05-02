using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200055B RID: 1371
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileImpactExplosion : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001EBD RID: 7869 RVA: 0x00016769 File Offset: 0x00014969
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.lifetime += UnityEngine.Random.Range(0f, this.lifetimeRandomOffset);
		}

		// Token: 0x06001EBE RID: 7870 RVA: 0x000961BC File Offset: 0x000943BC
		public void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (this.timerAfterImpact && this.hasImpact)
			{
				this.stopwatchAfterImpact += Time.fixedDeltaTime;
			}
			if ((this.stopwatch >= this.lifetime && (!this.timerAfterImpact || !this.hasImpact)) || this.stopwatchAfterImpact > this.lifetimeAfterImpact || (this.projectileHealthComponent && !this.projectileHealthComponent.alive))
			{
				this.alive = false;
			}
			if (this.alive && !this.hasPlayedLifetimeExpiredSound && (this.stopwatch > this.lifetime - this.offsetForLifetimeExpiredSound || (this.timerAfterImpact && this.stopwatchAfterImpact > this.lifetimeAfterImpact - this.offsetForLifetimeExpiredSound)))
			{
				this.hasPlayedLifetimeExpiredSound = true;
				Util.PlaySound(this.lifetimeExpiredSoundString, base.gameObject);
			}
			if (!this.alive)
			{
				if (NetworkServer.active)
				{
					if (this.impactEffect)
					{
						EffectManager.instance.SpawnEffect(this.impactEffect, new EffectData
						{
							origin = base.transform.position,
							scale = this.blastRadius
						}, true);
					}
					if (this.projectileDamage)
					{
						new BlastAttack
						{
							position = base.transform.position,
							baseDamage = this.projectileDamage.damage * this.blastDamageCoefficient,
							baseForce = this.projectileDamage.force * this.blastDamageCoefficient,
							radius = this.blastRadius,
							attacker = (this.projectileController.owner ? this.projectileController.owner.gameObject : null),
							inflictor = base.gameObject,
							teamIndex = this.projectileController.teamFilter.teamIndex,
							crit = this.projectileDamage.crit,
							procChainMask = this.projectileController.procChainMask,
							procCoefficient = this.projectileController.procCoefficient * this.blastProcCoefficient,
							bonusForce = this.bonusBlastForce,
							falloffModel = this.falloffModel,
							damageColorIndex = this.projectileDamage.damageColorIndex,
							damageType = this.projectileDamage.damageType
						}.Fire();
					}
					if (this.explosionSoundString.Length > 0)
					{
						Util.PlaySound(this.explosionSoundString, base.gameObject);
					}
					if (this.fireChildren)
					{
						for (int i = 0; i < this.childrenCount; i++)
						{
							Vector3 vector = new Vector3(UnityEngine.Random.Range(this.minAngleOffset.x, this.maxAngleOffset.x), UnityEngine.Random.Range(this.minAngleOffset.z, this.maxAngleOffset.z), UnityEngine.Random.Range(this.minAngleOffset.z, this.maxAngleOffset.z));
							switch (this.transformSpace)
							{
							case ProjectileImpactExplosion.TransformSpace.World:
								this.FireChild(vector);
								break;
							case ProjectileImpactExplosion.TransformSpace.Local:
								this.FireChild(base.transform.forward + base.transform.TransformDirection(vector));
								break;
							case ProjectileImpactExplosion.TransformSpace.Normal:
								this.FireChild(this.impactNormal + vector);
								break;
							}
						}
					}
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06001EBF RID: 7871 RVA: 0x00096524 File Offset: 0x00094724
		private void FireChild(Vector3 direction)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.childrenProjectilePrefab, base.transform.position, Util.QuaternionSafeLookRotation(direction));
			ProjectileController component = gameObject.GetComponent<ProjectileController>();
			if (component)
			{
				component.procChainMask = this.projectileController.procChainMask;
				component.procCoefficient = this.projectileController.procCoefficient;
				component.Networkowner = this.projectileController.owner;
			}
			gameObject.GetComponent<TeamFilter>().teamIndex = base.GetComponent<TeamFilter>().teamIndex;
			ProjectileDamage component2 = gameObject.GetComponent<ProjectileDamage>();
			if (component2)
			{
				component2.damage = this.projectileDamage.damage * this.childrenDamageCoefficient;
				component2.crit = this.projectileDamage.crit;
				component2.force = this.projectileDamage.force;
				component2.damageColorIndex = this.projectileDamage.damageColorIndex;
			}
			NetworkServer.Spawn(gameObject);
		}

		// Token: 0x06001EC0 RID: 7872 RVA: 0x000167A0 File Offset: 0x000149A0
		public void SetExplosionRadius(float newRadius)
		{
			this.blastRadius = newRadius;
		}

		// Token: 0x06001EC1 RID: 7873 RVA: 0x00096604 File Offset: 0x00094804
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (!this.alive)
			{
				return;
			}
			Collider collider = impactInfo.collider;
			this.impactNormal = impactInfo.estimatedImpactNormal;
			if (collider)
			{
				DamageInfo damageInfo = new DamageInfo();
				if (this.projectileDamage)
				{
					damageInfo.damage = this.projectileDamage.damage;
					damageInfo.crit = this.projectileDamage.crit;
					damageInfo.attacker = (this.projectileController.owner ? this.projectileController.owner.gameObject : null);
					damageInfo.inflictor = base.gameObject;
					damageInfo.position = impactInfo.estimatedPointOfImpact;
					damageInfo.force = this.projectileDamage.force * base.transform.forward;
					damageInfo.procChainMask = this.projectileController.procChainMask;
					damageInfo.procCoefficient = this.projectileController.procCoefficient;
				}
				else
				{
					Debug.Log("No projectile damage component!");
				}
				HurtBox component = collider.GetComponent<HurtBox>();
				if (component)
				{
					if (this.destroyOnEnemy)
					{
						HealthComponent healthComponent = component.healthComponent;
						if (healthComponent)
						{
							if (healthComponent.gameObject == this.projectileController.owner)
							{
								return;
							}
							if (this.projectileHealthComponent && healthComponent == this.projectileHealthComponent)
							{
								return;
							}
							this.alive = false;
						}
					}
				}
				else if (this.destroyOnWorld)
				{
					this.alive = false;
				}
				this.hasImpact = true;
				if (NetworkServer.active)
				{
					GlobalEventManager.instance.OnHitAll(damageInfo, collider.gameObject);
				}
			}
		}

		// Token: 0x06001EC2 RID: 7874 RVA: 0x000167A9 File Offset: 0x000149A9
		public void SetAlive(bool newAlive)
		{
			this.alive = newAlive;
		}

		// Token: 0x040020FC RID: 8444
		private ProjectileController projectileController;

		// Token: 0x040020FD RID: 8445
		private ProjectileDamage projectileDamage;

		// Token: 0x040020FE RID: 8446
		private bool alive = true;

		// Token: 0x040020FF RID: 8447
		private Vector3 impactNormal = Vector3.up;

		// Token: 0x04002100 RID: 8448
		public GameObject impactEffect;

		// Token: 0x04002101 RID: 8449
		public string explosionSoundString;

		// Token: 0x04002102 RID: 8450
		public string lifetimeExpiredSoundString;

		// Token: 0x04002103 RID: 8451
		public float offsetForLifetimeExpiredSound;

		// Token: 0x04002104 RID: 8452
		public bool destroyOnEnemy = true;

		// Token: 0x04002105 RID: 8453
		public bool destroyOnWorld;

		// Token: 0x04002106 RID: 8454
		public bool timerAfterImpact;

		// Token: 0x04002107 RID: 8455
		public BlastAttack.FalloffModel falloffModel = BlastAttack.FalloffModel.Linear;

		// Token: 0x04002108 RID: 8456
		public float lifetime;

		// Token: 0x04002109 RID: 8457
		public float lifetimeAfterImpact;

		// Token: 0x0400210A RID: 8458
		public float lifetimeRandomOffset;

		// Token: 0x0400210B RID: 8459
		public float blastRadius;

		// Token: 0x0400210C RID: 8460
		[Tooltip("The percentage of the damage, proc coefficient, and force of the initial projectile. Ranges from 0-1")]
		public float blastDamageCoefficient;

		// Token: 0x0400210D RID: 8461
		public float blastProcCoefficient = 1f;

		// Token: 0x0400210E RID: 8462
		public Vector3 bonusBlastForce;

		// Token: 0x0400210F RID: 8463
		[Tooltip("Does this projectile release children on death?")]
		public bool fireChildren;

		// Token: 0x04002110 RID: 8464
		public GameObject childrenProjectilePrefab;

		// Token: 0x04002111 RID: 8465
		public int childrenCount;

		// Token: 0x04002112 RID: 8466
		[Tooltip("What percentage of our damage does the children get?")]
		public float childrenDamageCoefficient;

		// Token: 0x04002113 RID: 8467
		public Vector3 minAngleOffset;

		// Token: 0x04002114 RID: 8468
		public Vector3 maxAngleOffset;

		// Token: 0x04002115 RID: 8469
		public ProjectileImpactExplosion.TransformSpace transformSpace;

		// Token: 0x04002116 RID: 8470
		public HealthComponent projectileHealthComponent;

		// Token: 0x04002117 RID: 8471
		private float stopwatch;

		// Token: 0x04002118 RID: 8472
		private float stopwatchAfterImpact;

		// Token: 0x04002119 RID: 8473
		private bool hasImpact;

		// Token: 0x0400211A RID: 8474
		private bool hasPlayedLifetimeExpiredSound;

		// Token: 0x0200055C RID: 1372
		public enum TransformSpace
		{
			// Token: 0x0400211C RID: 8476
			World,
			// Token: 0x0400211D RID: 8477
			Local,
			// Token: 0x0400211E RID: 8478
			Normal
		}
	}
}
