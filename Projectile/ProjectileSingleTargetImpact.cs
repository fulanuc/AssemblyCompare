using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200056C RID: 1388
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileSingleTargetImpact : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001F27 RID: 7975 RVA: 0x00016D2A File Offset: 0x00014F2A
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001F28 RID: 7976 RVA: 0x000980A4 File Offset: 0x000962A4
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
					damageInfo.force = this.projectileDamage.force * base.transform.forward;
					damageInfo.procChainMask = this.projectileController.procChainMask;
					damageInfo.procCoefficient = this.projectileController.procCoefficient;
					damageInfo.damageColorIndex = this.projectileDamage.damageColorIndex;
					damageInfo.damageType = this.projectileDamage.damageType;
				}
				else
				{
					Debug.Log("No projectile damage component!");
				}
				HurtBox component = collider.GetComponent<HurtBox>();
				if (component)
				{
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent)
					{
						if (healthComponent.gameObject == this.projectileController.owner)
						{
							return;
						}
						TeamComponent component2 = healthComponent.GetComponent<TeamComponent>();
						TeamFilter component3 = base.GetComponent<TeamFilter>();
						bool flag = false;
						if (component2 && component3)
						{
							flag = (component2.teamIndex == component3.teamIndex);
						}
						if (NetworkServer.active && !flag)
						{
							damageInfo.ModifyDamageInfo(component.damageModifier);
							healthComponent.TakeDamage(damageInfo);
							GlobalEventManager.instance.OnHitEnemy(damageInfo, component.healthComponent.gameObject);
						}
						this.alive = false;
					}
				}
				else if (this.destroyOnWorld)
				{
					this.alive = false;
				}
				damageInfo.position = base.transform.position;
				if (NetworkServer.active)
				{
					GlobalEventManager.instance.OnHitAll(damageInfo, collider.gameObject);
				}
			}
			if (!this.alive)
			{
				if (NetworkServer.active)
				{
					if (this.impactEffect)
					{
						EffectManager.instance.SimpleImpactEffect(this.impactEffect, impactInfo.estimatedPointOfImpact, -base.transform.forward, !this.projectileController.isPrediction);
					}
					if (this.hitSoundString.Length > 0)
					{
						Util.PlaySound(this.hitSoundString, base.gameObject);
					}
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x04002193 RID: 8595
		private ProjectileController projectileController;

		// Token: 0x04002194 RID: 8596
		private ProjectileDamage projectileDamage;

		// Token: 0x04002195 RID: 8597
		private bool alive = true;

		// Token: 0x04002196 RID: 8598
		public bool destroyOnWorld;

		// Token: 0x04002197 RID: 8599
		public GameObject impactEffect;

		// Token: 0x04002198 RID: 8600
		public string hitSoundString;
	}
}
