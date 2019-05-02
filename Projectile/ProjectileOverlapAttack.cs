using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000568 RID: 1384
	[RequireComponent(typeof(ProjectileDamage))]
	[RequireComponent(typeof(HitBoxGroup))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileOverlapAttack : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001F0E RID: 7950 RVA: 0x00097834 File Offset: 0x00095A34
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

		// Token: 0x06001F0F RID: 7951 RVA: 0x000025DA File Offset: 0x000007DA
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
		}

		// Token: 0x06001F10 RID: 7952 RVA: 0x000979A4 File Offset: 0x00095BA4
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

		// Token: 0x04002165 RID: 8549
		private ProjectileController projectileController;

		// Token: 0x04002166 RID: 8550
		private ProjectileDamage projectileDamage;

		// Token: 0x04002167 RID: 8551
		public float damageCoefficient;

		// Token: 0x04002168 RID: 8552
		public GameObject impactEffect;

		// Token: 0x04002169 RID: 8553
		public Vector3 forceVector;

		// Token: 0x0400216A RID: 8554
		public float overlapProcCoefficient = 1f;

		// Token: 0x0400216B RID: 8555
		private OverlapAttack attack;

		// Token: 0x0400216C RID: 8556
		[Tooltip("If non-negative, the attack clears its hit memory at the specified interval.")]
		public float resetInterval = -1f;

		// Token: 0x0400216D RID: 8557
		private float resetTimer;
	}
}
