using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x02000303 RID: 771
	[RequireComponent(typeof(CharacterBody))]
	public class GenericSkill : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x06000FC3 RID: 4035 RVA: 0x0000C1CC File Offset: 0x0000A3CC
		public void SetBonusStockFromBody(int newBonusStockFromBody)
		{
			this.bonusStockFromBody = newBonusStockFromBody;
			this.RecalculateMaxStock();
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06000FC4 RID: 4036 RVA: 0x0000C1DB File Offset: 0x0000A3DB
		// (set) Token: 0x06000FC5 RID: 4037 RVA: 0x0000C1E3 File Offset: 0x0000A3E3
		public int maxStock { get; protected set; }

		// Token: 0x06000FC6 RID: 4038 RVA: 0x0000C1EC File Offset: 0x0000A3EC
		private void RecalculateMaxStock()
		{
			this.maxStock = this.baseMaxStock + this.bonusStockFromBody;
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x06000FC7 RID: 4039 RVA: 0x0000C201 File Offset: 0x0000A401
		// (set) Token: 0x06000FC8 RID: 4040 RVA: 0x0000C209 File Offset: 0x0000A409
		public int stock { get; protected set; }

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x06000FC9 RID: 4041 RVA: 0x0000C212 File Offset: 0x0000A412
		// (set) Token: 0x06000FCA RID: 4042 RVA: 0x0000C21A File Offset: 0x0000A41A
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

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x06000FCB RID: 4043 RVA: 0x0000C233 File Offset: 0x0000A433
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

		// Token: 0x06000FCC RID: 4044 RVA: 0x0000C256 File Offset: 0x0000A456
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.RecalculateFinalRechargeInterval();
		}

		// Token: 0x06000FCD RID: 4045 RVA: 0x0000C26A File Offset: 0x0000A46A
		protected void Start()
		{
			this.RecalculateMaxStock();
			this.stock = this.maxStock;
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x0005DD38 File Offset: 0x0005BF38
		protected void FixedUpdate()
		{
			this.RunRecharge(Time.fixedDeltaTime);
			if (this.canceledFromSprinting && this.characterBody.isSprinting && this.stateMachine.state.GetType() == this.activationState.stateType)
			{
				this.stateMachine.SetNextStateToMain();
			}
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x0000A0C2 File Offset: 0x000082C2
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x0005DD94 File Offset: 0x0005BF94
		public virtual bool CanExecute()
		{
			if (!this.isBullets)
			{
				return this.stock >= this.requiredStock;
			}
			return (this.stock >= this.requiredStock && this.rechargeStopwatch >= this.shootDelay) || this.stock == this.maxStock;
		}

		// Token: 0x06000FD1 RID: 4049 RVA: 0x0000C27E File Offset: 0x0000A47E
		public bool ExecuteIfReady()
		{
			if (this.CanExecute())
			{
				this.OnExecute();
				return true;
			}
			return false;
		}

		// Token: 0x06000FD2 RID: 4050 RVA: 0x0005DDE8 File Offset: 0x0005BFE8
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

		// Token: 0x06000FD3 RID: 4051 RVA: 0x0000C291 File Offset: 0x0000A491
		public void Reset()
		{
			this.rechargeStopwatch = 0f;
			this.stock = this.maxStock;
		}

		// Token: 0x06000FD4 RID: 4052 RVA: 0x0000C2AA File Offset: 0x0000A4AA
		public bool CanApplyAmmoPack()
		{
			return this.stock < this.maxStock;
		}

		// Token: 0x06000FD5 RID: 4053 RVA: 0x0005DE50 File Offset: 0x0005C050
		public void AddOneStock()
		{
			int stock = this.stock + 1;
			this.stock = stock;
		}

		// Token: 0x06000FD6 RID: 4054 RVA: 0x0000C2BD File Offset: 0x0000A4BD
		public void RemoveAllStocks()
		{
			this.stock = 0;
			this.rechargeStopwatch = 0f;
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x0000C2D1 File Offset: 0x0000A4D1
		public void DeductStock(int count)
		{
			this.stock = Mathf.Max(0, this.stock - count);
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x0005DE70 File Offset: 0x0005C070
		protected virtual void OnExecute()
		{
			this.hasExecutedSuccessfully = false;
			if (this.stateMachine && !this.stateMachine.HasPendingState() && this.stateMachine.SetInterruptState(EntityState.Instantiate(this.activationState), this.interruptPriority))
			{
				this.hasExecutedSuccessfully = true;
				if (this.noSprint)
				{
					this.characterBody.isSprinting = false;
				}
				this.stock -= this.stockToConsume;
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

		// Token: 0x06000FD9 RID: 4057 RVA: 0x0005DF1C File Offset: 0x0005C11C
		private void RestockContinuous()
		{
			if (this.finalRechargeInterval == 0f)
			{
				this.stock = this.maxStock;
				this.rechargeStopwatch = 0f;
				return;
			}
			int num = Mathf.FloorToInt(this.rechargeStopwatch / this.finalRechargeInterval * (float)this.rechargeStock);
			this.stock += num;
			if (this.stock >= this.maxStock)
			{
				this.stock = this.maxStock;
				this.rechargeStopwatch = 0f;
				return;
			}
			this.rechargeStopwatch -= (float)num * this.finalRechargeInterval;
		}

		// Token: 0x06000FDA RID: 4058 RVA: 0x0005DFB4 File Offset: 0x0005C1B4
		private void RestockSteplike()
		{
			if (!this.isBullets)
			{
				this.stock += this.rechargeStock;
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

		// Token: 0x06000FDB RID: 4059 RVA: 0x0000C2E7 File Offset: 0x0000A4E7
		private void RecalculateFinalRechargeInterval()
		{
			this.finalRechargeInterval = Mathf.Min(this.baseRechargeInterval, Mathf.Max(0.5f, this.baseRechargeInterval * this.cooldownScale));
		}

		// Token: 0x040013B9 RID: 5049
		[Tooltip("The name of the skill. This is mainly for purposes of identification in the inspector and currently has no direct effect.")]
		public string skillName = "";

		// Token: 0x040013BA RID: 5050
		[Tooltip("The language token with the name of this skill.")]
		public string skillNameToken = "";

		// Token: 0x040013BB RID: 5051
		[Tooltip("The language token with the description of this skill.")]
		public string skillDescriptionToken = "";

		// Token: 0x040013BC RID: 5052
		[FormerlySerializedAs("rechargeInterval")]
		[Tooltip("How long it takes for this skill to recharge after being used.")]
		public float baseRechargeInterval = 1f;

		// Token: 0x040013BD RID: 5053
		[Tooltip("Maximum number of charges this skill can carry.")]
		[FormerlySerializedAs("maxStock")]
		public int baseMaxStock = 1;

		// Token: 0x040013BE RID: 5054
		[Tooltip("How much stock to restore on a recharge.")]
		public int rechargeStock = 1;

		// Token: 0x040013BF RID: 5055
		[Tooltip("Whether or not it has bullet reload behavior")]
		public bool isBullets;

		// Token: 0x040013C0 RID: 5056
		[Tooltip("Time between bullets for bullet-style weapons")]
		public float shootDelay = 0.3f;

		// Token: 0x040013C1 RID: 5057
		[Tooltip("Whether or not the cooldown waits until it leaves the set state")]
		public bool beginSkillCooldownOnSkillEnd;

		// Token: 0x040013C2 RID: 5058
		[Tooltip("The state machine this skill operates upon.")]
		public EntityStateMachine stateMachine;

		// Token: 0x040013C3 RID: 5059
		[Tooltip("The state to enter when this skill is activated.")]
		public SerializableEntityStateType activationState;

		// Token: 0x040013C4 RID: 5060
		[Tooltip("The priority of this skill.")]
		public InterruptPriority interruptPriority = InterruptPriority.Skill;

		// Token: 0x040013C5 RID: 5061
		[Tooltip("Whether or not this is considered a combat skill.")]
		public bool isCombatSkill = true;

		// Token: 0x040013C6 RID: 5062
		[Tooltip("Whether or not the usage of this skill is mutually exclusive with sprinting.")]
		public bool noSprint = true;

		// Token: 0x040013C7 RID: 5063
		[Tooltip("Sprinting will actively cancel this ability.")]
		public bool canceledFromSprinting;

		// Token: 0x040013C8 RID: 5064
		[Tooltip("The skill can't be activated if the key is held.")]
		public bool mustKeyPress;

		// Token: 0x040013C9 RID: 5065
		[ShowThumbnail]
		[Tooltip("The icon to display for this skill.")]
		public Sprite icon;

		// Token: 0x040013CA RID: 5066
		[Tooltip("How much stock is required to activate this skill.")]
		public int requiredStock = 1;

		// Token: 0x040013CB RID: 5067
		[Tooltip("How much stock to deduct when the skill is activated.")]
		public int stockToConsume = 1;

		// Token: 0x040013CC RID: 5068
		private CharacterBody characterBody;

		// Token: 0x040013CD RID: 5069
		protected int bonusStockFromBody;

		// Token: 0x040013D0 RID: 5072
		private float finalRechargeInterval;

		// Token: 0x040013D1 RID: 5073
		private float _cooldownScale = 1f;

		// Token: 0x040013D2 RID: 5074
		private float rechargeStopwatch;

		// Token: 0x040013D3 RID: 5075
		[HideInInspector]
		public bool hasExecutedSuccessfully;
	}
}
