﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000280 RID: 640
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(ProjectileController))]
	public class ChainController : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06000C17 RID: 3095 RVA: 0x0004F040 File Offset: 0x0004D240
		private void Start()
		{
			if (!NetworkServer.active)
			{
				base.enabled = false;
				return;
			}
			this.transform = base.transform;
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.teamFilter = base.GetComponent<TeamFilter>();
			this.rigidbody.velocity = this.transform.forward * this.maxVelocity;
			this.pastTargetList = new List<Transform>();
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x0004F0C4 File Offset: 0x0004D2C4
		private void FixedUpdate()
		{
			if (!this.currentTarget)
			{
				this.stopwatch += Time.fixedDeltaTime;
				if (this.stopwatch > this.lifeTime)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				if (this.assignFirstTarget || (this.bounceCount > 0 && this.searchAfterFirstFailure))
				{
					this.currentTarget = this.FindTarget();
					if (!this.currentTarget && this.destroyOnFailure)
					{
						UnityEngine.Object.Destroy(base.gameObject);
					}
					else if (this.bounceCount == 0 && this.ignoreFirstTarget)
					{
						this.bounceCount++;
						this.pastTargetList.Add(this.currentTarget);
						this.currentTarget = null;
					}
				}
				this.rigidbody.velocity = this.transform.forward * this.maxVelocity;
				return;
			}
			this.stopwatch = 0f;
			Vector3 vector = this.currentTarget.transform.position - this.rigidbody.position;
			if (vector != Vector3.zero)
			{
				this.transform.rotation = Util.QuaternionSafeLookRotation(vector);
			}
			this.rigidbody.velocity = this.transform.forward * Mathf.Min(this.maxVelocity, Vector3.Magnitude(vector) / Time.fixedDeltaTime);
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x0004F228 File Offset: 0x0004D428
		private Transform FindTarget()
		{
			TeamIndex teamIndex = TeamIndex.Monster;
			TeamIndex teamIndex2 = this.teamFilter.teamIndex;
			if (teamIndex2 != TeamIndex.Player)
			{
				if (teamIndex2 == TeamIndex.Monster)
				{
					teamIndex = TeamIndex.Player;
				}
			}
			else
			{
				teamIndex = TeamIndex.Monster;
			}
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
			float num = 99999f;
			Transform result = null;
			Vector3 position = this.transform.position;
			for (int i = 0; i < teamMembers.Count; i++)
			{
				Transform transform = teamMembers[i].transform;
				if (!this.pastTargetList.Contains(transform) || (this.canBounceToSameTarget && transform != this.lastTarget))
				{
					float num2 = Vector3.SqrMagnitude(transform.position - position);
					if (num2 < num && (!this.smartSeeking || !Physics.Raycast(position, transform.position - position, Mathf.Sqrt(num2), LayerIndex.world.mask)) && num2 <= this.maxChainDistance * this.maxChainDistance)
					{
						num = num2;
						result = transform;
					}
				}
			}
			return result;
		}

		// Token: 0x06000C1A RID: 3098 RVA: 0x0004F32C File Offset: 0x0004D52C
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (!this.alive)
			{
				return;
			}
			Collider collider = impactInfo.collider;
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
					damageInfo.force = this.projectileDamage.force * this.transform.forward;
					damageInfo.procChainMask = this.projectileController.procChainMask;
					damageInfo.procCoefficient = this.projectileController.procCoefficient;
					damageInfo.damageColorIndex = this.projectileDamage.damageColorIndex;
				}
				HurtBox component = collider.GetComponent<HurtBox>();
				if (component)
				{
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent)
					{
						Transform transform = healthComponent.gameObject.transform;
						if (healthComponent.gameObject == this.projectileController.owner)
						{
							return;
						}
						if ((!this.pastTargetList.Contains(transform) || (this.canBounceToSameTarget && transform != this.lastTarget)) && (transform == this.currentTarget || this.canHitNonTarget || !this.currentTarget))
						{
							this.pastTargetList.Add(transform);
							if (this.currentTarget)
							{
								this.lastTarget = this.currentTarget;
							}
							else
							{
								this.lastTarget = transform;
							}
							this.currentTarget = this.FindTarget();
							this.bounceCount++;
							healthComponent.TakeDamage(damageInfo);
							GlobalEventManager.instance.OnHitAll(damageInfo, collider.gameObject);
							GlobalEventManager.instance.OnHitEnemy(damageInfo, component.healthComponent.gameObject);
							if (this.projectileDamage)
							{
								this.projectileDamage.damage *= this.damageMultiplier;
							}
							if (this.impactEffect)
							{
								EffectManager.instance.SimpleImpactEffect(this.impactEffect, this.transform.position, -this.transform.forward, !this.projectileController.isPrediction);
							}
							if (this.bounceCount >= this.maxBounceCount)
							{
								this.alive = false;
							}
						}
					}
				}
				else if (this.destroyOnWorldIfNoTarget && this.currentTarget == null)
				{
					damageInfo.position = this.transform.position;
					GlobalEventManager.instance.OnHitAll(damageInfo, collider.gameObject);
					if (this.impactEffect)
					{
						EffectManager.instance.SimpleImpactEffect(this.impactEffect, this.transform.position, -this.transform.forward, !this.projectileController.isPrediction);
					}
					this.alive = false;
				}
			}
			if (!this.alive)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x04001030 RID: 4144
		private new Transform transform;

		// Token: 0x04001031 RID: 4145
		private Rigidbody rigidbody;

		// Token: 0x04001032 RID: 4146
		[HideInInspector]
		public Transform currentTarget;

		// Token: 0x04001033 RID: 4147
		private Transform lastTarget;

		// Token: 0x04001034 RID: 4148
		private TeamFilter teamFilter;

		// Token: 0x04001035 RID: 4149
		private ProjectileController projectileController;

		// Token: 0x04001036 RID: 4150
		private ProjectileDamage projectileDamage;

		// Token: 0x04001037 RID: 4151
		private bool alive = true;

		// Token: 0x04001038 RID: 4152
		private int bounceCount;

		// Token: 0x04001039 RID: 4153
		[HideInInspector]
		public List<Transform> pastTargetList;

		// Token: 0x0400103A RID: 4154
		public GameObject impactEffect;

		// Token: 0x0400103B RID: 4155
		public float maxVelocity;

		// Token: 0x0400103C RID: 4156
		public int maxBounceCount = 3;

		// Token: 0x0400103D RID: 4157
		public float maxChainDistance = 10f;

		// Token: 0x0400103E RID: 4158
		public float lifeTime = 5f;

		// Token: 0x0400103F RID: 4159
		private float stopwatch;

		// Token: 0x04001040 RID: 4160
		[Tooltip("Multiplier for damage on every chain. >1 for increasing damage, <1 for decreasing damage.")]
		public float damageMultiplier = 1f;

		// Token: 0x04001041 RID: 4161
		[Tooltip("Whether or not the projectile will automatically detect its first target or fly dumb until the first hit.")]
		public bool assignFirstTarget;

		// Token: 0x04001042 RID: 4162
		[Tooltip("Whether or not the projectile will automatically ignore the first target, preventing it from getting hit.")]
		public bool ignoreFirstTarget;

		// Token: 0x04001043 RID: 4163
		[Tooltip("Whether or not the projectile will destroy itself on colliding with terrain.")]
		public bool destroyOnWorldIfNoTarget;

		// Token: 0x04001044 RID: 4164
		[Tooltip("Whether or not the projectile can bounce between the same targets.")]
		public bool canBounceToSameTarget;

		// Token: 0x04001045 RID: 4165
		[Tooltip("Whether or not the projectile can hit targets accidentally on the way.")]
		public bool canHitNonTarget;

		// Token: 0x04001046 RID: 4166
		[Tooltip("Whether or not the projectile will raycast to make sure it can hit its target. Can be expensive.")]
		public bool smartSeeking;

		// Token: 0x04001047 RID: 4167
		[Tooltip("Whether or not the projectile will continue to search for targets after its first failure. Can be expensive, especially with smart seeking.")]
		public bool searchAfterFirstFailure;

		// Token: 0x04001048 RID: 4168
		[Tooltip("Whether or not the projectile should destruct on failure.")]
		public bool destroyOnFailure;
	}
}
