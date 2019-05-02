using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x02000300 RID: 768
	[RequireComponent(typeof(CharacterBody))]
	public class GenericSkill : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x06000FAD RID: 4013 RVA: 0x0000C0E2 File Offset: 0x0000A2E2
		public void SetBonusStockFromBody(int newBonusStockFromBody)
		{
			this.bonusStockFromBody = newBonusStockFromBody;
			this.RecalculateMaxStock();
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000FAE RID: 4014 RVA: 0x0000C0F1 File Offset: 0x0000A2F1
		// (set) Token: 0x06000FAF RID: 4015 RVA: 0x0000C0F9 File Offset: 0x0000A2F9
		public int maxStock { get; protected set; }

		// Token: 0x06000FB0 RID: 4016 RVA: 0x0000C102 File Offset: 0x0000A302
		private void RecalculateMaxStock()
		{
			this.maxStock = this.baseMaxStock + this.bonusStockFromBody;
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000FB1 RID: 4017 RVA: 0x0000C117 File Offset: 0x0000A317
		// (set) Token: 0x06000FB2 RID: 4018 RVA: 0x0000C11F File Offset: 0x0000A31F
		public int stock { get; protected set; }

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000FB3 RID: 4019 RVA: 0x0000C128 File Offset: 0x0000A328
		// (set) Token: 0x06000FB4 RID: 4020 RVA: 0x0000C130 File Offset: 0x0000A330
		public float cooldownScale
		{
			get
			{
				return this._cooldownScale;
			}
			set
			{
				if (this._cooldownScale == value)
				{
					return;
				}
				this._cooldownScale = value;
				this.RecalculateFinalRechargeInterval();
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000FB5 RID: 4021 RVA: 0x0000C149 File Offset: 0x0000A349
		public float cooldownRemaining
		{
			get
			{
				if (this.stock != this.maxStock)
				{
					return this.finalRechargeInterval - this.rechargeStopwatch;
				}
				return 0f;
			}
		}

		// Token: 0x06000FB6 RID: 4022 RVA: 0x0000C16C File Offset: 0x0000A36C
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.RecalculateFinalRechargeInterval();
		}

		// Token: 0x06000FB7 RID: 4023 RVA: 0x0000C180 File Offset: 0x0000A380
		protected void Start()
		{
			this.RecalculateMaxStock();
			this.stock = this.maxStock;
		}

		// Token: 0x06000FB8 RID: 4024 RVA: 0x0005DB18 File Offset: 0x0005BD18
		protected void FixedUpdate()
		{
			this.RunRecharge(Time.fixedDeltaTime);
			if (this.canceledFromSprinting && this.characterBody.isSprinting && this.stateMachine.state.GetType() == this.activationState.stateType)
			{
				this.stateMachine.SetNextStateToMain();
			}
		}

		// Token: 0x06000FB9 RID: 4025 RVA: 0x0000A05D File Offset: 0x0000825D
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x06000FBA RID: 4026 RVA: 0x000038B4 File Offset: 0x00001AB4
		public virtual bool CanExecute()
		{
			return true;
		}

		// Token: 0x06000FBB RID: 4027 RVA: 0x0000C194 File Offset: 0x0000A394
		public bool ExecuteIfReady()
		{
			if (this.CanExecute())
			{
				this.OnExecute();
				return true;
			}
			return false;
		}

		// Token: 0x06000FBC RID: 4028 RVA: 0x0005DB74 File Offset: 0x0005BD74
		public void RunRecharge(float dt)
		{
			if (this.stock < this.maxStock)
			{
				if (!this.beginSkillCooldownOnSkillEnd || this.stateMachine.state.GetType() != this.activationState.stateType)
				{
					this.rechargeStopwatch += dt;
				}
				if (this.rechargeStopwatch >= this.finalRechargeInterval)
				{
					this.RestockSteplike();
				}
			}
		}

		// Token: 0x06000FBD RID: 4029 RVA: 0x0000C1A7 File Offset: 0x0000A3A7
		public void Reset()
		{
			this.rechargeStopwatch = 0f;
			this.stock = this.maxStock;
		}

		// Token: 0x06000FBE RID: 4030 RVA: 0x0000C1C0 File Offset: 0x0000A3C0
		public bool CanApplyAmmoPack()
		{
			return this.stock < this.maxStock;
		}

		// Token: 0x06000FBF RID: 4031 RVA: 0x0005DBDC File Offset: 0x0005BDDC
		public void AddOneStock()
		{
			int stock = this.stock + 1;
			this.stock = stock;
		}

		// Token: 0x06000FC0 RID: 4032 RVA: 0x0000C1D3 File Offset: 0x0000A3D3
		public void RemoveAllStocks()
		{
			this.stock = 0;
			this.rechargeStopwatch = 0f;
		}

		// Token: 0x06000FC1 RID: 4033 RVA: 0x0000C1E7 File Offset: 0x0000A3E7
		public void DeductStock(int count)
		{
			this.stock = Mathf.Max(0, this.stock - count);
		}

		// Token: 0x06000FC2 RID: 4034 RVA: 0x0005DBFC File Offset: 0x0005BDFC
		protected virtual void OnExecute()
		{
			this.hasExecutedSuccessfully = false;
			if (this.characterBody.isPlayerControlled ? this.stateMachine.SetInterruptState1(EntityState.Instantiate(this.activationState), this.interruptPriority) : this.stateMachine.SetInterruptState(EntityState.Instantiate(this.activationState), this.interruptPriority))
			{
				this.hasExecutedSuccessfully = true;
				if (this.noSprint)
				{
					this.characterBody.isSprinting = false;
				}
				if (this.isBullets)
				{
					this.rechargeStopwatch = 0f;
				}
				if (this.characterBody)
				{
					this.characterBody.OnSkillActivated(this);
				}
			}
		}

		// Token: 0x06000FC3 RID: 4035 RVA: 0x0005DCA0 File Offset: 0x0005BEA0
		private void RestockContinuous()
		{
			if (this.finalRechargeInterval == 0f)
			{
				this.stock = this.maxStock;
				this.rechargeStopwatch = 0f;
				return;
			}
			int num = Mathf.FloorToInt(this.rechargeStopwatch / this.finalRechargeInterval * (float)this.rechargeStock);
			this.stock = this.maxStock;
			if (this.stock >= this.maxStock)
			{
				this.stock = this.maxStock;
				this.rechargeStopwatch = 0f;
				return;
			}
			this.rechargeStopwatch -= (float)num * this.finalRechargeInterval;
		}

		// Token: 0x06000FC4 RID: 4036 RVA: 0x0005DD38 File Offset: 0x0005BF38
		private void RestockSteplike()
		{
			if (!this.isBullets)
			{
				this.stock = this.maxStock;
				if (this.stock >= this.maxStock)
				{
					this.stock = this.maxStock;
				}
			}
			else
			{
				this.stock = this.maxStock;
			}
			this.rechargeStopwatch = 0f;
		}

		// Token: 0x06000FC5 RID: 4037 RVA: 0x0000C1FD File Offset: 0x0000A3FD
		private void RecalculateFinalRechargeInterval()
		{
			this.finalRechargeInterval = Mathf.Min(this.baseRechargeInterval, Mathf.Max(0.5f, this.baseRechargeInterval * this.cooldownScale));
		}

		// Token: 0x040013A1 RID: 5025
		[Tooltip("The name of the skill. This is mainly for purposes of identification in the inspector and currently has no direct effect.")]
		public string skillName = "";

		// Token: 0x040013A2 RID: 5026
		[Tooltip("The language token with the name of this skill.")]
		public string skillNameToken = "";

		// Token: 0x040013A3 RID: 5027
		[Tooltip("The language token with the description of this skill.")]
		public string skillDescriptionToken = "";

		// Token: 0x040013A4 RID: 5028
		[Tooltip("How long it takes for this skill to recharge after being used.")]
		[FormerlySerializedAs("rechargeInterval")]
		public float baseRechargeInterval = 1f;

		// Token: 0x040013A5 RID: 5029
		[FormerlySerializedAs("maxStock")]
		[Tooltip("Maximum number of charges this skill can carry.")]
		public int baseMaxStock = 1;

		// Token: 0x040013A6 RID: 5030
		[Tooltip("How much stock to restore on a recharge.")]
		public int rechargeStock = 1;

		// Token: 0x040013A7 RID: 5031
		[Tooltip("Whether or not it has bullet reload behavior")]
		public bool isBullets;

		// Token: 0x040013A8 RID: 5032
		[Tooltip("Time between bullets for bullet-style weapons")]
		public float shootDelay = 0.3f;

		// Token: 0x040013A9 RID: 5033
		[Tooltip("Whether or not the cooldown waits until it leaves the set state")]
		public bool beginSkillCooldownOnSkillEnd;

		// Token: 0x040013AA RID: 5034
		[Tooltip("The state machine this skill operates upon.")]
		public EntityStateMachine stateMachine;

		// Token: 0x040013AB RID: 5035
		[Tooltip("The state to enter when this skill is activated.")]
		public SerializableEntityStateType activationState;

		// Token: 0x040013AC RID: 5036
		[Tooltip("The priority of this skill.")]
		public InterruptPriority interruptPriority = InterruptPriority.Skill;

		// Token: 0x040013AD RID: 5037
		[Tooltip("Whether or not this is considered a combat skill.")]
		public bool isCombatSkill = true;

		// Token: 0x040013AE RID: 5038
		[Tooltip("Whether or not the usage of this skill is mutually exclusive with sprinting.")]
		public bool noSprint = true;

		// Token: 0x040013AF RID: 5039
		[Tooltip("Sprinting will actively cancel this ability.")]
		public bool canceledFromSprinting;

		// Token: 0x040013B0 RID: 5040
		[Tooltip("The skill can't be activated if the key is held.")]
		public bool mustKeyPress;

		// Token: 0x040013B1 RID: 5041
		[ShowThumbnail]
		[Tooltip("The icon to display for this skill.")]
		public Sprite icon;

		// Token: 0x040013B2 RID: 5042
		[Tooltip("How much stock is required to activate this skill.")]
		public int requiredStock = 1;

		// Token: 0x040013B3 RID: 5043
		[Tooltip("How much stock to deduct when the skill is activated.")]
		public int stockToConsume = 1;

		// Token: 0x040013B4 RID: 5044
		private CharacterBody characterBody;

		// Token: 0x040013B5 RID: 5045
		protected int bonusStockFromBody;

		// Token: 0x040013B8 RID: 5048
		private float finalRechargeInterval;

		// Token: 0x040013B9 RID: 5049
		private float _cooldownScale = 1f;

		// Token: 0x040013BA RID: 5050
		private float rechargeStopwatch;

		// Token: 0x040013BB RID: 5051
		[HideInInspector]
		public bool hasExecutedSuccessfully;
	}
}
