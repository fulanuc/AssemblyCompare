using System;
using EntityStates;
using RoR2.Networking;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200030B RID: 779
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CharacterBody))]
	public class HealthComponent : NetworkBehaviour
	{
		// Token: 0x1700015A RID: 346
		// (get) Token: 0x0600101C RID: 4124 RVA: 0x0000C58F File Offset: 0x0000A78F
		public bool alive
		{
			get
			{
				return this.health > 0f;
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x0600101D RID: 4125 RVA: 0x0000C59E File Offset: 0x0000A79E
		public float fullHealth
		{
			get
			{
				return this.body.maxHealth;
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x0600101E RID: 4126 RVA: 0x0000C5AB File Offset: 0x0000A7AB
		public float fullShield
		{
			get
			{
				return this.body.maxShield;
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x0600101F RID: 4127 RVA: 0x0000C5B8 File Offset: 0x0000A7B8
		public float combinedHealth
		{
			get
			{
				return this.health + this.shield;
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06001020 RID: 4128 RVA: 0x0000C5C7 File Offset: 0x0000A7C7
		public float fullCombinedHealth
		{
			get
			{
				return this.fullHealth + this.fullShield;
			}
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06001021 RID: 4129 RVA: 0x0000C5D6 File Offset: 0x0000A7D6
		public float combinedHealthFraction
		{
			get
			{
				return this.combinedHealth / this.fullCombinedHealth;
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06001022 RID: 4130 RVA: 0x0000C5E5 File Offset: 0x0000A7E5
		// (set) Token: 0x06001023 RID: 4131 RVA: 0x0000C5ED File Offset: 0x0000A7ED
		public float lastHitTime { get; private set; }

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06001024 RID: 4132 RVA: 0x0000C5F6 File Offset: 0x0000A7F6
		public float timeSinceLastHit
		{
			get
			{
				return Run.instance.fixedTime - this.lastHitTime;
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06001025 RID: 4133 RVA: 0x0000C609 File Offset: 0x0000A809
		// (set) Token: 0x06001026 RID: 4134 RVA: 0x0000C611 File Offset: 0x0000A811
		public bool godMode { get; set; }

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06001027 RID: 4135 RVA: 0x0000C61A File Offset: 0x0000A81A
		// (set) Token: 0x06001028 RID: 4136 RVA: 0x0000C622 File Offset: 0x0000A822
		public float potionReserve { get; private set; }

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06001029 RID: 4137 RVA: 0x0000C62B File Offset: 0x0000A82B
		// (set) Token: 0x0600102A RID: 4138 RVA: 0x0000C633 File Offset: 0x0000A833
		public bool isInFrozenState { get; set; }

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x0600102B RID: 4139 RVA: 0x00060A34 File Offset: 0x0005EC34
		// (remove) Token: 0x0600102C RID: 4140 RVA: 0x00060A68 File Offset: 0x0005EC68
		public static event Action<HealthComponent, float> onCharacterHealServer;

		// Token: 0x0600102D RID: 4141 RVA: 0x0000C63C File Offset: 0x0000A83C
		public void OnValidate()
		{
			if (base.gameObject.GetComponents<HealthComponent>().Length > 1)
			{
				Debug.LogErrorFormat(base.gameObject, "{0} has multiple health components!!", new object[]
				{
					base.gameObject
				});
			}
		}

		// Token: 0x0600102E RID: 4142 RVA: 0x00060A9C File Offset: 0x0005EC9C
		public float Heal(float amount, ProcChainMask procChainMask, bool nonRegen = true)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Single RoR2.HealthComponent::Heal(System.Single, RoR2.ProcChainMask, System.Boolean)' called on client");
				return 0f;
			}
			if (!this.alive || amount <= 0f)
			{
				return 0f;
			}
			float num = this.health;
			CharacterMaster characterMaster = null;
			Inventory inventory = null;
			bool flag = false;
			if (this.body)
			{
				characterMaster = this.body.master;
				if (characterMaster)
				{
					inventory = characterMaster.inventory;
					if (inventory && inventory.currentEquipmentIndex == EquipmentIndex.LunarPotion && !procChainMask.HasProc(ProcType.LunarPotionActivation))
					{
						this.potionReserve += amount;
						return amount;
					}
					if (nonRegen && !procChainMask.HasProc(ProcType.CritHeal) && Util.CheckRoll(this.body.critHeal, characterMaster))
					{
						procChainMask.AddProc(ProcType.CritHeal);
						flag = true;
					}
				}
			}
			if (flag)
			{
				amount *= 2f;
			}
			if (this.increaseHealingCount > 0)
			{
				amount *= 1f + (float)this.increaseHealingCount;
			}
			if (nonRegen && this.repeatHealComponent && !procChainMask.HasProc(ProcType.RepeatHeal))
			{
				this.repeatHealComponent.healthFractionToRestorePerSecond = 0.1f / (float)this.repeatHealCount;
				this.repeatHealComponent.AddReserve(amount * (float)(1 + this.repeatHealCount), this.fullHealth);
				return 0f;
			}
			this.Networkhealth = Mathf.Min(this.health + amount, this.fullHealth);
			if (nonRegen)
			{
				HealthComponent.SendHeal(base.gameObject, amount, flag);
				if (inventory && !procChainMask.HasProc(ProcType.HealNova))
				{
					int itemCount = inventory.GetItemCount(ItemIndex.NovaOnHeal);
					if (itemCount > 0)
					{
						this.devilOrbHealPool = Mathf.Min(this.devilOrbHealPool + amount * (float)itemCount, this.fullCombinedHealth);
					}
				}
			}
			if (flag)
			{
				GlobalEventManager.instance.OnCrit(this.body, characterMaster, amount / this.fullHealth * 10f, procChainMask);
			}
			if (nonRegen)
			{
				Action<HealthComponent, float> action = HealthComponent.onCharacterHealServer;
				if (action != null)
				{
					action(this, amount);
				}
			}
			return this.health - num;
		}

		// Token: 0x0600102F RID: 4143 RVA: 0x00060C88 File Offset: 0x0005EE88
		public void UsePotion()
		{
			ProcChainMask procChainMask = default(ProcChainMask);
			procChainMask.AddProc(ProcType.LunarPotionActivation);
			this.Heal(this.potionReserve, procChainMask, true);
		}

		// Token: 0x06001030 RID: 4144 RVA: 0x0000C66D File Offset: 0x0000A86D
		public float HealFraction(float fraction, ProcChainMask procChainMask)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Single RoR2.HealthComponent::HealFraction(System.Single, RoR2.ProcChainMask)' called on client");
				return 0f;
			}
			return this.Heal(fraction * this.fullHealth, procChainMask, true);
		}

		// Token: 0x06001031 RID: 4145 RVA: 0x0000C696 File Offset: 0x0000A896
		[Server]
		public void RechargeShieldFull()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::RechargeShieldFull()' called on client");
				return;
			}
			if (this.shield < this.fullShield)
			{
				this.Networkshield = this.fullShield;
			}
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x00060CB8 File Offset: 0x0005EEB8
		[Server]
		public void RechargeShield(float value)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::RechargeShield(System.Single)' called on client");
				return;
			}
			if (this.shield < this.fullShield)
			{
				this.Networkshield = this.shield + value;
				if (this.shield > this.fullShield)
				{
					this.Networkshield = this.fullShield;
				}
			}
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x00060D10 File Offset: 0x0005EF10
		[Server]
		public void TakeDamageForce(DamageInfo damageInfo, bool alwaysApply = false)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::TakeDamageForce(RoR2.DamageInfo,System.Boolean)' called on client");
				return;
			}
			if (this.body.HasBuff(BuffIndex.EngiShield) && this.shield > 0f)
			{
				return;
			}
			CharacterMotor component = base.GetComponent<CharacterMotor>();
			if (component)
			{
				component.ApplyForce(damageInfo.force, alwaysApply);
			}
			Rigidbody component2 = base.GetComponent<Rigidbody>();
			if (component2)
			{
				component2.AddForce(damageInfo.force, ForceMode.Impulse);
			}
		}

		// Token: 0x06001034 RID: 4148 RVA: 0x00060D88 File Offset: 0x0005EF88
		[Server]
		public void TakeDamageForce(Vector3 force, bool alwaysApply = false)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::TakeDamageForce(UnityEngine.Vector3,System.Boolean)' called on client");
				return;
			}
			if (this.body.HasBuff(BuffIndex.EngiShield) && this.shield > 0f)
			{
				return;
			}
			CharacterMotor component = base.GetComponent<CharacterMotor>();
			if (component)
			{
				component.ApplyForce(force, alwaysApply);
			}
			Rigidbody component2 = base.GetComponent<Rigidbody>();
			if (component2)
			{
				component2.AddForce(force, ForceMode.Impulse);
			}
		}

		// Token: 0x06001035 RID: 4149 RVA: 0x00060DF8 File Offset: 0x0005EFF8
		[Server]
		public void TakeDamage(DamageInfo damageInfo)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::TakeDamage(RoR2.DamageInfo)' called on client");
				return;
			}
			if (!this.alive || this.godMode)
			{
				return;
			}
			CharacterBody characterBody = null;
			if (damageInfo.attacker)
			{
				characterBody = damageInfo.attacker.GetComponent<CharacterBody>();
			}
			base.BroadcastMessage("OnIncomingDamage", damageInfo, SendMessageOptions.DontRequireReceiver);
			CharacterMaster master = this.body.master;
			base.GetComponent<TeamComponent>();
			if (master && master.inventory)
			{
				int itemCount = master.inventory.GetItemCount(ItemIndex.Bear);
				if (itemCount > 0 && Util.CheckRoll((1f - 1f / (0.15f * (float)itemCount + 1f)) * 100f, 0f, null))
				{
					EffectData effectData = new EffectData
					{
						origin = damageInfo.position,
						rotation = Util.QuaternionSafeLookRotation((damageInfo.force != Vector3.zero) ? damageInfo.force : UnityEngine.Random.onUnitSphere)
					};
					EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/BearProc"), effectData, true);
					damageInfo.rejected = true;
				}
			}
			if (this.body.HasBuff(BuffIndex.HiddenInvincibility))
			{
				damageInfo.rejected = true;
			}
			if (this.body.HasBuff(BuffIndex.Immune) && (!characterBody || !characterBody.HasBuff(BuffIndex.GoldEmpowered)))
			{
				EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/DamageRejected"), new EffectData
				{
					origin = damageInfo.position
				}, true);
				damageInfo.rejected = true;
			}
			if (damageInfo.rejected)
			{
				return;
			}
			float num = damageInfo.damage;
			if (characterBody)
			{
				CharacterMaster master2 = characterBody.master;
				if (master2 && master2.inventory)
				{
					if (this.combinedHealth >= this.fullCombinedHealth * 0.9f)
					{
						int itemCount2 = master2.inventory.GetItemCount(ItemIndex.Crowbar);
						if (itemCount2 > 0)
						{
							Util.PlaySound("Play_item_proc_crowbar", base.gameObject);
							num *= 1.5f + 0.3f * (float)(itemCount2 - 1);
							EffectManager.instance.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/ImpactCrowbar"), damageInfo.position, -damageInfo.force, true);
						}
					}
					if (this.body.isBoss)
					{
						int itemCount3 = master2.inventory.GetItemCount(ItemIndex.BossDamageBonus);
						if (itemCount3 > 0)
						{
							num *= 1.2f + 0.1f * (float)(itemCount3 - 1);
							damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
							EffectManager.instance.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/ImpactBossDamageBonus"), damageInfo.position, -damageInfo.force, true);
						}
					}
				}
			}
			if (damageInfo.crit)
			{
				num *= 2f;
			}
			if ((damageInfo.damageType & DamageType.WeakPointHit) != DamageType.Generic)
			{
				num *= 1.5f;
				damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
			}
			if ((damageInfo.damageType & DamageType.BypassArmor) == DamageType.Generic)
			{
				float armor = this.body.armor;
				float num2 = (armor >= 0f) ? (1f - armor / (armor + 100f)) : (2f - 100f / (100f - armor));
				num = Mathf.Max(1f, num * num2);
			}
			if ((damageInfo.damageType & DamageType.BarrierBlocked) != DamageType.Generic)
			{
				damageInfo.force *= 0.5f;
				IBarrier component = base.GetComponent<IBarrier>();
				if (component != null)
				{
					component.BlockedDamage(damageInfo, num);
				}
				damageInfo.procCoefficient = 0f;
				num = 0f;
			}
			if (this.hasOneshotProtection)
			{
				num = Mathf.Min(num, this.fullCombinedHealth * 0.9f);
			}
			if ((damageInfo.damageType & DamageType.SlowOnHit) != DamageType.Generic)
			{
				this.body.AddTimedBuff(BuffIndex.Slow50, 2f);
			}
			if ((damageInfo.damageType & DamageType.ClayGoo) != DamageType.Generic && (this.body.bodyFlags & CharacterBody.BodyFlags.ImmuneToGoo) == CharacterBody.BodyFlags.None)
			{
				this.body.AddTimedBuff(BuffIndex.ClayGoo, 2f);
			}
			if (master && master.inventory)
			{
				int itemCount4 = master.inventory.GetItemCount(ItemIndex.GoldOnHit);
				if (itemCount4 > 0)
				{
					float num3 = num / this.fullCombinedHealth;
					uint num4 = (uint)Mathf.Max(master.money * num3 * (float)itemCount4, damageInfo.damage * (float)itemCount4);
					master.money = (uint)Mathf.Max(0f, master.money - num4);
					EffectManager.instance.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CoinImpact"), damageInfo.position, Vector3.up, true);
				}
			}
			if ((damageInfo.damageType & DamageType.NonLethal) != DamageType.Generic)
			{
				num = Mathf.Max(Mathf.Min(num, this.health - 1f), 0f);
			}
			if (this.shield > 0f)
			{
				float num5 = Mathf.Min(num, this.shield);
				float num6 = num - num5;
				this.Networkshield = Mathf.Max(this.shield - num5, 0f);
				if (this.shield == 0f)
				{
					float scale = 1f;
					if (this.body)
					{
						scale = this.body.radius;
					}
					EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShieldBreakEffect"), new EffectData
					{
						origin = base.transform.position,
						scale = scale
					}, true);
				}
				if (num6 > 0f)
				{
					this.Networkhealth = this.health - num6;
				}
			}
			else
			{
				this.Networkhealth = this.health - num;
			}
			this.TakeDamageForce(damageInfo, false);
			base.SendMessage("OnTakeDamage", damageInfo, SendMessageOptions.DontRequireReceiver);
			DamageReport damageReport = new DamageReport
			{
				victim = this,
				damageInfo = damageInfo
			};
			damageReport.damageInfo.damage = num;
			if (num > 0f)
			{
				HealthComponent.SendDamageDealt(damageReport);
			}
			this.UpdateLastHitTime(damageInfo.damage, damageInfo.position, (damageInfo.damageType & DamageType.Silent) > DamageType.Generic);
			if (damageInfo.attacker)
			{
				damageInfo.attacker.SendMessage("OnDamageDealt", damageReport, SendMessageOptions.DontRequireReceiver);
			}
			GlobalEventManager.ServerDamageDealt(damageReport);
			if (this.isInFrozenState && (this.body.bodyFlags & CharacterBody.BodyFlags.ImmuneToExecutes) == CharacterBody.BodyFlags.None && this.combinedHealthFraction < 0.3f)
			{
				this.Networkhealth = -1f;
				EffectManager.instance.SpawnEffect(FrozenState.executeEffectPrefab, new EffectData
				{
					origin = this.body.corePosition,
					scale = (this.body ? this.body.radius : 1f)
				}, true);
			}
			if (!this.alive)
			{
				base.BroadcastMessage("OnKilled", damageInfo, SendMessageOptions.DontRequireReceiver);
				if (damageInfo.attacker)
				{
					damageInfo.attacker.SendMessage("OnKilledOther", damageReport, SendMessageOptions.DontRequireReceiver);
				}
				GlobalEventManager.instance.OnCharacterDeath(damageReport);
				return;
			}
			if (master && master.inventory)
			{
				int itemCount5 = master.inventory.GetItemCount(ItemIndex.Phasing);
				if (itemCount5 > 0 && Util.CheckRoll(damageInfo.damage / this.fullCombinedHealth * 100f, master))
				{
					this.body.AddTimedBuff(BuffIndex.Cloak, 1.5f + (float)itemCount5 * 1.5f);
					this.body.AddTimedBuff(BuffIndex.CloakSpeed, 1.5f + (float)itemCount5 * 1.5f);
					EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ProcStealthkit"), new EffectData
					{
						origin = base.transform.position,
						rotation = Quaternion.identity
					}, true);
				}
			}
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x00061558 File Offset: 0x0005F758
		[Server]
		public void Suicide(GameObject killerOverride = null)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::Suicide(UnityEngine.GameObject)' called on client");
				return;
			}
			if (this.alive)
			{
				DamageInfo damageInfo = new DamageInfo();
				damageInfo.damage = this.health + this.shield;
				damageInfo.position = base.transform.position;
				damageInfo.damageType = DamageType.Generic;
				damageInfo.procCoefficient = 1f;
				if (killerOverride)
				{
					damageInfo.attacker = killerOverride;
				}
				this.Networkhealth = 0f;
				base.BroadcastMessage("OnKilled", damageInfo, SendMessageOptions.DontRequireReceiver);
				GlobalEventManager.instance.OnCharacterDeath(new DamageReport
				{
					damageInfo = damageInfo,
					victim = this
				});
			}
		}

		// Token: 0x06001037 RID: 4151 RVA: 0x00061608 File Offset: 0x0005F808
		public void UpdateLastHitTime(float damageValue, Vector3 damagePosition, bool damageIsSilent)
		{
			if (damageIsSilent)
			{
				return;
			}
			this.lastHitTime = Run.instance.fixedTime;
			if (this.body && this.body.inventory && this.body.inventory.GetItemCount(ItemIndex.Medkit) > 0)
			{
				this.body.AddTimedBuff(BuffIndex.MedkitHeal, 1.1f);
			}
			if (this.modelLocator)
			{
				Transform modelTransform = this.modelLocator.modelTransform;
				if (modelTransform)
				{
					Animator component = modelTransform.GetComponent<Animator>();
					if (component)
					{
						string layerName = "Flinch";
						int layerIndex = component.GetLayerIndex(layerName);
						if (layerIndex >= 0)
						{
							component.SetLayerWeight(layerIndex, 1f + Mathf.Clamp01(damageValue / this.fullCombinedHealth * 10f) * 3f);
							component.Play("FlinchStart", layerIndex);
						}
					}
				}
			}
			IPainAnimationHandler painAnimationHandler = this.painAnimationHandler;
			if (painAnimationHandler == null)
			{
				return;
			}
			painAnimationHandler.HandlePain(damageValue, damagePosition);
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06001038 RID: 4152 RVA: 0x0000C6C7 File Offset: 0x0000A8C7
		private bool hasOneshotProtection
		{
			get
			{
				return this.body && this.body.isPlayerControlled;
			}
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x000616F8 File Offset: 0x0005F8F8
		private void Awake()
		{
			this.body = base.GetComponent<CharacterBody>();
			this.modelLocator = base.GetComponent<ModelLocator>();
			this.painAnimationHandler = base.GetComponent<IPainAnimationHandler>();
			this.lastHitTime = float.NegativeInfinity;
			this.body.onInventoryChanged += this.OnInventoryChanged;
		}

		// Token: 0x0600103A RID: 4154 RVA: 0x0000C6E3 File Offset: 0x0000A8E3
		private void OnDestroy()
		{
			this.body.onInventoryChanged -= this.OnInventoryChanged;
		}

		// Token: 0x0600103B RID: 4155 RVA: 0x0006174C File Offset: 0x0005F94C
		public void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				if (this.body)
				{
					this.regenAccumulator += this.body.regen * Time.fixedDeltaTime;
				}
				if (this.regenAccumulator > 1f)
				{
					float num = Mathf.Floor(this.regenAccumulator);
					this.regenAccumulator -= num;
					this.Heal(num, default(ProcChainMask), false);
				}
				if (this.alive && this.regenAccumulator < -1f)
				{
					float num2 = Mathf.Ceil(this.regenAccumulator);
					this.regenAccumulator -= num2;
					this.Networkhealth = this.health + num2;
					if (this.health <= 0f)
					{
						this.Suicide(null);
					}
				}
				bool flag = this.shield >= this.body.maxShield;
				if (this.body.outOfDanger && !flag)
				{
					this.Networkshield = Mathf.Max(this.body.maxShield, this.shield + this.body.maxShield * 0.5f * Time.fixedDeltaTime);
				}
				if (this.shield >= this.body.maxShield && !flag)
				{
					Util.PlaySound("Play_item_proc_personal_shield_end", base.gameObject);
				}
				if (this.devilOrbHealPool > 0f)
				{
					this.devilOrbTimer -= Time.fixedDeltaTime;
					if (this.devilOrbTimer <= 0f)
					{
						this.devilOrbTimer += 0.1f;
						float scale = 1f;
						float num3 = this.fullCombinedHealth / 10f;
						float num4 = 2.5f;
						this.devilOrbHealPool -= num3;
						DevilOrb devilOrb = new DevilOrb();
						devilOrb.origin = this.body.aimOriginTransform.position;
						devilOrb.damageValue = num3 * num4;
						devilOrb.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
						devilOrb.attacker = base.gameObject;
						devilOrb.damageColorIndex = DamageColorIndex.Poison;
						devilOrb.scale = scale;
						devilOrb.procChainMask.AddProc(ProcType.HealNova);
						HurtBox hurtBox = devilOrb.PickNextTarget(devilOrb.origin, 40f);
						if (hurtBox)
						{
							devilOrb.target = hurtBox;
							devilOrb.isCrit = Util.CheckRoll(this.body.crit, this.body.master);
							OrbManager.instance.AddOrb(devilOrb);
						}
					}
				}
			}
			if (this.combatHealthbarTransform)
			{
				this.combatHealthTimer -= Time.fixedDeltaTime;
				if (this.combatHealthTimer <= 0f || !this.alive || this.body.HasBuff(BuffIndex.Cloak))
				{
					this.combatHealthTimer = 0f;
					UnityEngine.Object.Destroy(this.combatHealthbarTransform.gameObject);
				}
			}
			if (!this.alive && this.wasAlive)
			{
				this.wasAlive = false;
				base.BroadcastMessage("OnDeath", null, SendMessageOptions.DontRequireReceiver);
			}
		}

		// Token: 0x0600103C RID: 4156 RVA: 0x00061A4C File Offset: 0x0005FC4C
		private static void SendDamageDealt(DamageReport damageReport)
		{
			DamageInfo damageInfo = damageReport.damageInfo;
			NetworkServer.SendToAll(60, new DamageDealtMessage
			{
				victim = damageReport.victim.gameObject,
				damage = damageInfo.damage,
				attacker = damageInfo.attacker,
				position = damageInfo.position,
				crit = damageInfo.crit,
				damageType = damageInfo.damageType,
				damageColorIndex = damageInfo.damageColorIndex
			});
		}

		// Token: 0x0600103D RID: 4157 RVA: 0x00061AC8 File Offset: 0x0005FCC8
		[NetworkMessageHandler(msgType = 60, client = true)]
		private static void HandleDamageDealt(NetworkMessage netMsg)
		{
			DamageDealtMessage damageDealtMessage = netMsg.ReadMessage<DamageDealtMessage>();
			if (damageDealtMessage.victim)
			{
				HealthComponent component = damageDealtMessage.victim.GetComponent<HealthComponent>();
				if (component && !NetworkServer.active)
				{
					component.UpdateLastHitTime(damageDealtMessage.damage, damageDealtMessage.position, damageDealtMessage.isSilent);
				}
			}
			if (RoR2Application.enableDamageNumbers.value && DamageNumberManager.instance)
			{
				TeamComponent teamComponent = null;
				if (damageDealtMessage.attacker)
				{
					teamComponent = damageDealtMessage.attacker.GetComponent<TeamComponent>();
				}
				DamageNumberManager.instance.SpawnDamageNumber(damageDealtMessage.damage, damageDealtMessage.position, damageDealtMessage.crit, teamComponent ? teamComponent.teamIndex : TeamIndex.None, damageDealtMessage.damageColorIndex);
			}
			GlobalEventManager.ClientDamageNotified(damageDealtMessage);
		}

		// Token: 0x0600103E RID: 4158 RVA: 0x00061B88 File Offset: 0x0005FD88
		private static void SendHeal(GameObject target, float amount, bool isCrit)
		{
			NetworkServer.SendToAll(61, new HealthComponent.HealMessage
			{
				target = target,
				amount = (isCrit ? (-amount) : amount)
			});
		}

		// Token: 0x0600103F RID: 4159 RVA: 0x00061BBC File Offset: 0x0005FDBC
		[NetworkMessageHandler(msgType = 61, client = true)]
		private static void HandleHeal(NetworkMessage netMsg)
		{
			HealthComponent.HealMessage healMessage = netMsg.ReadMessage<HealthComponent.HealMessage>();
			if (RoR2Application.enableDamageNumbers.value && healMessage.target && DamageNumberManager.instance)
			{
				DamageNumberManager.instance.SpawnDamageNumber(healMessage.amount, Util.GetCorePosition(healMessage.target), healMessage.amount < 0f, TeamIndex.None, DamageColorIndex.Heal);
			}
		}

		// Token: 0x06001040 RID: 4160 RVA: 0x00061C20 File Offset: 0x0005FE20
		private void OnInventoryChanged()
		{
			if (NetworkServer.active)
			{
				this.repeatHealCount = this.body.inventory.GetItemCount(ItemIndex.RepeatHeal);
				this.increaseHealingCount = this.body.inventory.GetItemCount(ItemIndex.IncreaseHealing);
				bool flag = this.repeatHealCount != 0;
				if (flag != this.repeatHealComponent)
				{
					if (flag)
					{
						this.repeatHealComponent = base.gameObject.AddComponent<HealthComponent.RepeatHealComponent>();
						this.repeatHealComponent.healthComponent = this;
						return;
					}
					UnityEngine.Object.Destroy(this.repeatHealComponent);
					this.repeatHealComponent = null;
				}
			}
		}

		// Token: 0x06001042 RID: 4162 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06001043 RID: 4163 RVA: 0x00061CB0 File Offset: 0x0005FEB0
		// (set) Token: 0x06001044 RID: 4164 RVA: 0x0000C716 File Offset: 0x0000A916
		public float Networkhealth
		{
			get
			{
				return this.health;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.health, 1u);
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06001045 RID: 4165 RVA: 0x00061CC4 File Offset: 0x0005FEC4
		// (set) Token: 0x06001046 RID: 4166 RVA: 0x0000C72A File Offset: 0x0000A92A
		public float Networkshield
		{
			get
			{
				return this.shield;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.shield, 2u);
			}
		}

		// Token: 0x06001047 RID: 4167 RVA: 0x00061CD8 File Offset: 0x0005FED8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.health);
				writer.Write(this.shield);
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
				writer.Write(this.health);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.shield);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001048 RID: 4168 RVA: 0x00061D84 File Offset: 0x0005FF84
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.health = reader.ReadSingle();
				this.shield = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.health = reader.ReadSingle();
			}
			if ((num & 2) != 0)
			{
				this.shield = reader.ReadSingle();
			}
		}

		// Token: 0x04001410 RID: 5136
		[HideInInspector]
		[SyncVar]
		[Tooltip("How much health this object has.")]
		public float health = 100f;

		// Token: 0x04001411 RID: 5137
		[Tooltip("How much shield this object has.")]
		[HideInInspector]
		[SyncVar]
		public float shield;

		// Token: 0x04001412 RID: 5138
		public CharacterBody body;

		// Token: 0x04001413 RID: 5139
		private Transform combatHealthbarTransform;

		// Token: 0x04001414 RID: 5140
		private ModelLocator modelLocator;

		// Token: 0x04001415 RID: 5141
		private IPainAnimationHandler painAnimationHandler;

		// Token: 0x04001418 RID: 5144
		public bool dontShowHealthbar;

		// Token: 0x04001419 RID: 5145
		private float devilOrbHealPool;

		// Token: 0x0400141A RID: 5146
		private float devilOrbTimer;

		// Token: 0x0400141B RID: 5147
		private const float devilOrbMaxTimer = 0.1f;

		// Token: 0x0400141C RID: 5148
		private float regenAccumulator;

		// Token: 0x0400141D RID: 5149
		private float combatHealthTimer;

		// Token: 0x0400141E RID: 5150
		private bool wasAlive = true;

		// Token: 0x0400141F RID: 5151
		public const float medkitActivationDelay = 1.1f;

		// Token: 0x04001422 RID: 5154
		public const float frozenExecuteThreshold = 0.3f;

		// Token: 0x04001424 RID: 5156
		private int increaseHealingCount;

		// Token: 0x04001425 RID: 5157
		private int repeatHealCount;

		// Token: 0x04001426 RID: 5158
		private HealthComponent.RepeatHealComponent repeatHealComponent;

		// Token: 0x0200030C RID: 780
		private class HealMessage : MessageBase
		{
			// Token: 0x0600104A RID: 4170 RVA: 0x0000C73E File Offset: 0x0000A93E
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.target);
				writer.Write(this.amount);
			}

			// Token: 0x0600104B RID: 4171 RVA: 0x0000C758 File Offset: 0x0000A958
			public override void Deserialize(NetworkReader reader)
			{
				this.target = reader.ReadGameObject();
				this.amount = reader.ReadSingle();
			}

			// Token: 0x04001427 RID: 5159
			public GameObject target;

			// Token: 0x04001428 RID: 5160
			public float amount;
		}

		// Token: 0x0200030D RID: 781
		private class RepeatHealComponent : MonoBehaviour
		{
			// Token: 0x0600104C RID: 4172 RVA: 0x00061DEC File Offset: 0x0005FFEC
			private void FixedUpdate()
			{
				this.timer -= Time.fixedDeltaTime;
				if (this.timer <= 0f)
				{
					this.timer = 0.2f;
					if (this.reserve > 0f)
					{
						float num = Mathf.Min(this.healthComponent.fullHealth * this.healthFractionToRestorePerSecond * 0.2f, this.reserve);
						this.reserve -= num;
						ProcChainMask procChainMask = default(ProcChainMask);
						procChainMask.AddProc(ProcType.RepeatHeal);
						this.healthComponent.Heal(num, procChainMask, true);
					}
				}
			}

			// Token: 0x0600104D RID: 4173 RVA: 0x0000C772 File Offset: 0x0000A972
			public void AddReserve(float amount, float max)
			{
				this.reserve = Mathf.Min(this.reserve + amount, max);
			}

			// Token: 0x04001429 RID: 5161
			private float reserve;

			// Token: 0x0400142A RID: 5162
			private float timer;

			// Token: 0x0400142B RID: 5163
			private const float interval = 0.2f;

			// Token: 0x0400142C RID: 5164
			public float healthFractionToRestorePerSecond = 0.1f;

			// Token: 0x0400142D RID: 5165
			public HealthComponent healthComponent;
		}
	}
}
