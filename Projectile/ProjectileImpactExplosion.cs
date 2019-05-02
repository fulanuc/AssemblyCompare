using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200054C RID: 1356
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileImpactExplosion : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E53 RID: 7763 RVA: 0x0001628A File Offset: 0x0001448A
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.lifetime += UnityEngine.Random.Range(0f, this.lifetimeRandomOffset);
		}

		// Token: 0x06001E54 RID: 7764 RVA: 0x000954A0 File Offset: 0x000936A0
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

		// Token: 0x06001E55 RID: 7765 RVA: 0x00095808 File Offset: 0x00093A08
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

		// Token: 0x06001E56 RID: 7766 RVA: 0x000162C1 File Offset: 0x000144C1
		public void SetExplosionRadius(float newRadius)
		{
			this.blastRadius = newRadius;
		}

		// Token: 0x06001E57 RID: 7767 RVA: 0x000958E8 File Offset: 0x00093AE8
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

		// Token: 0x06001E58 RID: 7768 RVA: 0x000162CA File Offset: 0x000144CA
		public void SetAlive(bool newAlive)
		{
			this.alive = newAlive;
		}

		// Token: 0x040020BE RID: 8382
		private ProjectileController projectileController;

		// Token: 0x040020BF RID: 8383
		private ProjectileDamage projectileDamage;

		// Token: 0x040020C0 RID: 8384
		private bool alive = true;

		// Token: 0x040020C1 RID: 8385
		private Vector3 impactNormal = Vector3.up;

		// Token: 0x040020C2 RID: 8386
		public GameObject impactEffect;

		// Token: 0x040020C3 RID: 8387
		public string explosionSoundString;

		// Token: 0x040020C4 RID: 8388
		public string lifetimeExpiredSoundString;

		// Token: 0x040020C5 RID: 8389
		public float offsetForLifetimeExpiredSound;

		// Token: 0x040020C6 RID: 8390
		public bool destroyOnEnemy = true;

		// Token: 0x040020C7 RID: 8391
		public bool destroyOnWorld;

		// Token: 0x040020C8 RID: 8392
		public bool timerAfterImpact;

		// Token: 0x040020C9 RID: 8393
		public BlastAttack.FalloffModel falloffModel = BlastAttack.FalloffModel.Linear;

		// Token: 0x040020CA RID: 8394
		public float lifetime;

		// Token: 0x040020CB RID: 8395
		public float lifetimeAfterImpact;

		// Token: 0x040020CC RID: 8396
		public float lifetimeRandomOffset;

		// Token: 0x040020CD RID: 8397
		public float blastRadius;

		// Token: 0x040020CE RID: 8398
		[Tooltip("The percentage of the damage, proc coefficient, and force of the initial projectile. Ranges from 0-1")]
		public float blastDamageCoefficient;

		// Token: 0x040020CF RID: 8399
		public float blastProcCoefficient = 1f;

		// Token: 0x040020D0 RID: 8400
		public Vector3 bonusBlastForce;

		// Token: 0x040020D1 RID: 8401
		[Tooltip("Does this projectile release children on death?")]
		public bool fireChildren;

		// Token: 0x040020D2 RID: 8402
		public GameObject childrenProjectilePrefab;

		// Token: 0x040020D3 RID: 8403
		public int childrenCount;

		// Token: 0x040020D4 RID: 8404
		[Tooltip("What percentage of our damage does the children get?")]
		public float childrenDamageCoefficient;

		// Token: 0x040020D5 RID: 8405
		public Vector3 minAngleOffset;

		// Token: 0x040020D6 RID: 8406
		public Vector3 maxAngleOffset;

		// Token: 0x040020D7 RID: 8407
		public ProjectileImpactExplosion.TransformSpace transformSpace;

		// Token: 0x040020D8 RID: 8408
		public HealthComponent projectileHealthComponent;

		// Token: 0x040020D9 RID: 8409
		private float stopwatch;

		// Token: 0x040020DA RID: 8410
		private float stopwatchAfterImpact;

		// Token: 0x040020DB RID: 8411
		private bool hasImpact;

		// Token: 0x040020DC RID: 8412
		private bool hasPlayedLifetimeExpiredSound;

		// Token: 0x0200054D RID: 1357
		public enum TransformSpace
		{
			// Token: 0x040020DE RID: 8414
			World,
			// Token: 0x040020DF RID: 8415
			Local,
			// Token: 0x040020E0 RID: 8416
			Normal
		}
	}
}
