using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000542 RID: 1346
	[RequireComponent(typeof(ProjectileController))]
	public class BoomerangProjectile : NetworkBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E4F RID: 7759 RVA: 0x000940DC File Offset: 0x000922DC
		private void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			if (this.projectileController && this.projectileController.owner)
			{
				this.ownerTransform = this.projectileController.owner.transform;
			}
		}

		// Token: 0x06001E50 RID: 7760 RVA: 0x00094144 File Offset: 0x00092344
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			HurtBox component = impactInfo.collider.GetComponent<HurtBox>();
			if (component)
			{
				HealthComponent healthComponent = component.healthComponent;
				if (healthComponent)
				{
					if (!this.canHitCharacters)
					{
						return;
					}
					TeamComponent component2 = healthComponent.GetComponent<TeamComponent>();
					TeamFilter component3 = base.GetComponent<TeamFilter>();
					if (healthComponent.gameObject == this.projectileController.owner || component2.teamIndex == component3.teamIndex)
					{
						return;
					}
					GameObject gameObject = healthComponent.gameObject;
					DamageInfo damageInfo = new DamageInfo();
					if (this.projectileDamage)
					{
						damageInfo.damage = this.projectileDamage.damage * this.damageCoefficient;
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
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					this.NetworkboomerangState = BoomerangProjectile.BoomerangState.FlyBack;
				}
			}
			if (!this.canHitWorld)
			{
				return;
			}
			this.NetworkboomerangState = BoomerangProjectile.BoomerangState.FlyBack;
			EffectManager.instance.SimpleImpactEffect(this.impactSpark, impactInfo.estimatedPointOfImpact, -base.transform.forward, true);
		}

		// Token: 0x06001E51 RID: 7761 RVA: 0x0009430C File Offset: 0x0009250C
		private bool Reel()
		{
			Vector3 vector = this.projectileController.owner.transform.position - base.transform.position;
			Vector3 normalized = vector.normalized;
			return vector.magnitude <= 2f;
		}

		// Token: 0x06001E52 RID: 7762 RVA: 0x00094358 File Offset: 0x00092558
		public void FixedUpdate()
		{
			if (NetworkServer.active && !this.projectileController.owner)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			switch (this.boomerangState)
			{
			case BoomerangProjectile.BoomerangState.FlyOut:
				if (NetworkServer.active)
				{
					this.rigidbody.velocity = this.travelSpeed * base.transform.forward;
					this.stopwatch += Time.fixedDeltaTime;
					if (this.stopwatch >= this.maxFlyStopwatch)
					{
						this.stopwatch = 0f;
						this.NetworkboomerangState = BoomerangProjectile.BoomerangState.Transition;
						return;
					}
				}
				break;
			case BoomerangProjectile.BoomerangState.Transition:
			{
				this.stopwatch += Time.fixedDeltaTime;
				float num = this.stopwatch / this.transitionDuration;
				Vector3 a = this.CalculatePullDirection();
				this.rigidbody.velocity = Vector3.Lerp(this.travelSpeed * base.transform.forward, this.travelSpeed * a, num);
				if (num >= 1f)
				{
					this.NetworkboomerangState = BoomerangProjectile.BoomerangState.FlyBack;
					return;
				}
				break;
			}
			case BoomerangProjectile.BoomerangState.FlyBack:
			{
				bool flag = this.Reel();
				if (NetworkServer.active)
				{
					Vector3 a2 = this.CalculatePullDirection();
					this.rigidbody.velocity = this.travelSpeed * a2;
					if (flag)
					{
						UnityEngine.Object.Destroy(base.gameObject);
					}
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x06001E53 RID: 7763 RVA: 0x000944AC File Offset: 0x000926AC
		private Vector3 CalculatePullDirection()
		{
			if (this.projectileController.owner)
			{
				return (this.projectileController.owner.transform.position - base.transform.position).normalized;
			}
			return base.transform.forward;
		}

		// Token: 0x06001E55 RID: 7765 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06001E56 RID: 7766 RVA: 0x00094504 File Offset: 0x00092704
		// (set) Token: 0x06001E57 RID: 7767 RVA: 0x00016272 File Offset: 0x00014472
		public BoomerangProjectile.BoomerangState NetworkboomerangState
		{
			get
			{
				return this.boomerangState;
			}
			set
			{
				base.SetSyncVar<BoomerangProjectile.BoomerangState>(value, ref this.boomerangState, 1u);
			}
		}

		// Token: 0x06001E58 RID: 7768 RVA: 0x00094518 File Offset: 0x00092718
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write((int)this.boomerangState);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write((int)this.boomerangState);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001E59 RID: 7769 RVA: 0x00094584 File Offset: 0x00092784
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.boomerangState = (BoomerangProjectile.BoomerangState)reader.ReadInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.boomerangState = (BoomerangProjectile.BoomerangState)reader.ReadInt32();
			}
		}

		// Token: 0x04002053 RID: 8275
		public float travelSpeed = 40f;

		// Token: 0x04002054 RID: 8276
		public float transitionDuration;

		// Token: 0x04002055 RID: 8277
		public float maxFlyStopwatch;

		// Token: 0x04002056 RID: 8278
		public GameObject impactSpark;

		// Token: 0x04002057 RID: 8279
		public float damageCoefficient;

		// Token: 0x04002058 RID: 8280
		public bool canHitCharacters;

		// Token: 0x04002059 RID: 8281
		public bool canHitWorld;

		// Token: 0x0400205A RID: 8282
		private ProjectileController projectileController;

		// Token: 0x0400205B RID: 8283
		[SyncVar]
		private BoomerangProjectile.BoomerangState boomerangState;

		// Token: 0x0400205C RID: 8284
		private Transform ownerTransform;

		// Token: 0x0400205D RID: 8285
		private ProjectileDamage projectileDamage;

		// Token: 0x0400205E RID: 8286
		private Rigidbody rigidbody;

		// Token: 0x0400205F RID: 8287
		private float stopwatch;

		// Token: 0x02000543 RID: 1347
		private enum BoomerangState
		{
			// Token: 0x04002061 RID: 8289
			FlyOut,
			// Token: 0x04002062 RID: 8290
			Transition,
			// Token: 0x04002063 RID: 8291
			FlyBack
		}
	}
}
