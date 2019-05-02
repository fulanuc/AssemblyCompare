using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000559 RID: 1369
	[RequireComponent(typeof(HitBoxGroup))]
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileOverlapAttack : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001EA4 RID: 7844 RVA: 0x00096B18 File Offset: 0x00094D18
		private void Start()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.attack = new OverlapAttack();
			this.attack.procChainMask = this.projectileController.procChainMask;
			this.attack.procCoefficient = this.projectileController.procCoefficient * this.overlapProcCoefficient;
			this.attack.attacker = this.projectileController.owner;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = this.projectileController.teamFilter.teamIndex;
			this.attack.damage = this.damageCoefficient * this.projectileDamage.damage;
			this.attack.forceVector = this.forceVector + this.projectileDamage.force * base.transform.forward;
			this.attack.hitEffectPrefab = this.impactEffect;
			this.attack.isCrit = this.projectileDamage.crit;
			this.attack.damageColorIndex = this.projectileDamage.damageColorIndex;
			this.attack.damageType = this.projectileDamage.damageType;
			this.attack.procChainMask = this.projectileController.procChainMask;
			this.attack.hitBoxGroup = base.GetComponent<HitBoxGroup>();
		}

		// Token: 0x06001EA5 RID: 7845 RVA: 0x000025F6 File Offset: 0x000007F6
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
		}

		// Token: 0x06001EA6 RID: 7846 RVA: 0x00096C88 File Offset: 0x00094E88
		public void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				if (this.resetInterval >= 0f)
				{
					this.resetTimer -= Time.fixedDeltaTime;
					if (this.resetTimer <= 0f)
					{
						this.resetTimer = this.resetInterval;
						this.attack.ResetIgnoredHealthComponents();
					}
				}
				this.attack.Fire(null);
			}
		}

		// Token: 0x04002127 RID: 8487
		private ProjectileController projectileController;

		// Token: 0x04002128 RID: 8488
		private ProjectileDamage projectileDamage;

		// Token: 0x04002129 RID: 8489
		public float damageCoefficient;

		// Token: 0x0400212A RID: 8490
		public GameObject impactEffect;

		// Token: 0x0400212B RID: 8491
		public Vector3 forceVector;

		// Token: 0x0400212C RID: 8492
		public float overlapProcCoefficient = 1f;

		// Token: 0x0400212D RID: 8493
		private OverlapAttack attack;

		// Token: 0x0400212E RID: 8494
		[Tooltip("If non-negative, the attack clears its hit memory at the specified interval.")]
		public float resetInterval = -1f;

		// Token: 0x0400212F RID: 8495
		private float resetTimer;
	}
}
