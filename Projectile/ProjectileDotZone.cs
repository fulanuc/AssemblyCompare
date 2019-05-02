using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000543 RID: 1347
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(HitBoxGroup))]
	public class ProjectileDotZone : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E31 RID: 7729 RVA: 0x0001609A File Offset: 0x0001429A
		private void Start()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.ResetOverlap();
			this.onBegin.Invoke();
		}

		// Token: 0x06001E32 RID: 7730 RVA: 0x00094C80 File Offset: 0x00092E80
		private void ResetOverlap()
		{
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
			this.attack.hitBoxGroup = base.GetComponent<HitBoxGroup>();
		}

		// Token: 0x06001E33 RID: 7731 RVA: 0x000025F6 File Offset: 0x000007F6
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
		}

		// Token: 0x06001E34 RID: 7732 RVA: 0x00094DC0 File Offset: 0x00092FC0
		public void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.totalStopwatch += Time.fixedDeltaTime;
				this.resetStopwatch += Time.fixedDeltaTime;
				this.fireStopwatch += Time.fixedDeltaTime;
				if (this.resetStopwatch >= 1f / this.resetFrequency)
				{
					this.ResetOverlap();
					this.resetStopwatch -= 1f / this.resetFrequency;
				}
				if (this.fireStopwatch >= 1f / this.fireFrequency)
				{
					this.attack.Fire(null);
					this.fireStopwatch -= 1f / this.fireFrequency;
				}
				if (this.totalStopwatch >= this.lifetime)
				{
					this.onEnd.Invoke();
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x0400208C RID: 8332
		private ProjectileController projectileController;

		// Token: 0x0400208D RID: 8333
		private ProjectileDamage projectileDamage;

		// Token: 0x0400208E RID: 8334
		public float damageCoefficient;

		// Token: 0x0400208F RID: 8335
		public GameObject impactEffect;

		// Token: 0x04002090 RID: 8336
		public Vector3 forceVector;

		// Token: 0x04002091 RID: 8337
		public float overlapProcCoefficient = 1f;

		// Token: 0x04002092 RID: 8338
		[Tooltip("The frequency (1/time) at which the overlap attack is tested. Higher values are more accurate but more expensive.")]
		public float fireFrequency = 1f;

		// Token: 0x04002093 RID: 8339
		[Tooltip("The frequency  (1/time) at which the overlap attack is reset. Higher values means more frequent ticks of damage.")]
		public float resetFrequency = 20f;

		// Token: 0x04002094 RID: 8340
		public float lifetime = 30f;

		// Token: 0x04002095 RID: 8341
		[Tooltip("The event that runs at the start.")]
		public UnityEvent onBegin;

		// Token: 0x04002096 RID: 8342
		[Tooltip("The event that runs at the start.")]
		public UnityEvent onEnd;

		// Token: 0x04002097 RID: 8343
		private OverlapAttack attack;

		// Token: 0x04002098 RID: 8344
		private float fireStopwatch;

		// Token: 0x04002099 RID: 8345
		private float resetStopwatch;

		// Token: 0x0400209A RID: 8346
		private float totalStopwatch;
	}
}
