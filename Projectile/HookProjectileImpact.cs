using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000546 RID: 1350
	[RequireComponent(typeof(ProjectileController))]
	public class HookProjectileImpact : NetworkBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E62 RID: 7778 RVA: 0x000947F0 File Offset: 0x000929F0
		private void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			if (this.projectileController && this.projectileController.owner)
			{
				this.ownerTransform = this.projectileController.owner.transform;
			}
			this.liveTimer = this.maxDistance / this.reelSpeed;
		}

		// Token: 0x06001E63 RID: 7779 RVA: 0x0009486C File Offset: 0x00092A6C
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			EffectManager.instance.SimpleImpactEffect(this.impactSpark, impactInfo.estimatedPointOfImpact, -base.transform.forward, true);
			if (this.hookState != HookProjectileImpact.HookState.Flying)
			{
				return;
			}
			HurtBox component = impactInfo.collider.GetComponent<HurtBox>();
			if (component)
			{
				HealthComponent healthComponent = component.healthComponent;
				if (healthComponent)
				{
					TeamComponent component2 = healthComponent.GetComponent<TeamComponent>();
					TeamFilter component3 = base.GetComponent<TeamFilter>();
					if (healthComponent.gameObject == this.projectileController.owner || component2.teamIndex == component3.teamIndex)
					{
						return;
					}
					this.Networkvictim = healthComponent.gameObject;
					this.victimSetStateOnHurt = this.victim.GetComponent<SetStateOnHurt>();
					if (this.victimSetStateOnHurt)
					{
						this.victimSetStateOnHurt.SetPain();
					}
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
					}
					else
					{
						Debug.Log("No projectile damage component!");
					}
					Debug.Log(damageInfo.damage);
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					this.NetworkhookState = HookProjectileImpact.HookState.HitDelay;
					EffectManager.instance.SimpleImpactEffect(this.impactSuccess, impactInfo.estimatedPointOfImpact, -base.transform.forward, true);
				}
			}
			if (!this.victim)
			{
				this.NetworkhookState = HookProjectileImpact.HookState.ReelFail;
			}
		}

		// Token: 0x06001E64 RID: 7780 RVA: 0x00094A90 File Offset: 0x00092C90
		private bool Reel()
		{
			Vector3 vector = this.projectileController.owner.transform.position - this.victim.transform.position;
			Vector3 normalized = vector.normalized;
			float num = vector.magnitude;
			Collider component = this.projectileController.owner.GetComponent<Collider>();
			Collider component2 = this.victim.GetComponent<Collider>();
			if (component && component2)
			{
				num = Util.EstimateSurfaceDistance(component, component2);
			}
			bool flag = num <= 2f;
			Rigidbody rigidbody = null;
			float num2 = -1f;
			CharacterMotor component3 = this.projectileController.owner.GetComponent<CharacterMotor>();
			if (component3)
			{
				num2 = component3.mass;
			}
			else
			{
				rigidbody = this.projectileController.owner.GetComponent<Rigidbody>();
				if (rigidbody)
				{
					num2 = rigidbody.mass;
				}
			}
			Rigidbody rigidbody2 = null;
			float num3 = -1f;
			CharacterMotor component4 = this.victim.GetComponent<CharacterMotor>();
			if (component4)
			{
				num3 = component4.mass;
			}
			else
			{
				rigidbody2 = this.victim.GetComponent<Rigidbody>();
				if (rigidbody2)
				{
					num3 = rigidbody2.mass;
				}
			}
			float num4 = 0f;
			float num5 = 0f;
			if (num2 > 0f && num3 > 0f)
			{
				num4 = 1f - num2 / (num2 + num3);
				num5 = 1f - num4;
			}
			else if (num2 > 0f)
			{
				num4 = 1f;
			}
			else if (num3 > 0f)
			{
				num5 = 1f;
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				num4 = 0f;
				num5 = 0f;
			}
			Vector3 velocity = normalized * (num4 * this.ownerPullFactor * -this.reelSpeed);
			Vector3 velocity2 = normalized * (num5 * this.victimPullFactor * this.reelSpeed);
			if (component3)
			{
				component3.velocity = velocity;
			}
			if (rigidbody)
			{
				rigidbody.velocity = velocity;
			}
			if (component4)
			{
				component4.velocity = velocity2;
			}
			if (rigidbody2)
			{
				rigidbody2.velocity = velocity2;
			}
			CharacterDirection component5 = this.projectileController.owner.GetComponent<CharacterDirection>();
			CharacterDirection component6 = this.victim.GetComponent<CharacterDirection>();
			if (component5)
			{
				component5.forward = -normalized;
			}
			if (component6)
			{
				component6.forward = normalized;
			}
			return flag;
		}

		// Token: 0x06001E65 RID: 7781 RVA: 0x00094CF8 File Offset: 0x00092EF8
		public void FixedUpdate()
		{
			if (NetworkServer.active && !this.projectileController.owner)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (this.victim)
			{
				base.transform.position = this.victim.transform.position;
			}
			switch (this.hookState)
			{
			case HookProjectileImpact.HookState.Flying:
				if (NetworkServer.active)
				{
					this.flyTimer += Time.fixedDeltaTime;
					if (this.flyTimer >= this.liveTimer)
					{
						this.NetworkhookState = HookProjectileImpact.HookState.ReelFail;
						return;
					}
				}
				break;
			case HookProjectileImpact.HookState.HitDelay:
				if (NetworkServer.active)
				{
					if (!this.victim)
					{
						this.NetworkhookState = HookProjectileImpact.HookState.Reel;
						return;
					}
					this.delayTimer += Time.fixedDeltaTime;
					if (this.delayTimer >= this.reelDelayTime)
					{
						this.NetworkhookState = HookProjectileImpact.HookState.Reel;
						return;
					}
				}
				break;
			case HookProjectileImpact.HookState.Reel:
			{
				bool flag = true;
				if (this.victim)
				{
					flag = this.Reel();
				}
				if (NetworkServer.active)
				{
					if (!this.victim)
					{
						this.NetworkhookState = HookProjectileImpact.HookState.ReelFail;
					}
					if (flag)
					{
						if (this.victimSetStateOnHurt)
						{
							this.victimSetStateOnHurt.SetPain();
						}
						UnityEngine.Object.Destroy(base.gameObject);
						return;
					}
				}
				break;
			}
			case HookProjectileImpact.HookState.ReelFail:
				if (NetworkServer.active)
				{
					if (this.rigidbody)
					{
						this.rigidbody.isKinematic = true;
					}
					this.ownerTransform = this.projectileController.owner.transform;
					if (this.ownerTransform)
					{
						base.transform.position = Vector3.MoveTowards(base.transform.position, this.ownerTransform.position, this.reelSpeed * Time.fixedDeltaTime);
						if (base.transform.position == this.ownerTransform.position)
						{
							UnityEngine.Object.Destroy(base.gameObject);
						}
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06001E67 RID: 7783 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06001E68 RID: 7784 RVA: 0x00094EF0 File Offset: 0x000930F0
		// (set) Token: 0x06001E69 RID: 7785 RVA: 0x0001631B File Offset: 0x0001451B
		public HookProjectileImpact.HookState NetworkhookState
		{
			get
			{
				return this.hookState;
			}
			set
			{
				base.SetSyncVar<HookProjectileImpact.HookState>(value, ref this.hookState, 1u);
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06001E6A RID: 7786 RVA: 0x00094F04 File Offset: 0x00093104
		// (set) Token: 0x06001E6B RID: 7787 RVA: 0x0001632F File Offset: 0x0001452F
		public GameObject Networkvictim
		{
			get
			{
				return this.victim;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.victim, 2u, ref this.___victimNetId);
			}
		}

		// Token: 0x06001E6C RID: 7788 RVA: 0x00094F18 File Offset: 0x00093118
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write((int)this.hookState);
				writer.Write(this.victim);
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
				writer.Write((int)this.hookState);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.victim);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001E6D RID: 7789 RVA: 0x00094FC4 File Offset: 0x000931C4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.hookState = (HookProjectileImpact.HookState)reader.ReadInt32();
				this.___victimNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.hookState = (HookProjectileImpact.HookState)reader.ReadInt32();
			}
			if ((num & 2) != 0)
			{
				this.victim = reader.ReadGameObject();
			}
		}

		// Token: 0x06001E6E RID: 7790 RVA: 0x00016349 File Offset: 0x00014549
		public override void PreStartClient()
		{
			if (!this.___victimNetId.IsEmpty())
			{
				this.Networkvictim = ClientScene.FindLocalObject(this.___victimNetId);
			}
		}

		// Token: 0x04002072 RID: 8306
		private ProjectileController projectileController;

		// Token: 0x04002073 RID: 8307
		public float reelDelayTime;

		// Token: 0x04002074 RID: 8308
		public float reelSpeed = 40f;

		// Token: 0x04002075 RID: 8309
		public float ownerPullFactor = 1f;

		// Token: 0x04002076 RID: 8310
		public float victimPullFactor = 1f;

		// Token: 0x04002077 RID: 8311
		public float maxDistance;

		// Token: 0x04002078 RID: 8312
		public GameObject impactSpark;

		// Token: 0x04002079 RID: 8313
		public GameObject impactSuccess;

		// Token: 0x0400207A RID: 8314
		[SyncVar]
		private HookProjectileImpact.HookState hookState;

		// Token: 0x0400207B RID: 8315
		[SyncVar]
		private GameObject victim;

		// Token: 0x0400207C RID: 8316
		private SetStateOnHurt victimSetStateOnHurt;

		// Token: 0x0400207D RID: 8317
		private Transform ownerTransform;

		// Token: 0x0400207E RID: 8318
		private ProjectileDamage projectileDamage;

		// Token: 0x0400207F RID: 8319
		private Rigidbody rigidbody;

		// Token: 0x04002080 RID: 8320
		private float liveTimer;

		// Token: 0x04002081 RID: 8321
		private float delayTimer;

		// Token: 0x04002082 RID: 8322
		private float flyTimer;

		// Token: 0x04002083 RID: 8323
		private NetworkInstanceId ___victimNetId;

		// Token: 0x02000547 RID: 1351
		private enum HookState
		{
			// Token: 0x04002085 RID: 8325
			Flying,
			// Token: 0x04002086 RID: 8326
			HitDelay,
			// Token: 0x04002087 RID: 8327
			Reel,
			// Token: 0x04002088 RID: 8328
			ReelFail
		}
	}
}
