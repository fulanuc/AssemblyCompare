using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000537 RID: 1335
	[RequireComponent(typeof(ProjectileController))]
	public class HookProjectileImpact : NetworkBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001DF8 RID: 7672 RVA: 0x00093AD4 File Offset: 0x00091CD4
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

		// Token: 0x06001DF9 RID: 7673 RVA: 0x00093B50 File Offset: 0x00091D50
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

		// Token: 0x06001DFA RID: 7674 RVA: 0x00093D74 File Offset: 0x00091F74
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

		// Token: 0x06001DFB RID: 7675 RVA: 0x00093FDC File Offset: 0x000921DC
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

		// Token: 0x06001DFD RID: 7677 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06001DFE RID: 7678 RVA: 0x000941D4 File Offset: 0x000923D4
		// (set) Token: 0x06001DFF RID: 7679 RVA: 0x00015E3C File Offset: 0x0001403C
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

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06001E00 RID: 7680 RVA: 0x000941E8 File Offset: 0x000923E8
		// (set) Token: 0x06001E01 RID: 7681 RVA: 0x00015E50 File Offset: 0x00014050
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

		// Token: 0x06001E02 RID: 7682 RVA: 0x000941FC File Offset: 0x000923FC
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

		// Token: 0x06001E03 RID: 7683 RVA: 0x000942A8 File Offset: 0x000924A8
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

		// Token: 0x06001E04 RID: 7684 RVA: 0x00015E6A File Offset: 0x0001406A
		public override void PreStartClient()
		{
			if (!this.___victimNetId.IsEmpty())
			{
				this.Networkvictim = ClientScene.FindLocalObject(this.___victimNetId);
			}
		}

		// Token: 0x04002034 RID: 8244
		private ProjectileController projectileController;

		// Token: 0x04002035 RID: 8245
		public float reelDelayTime;

		// Token: 0x04002036 RID: 8246
		public float reelSpeed = 40f;

		// Token: 0x04002037 RID: 8247
		public float ownerPullFactor = 1f;

		// Token: 0x04002038 RID: 8248
		public float victimPullFactor = 1f;

		// Token: 0x04002039 RID: 8249
		public float maxDistance;

		// Token: 0x0400203A RID: 8250
		public GameObject impactSpark;

		// Token: 0x0400203B RID: 8251
		public GameObject impactSuccess;

		// Token: 0x0400203C RID: 8252
		[SyncVar]
		private HookProjectileImpact.HookState hookState;

		// Token: 0x0400203D RID: 8253
		[SyncVar]
		private GameObject victim;

		// Token: 0x0400203E RID: 8254
		private SetStateOnHurt victimSetStateOnHurt;

		// Token: 0x0400203F RID: 8255
		private Transform ownerTransform;

		// Token: 0x04002040 RID: 8256
		private ProjectileDamage projectileDamage;

		// Token: 0x04002041 RID: 8257
		private Rigidbody rigidbody;

		// Token: 0x04002042 RID: 8258
		private float liveTimer;

		// Token: 0x04002043 RID: 8259
		private float delayTimer;

		// Token: 0x04002044 RID: 8260
		private float flyTimer;

		// Token: 0x04002045 RID: 8261
		private NetworkInstanceId ___victimNetId;

		// Token: 0x02000538 RID: 1336
		private enum HookState
		{
			// Token: 0x04002047 RID: 8263
			Flying,
			// Token: 0x04002048 RID: 8264
			HitDelay,
			// Token: 0x04002049 RID: 8265
			Reel,
			// Token: 0x0400204A RID: 8266
			ReelFail
		}
	}
}
