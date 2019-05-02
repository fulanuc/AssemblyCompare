using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000533 RID: 1331
	[RequireComponent(typeof(ProjectileController))]
	public class BoomerangProjectile : NetworkBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001DE5 RID: 7653 RVA: 0x000933C0 File Offset: 0x000915C0
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

		// Token: 0x06001DE6 RID: 7654 RVA: 0x00093428 File Offset: 0x00091628
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

		// Token: 0x06001DE7 RID: 7655 RVA: 0x000935F0 File Offset: 0x000917F0
		private bool Reel()
		{
			Vector3 vector = this.projectileController.owner.transform.position - base.transform.position;
			Vector3 normalized = vector.normalized;
			return vector.magnitude <= 2f;
		}

		// Token: 0x06001DE8 RID: 7656 RVA: 0x0009363C File Offset: 0x0009183C
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

		// Token: 0x06001DE9 RID: 7657 RVA: 0x00093790 File Offset: 0x00091990
		private Vector3 CalculatePullDirection()
		{
			if (this.projectileController.owner)
			{
				return (this.projectileController.owner.transform.position - base.transform.position).normalized;
			}
			return base.transform.forward;
		}

		// Token: 0x06001DEB RID: 7659 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06001DEC RID: 7660 RVA: 0x000937E8 File Offset: 0x000919E8
		// (set) Token: 0x06001DED RID: 7661 RVA: 0x00015D93 File Offset: 0x00013F93
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

		// Token: 0x06001DEE RID: 7662 RVA: 0x000937FC File Offset: 0x000919FC
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

		// Token: 0x06001DEF RID: 7663 RVA: 0x00093868 File Offset: 0x00091A68
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

		// Token: 0x04002015 RID: 8213
		public float travelSpeed = 40f;

		// Token: 0x04002016 RID: 8214
		public float transitionDuration;

		// Token: 0x04002017 RID: 8215
		public float maxFlyStopwatch;

		// Token: 0x04002018 RID: 8216
		public GameObject impactSpark;

		// Token: 0x04002019 RID: 8217
		public float damageCoefficient;

		// Token: 0x0400201A RID: 8218
		public bool canHitCharacters;

		// Token: 0x0400201B RID: 8219
		public bool canHitWorld;

		// Token: 0x0400201C RID: 8220
		private ProjectileController projectileController;

		// Token: 0x0400201D RID: 8221
		[SyncVar]
		private BoomerangProjectile.BoomerangState boomerangState;

		// Token: 0x0400201E RID: 8222
		private Transform ownerTransform;

		// Token: 0x0400201F RID: 8223
		private ProjectileDamage projectileDamage;

		// Token: 0x04002020 RID: 8224
		private Rigidbody rigidbody;

		// Token: 0x04002021 RID: 8225
		private float stopwatch;

		// Token: 0x02000534 RID: 1332
		private enum BoomerangState
		{
			// Token: 0x04002023 RID: 8227
			FlyOut,
			// Token: 0x04002024 RID: 8228
			Transition,
			// Token: 0x04002025 RID: 8229
			FlyBack
		}
	}
}
