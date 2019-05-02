using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000552 RID: 1362
	[RequireComponent(typeof(HitBoxGroup))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileDotZone : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E9B RID: 7835 RVA: 0x00016579 File Offset: 0x00014779
		private void Start()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.ResetOverlap();
			this.onBegin.Invoke();
		}

		// Token: 0x06001E9C RID: 7836 RVA: 0x0009599C File Offset: 0x00093B9C
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

		// Token: 0x06001E9D RID: 7837 RVA: 0x000025DA File Offset: 0x000007DA
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
		}

		// Token: 0x06001E9E RID: 7838 RVA: 0x00095ADC File Offset: 0x00093CDC
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

		// Token: 0x040020CA RID: 8394
		private ProjectileController projectileController;

		// Token: 0x040020CB RID: 8395
		private ProjectileDamage projectileDamage;

		// Token: 0x040020CC RID: 8396
		public float damageCoefficient;

		// Token: 0x040020CD RID: 8397
		public GameObject impactEffect;

		// Token: 0x040020CE RID: 8398
		public Vector3 forceVector;

		// Token: 0x040020CF RID: 8399
		public float overlapProcCoefficient = 1f;

		// Token: 0x040020D0 RID: 8400
		[Tooltip("The frequency (1/time) at which the overlap attack is tested. Higher values are more accurate but more expensive.")]
		public float fireFrequency = 1f;

		// Token: 0x040020D1 RID: 8401
		[Tooltip("The frequency  (1/time) at which the overlap attack is reset. Higher values means more frequent ticks of damage.")]
		public float resetFrequency = 20f;

		// Token: 0x040020D2 RID: 8402
		public float lifetime = 30f;

		// Token: 0x040020D3 RID: 8403
		[Tooltip("The event that runs at the start.")]
		public UnityEvent onBegin;

		// Token: 0x040020D4 RID: 8404
		[Tooltip("The event that runs at the start.")]
		public UnityEvent onEnd;

		// Token: 0x040020D5 RID: 8405
		private OverlapAttack attack;

		// Token: 0x040020D6 RID: 8406
		private float fireStopwatch;

		// Token: 0x040020D7 RID: 8407
		private float resetStopwatch;

		// Token: 0x040020D8 RID: 8408
		private float totalStopwatch;
	}
}
