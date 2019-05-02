using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003D6 RID: 982
	public class SetStateOnHurt : NetworkBehaviour
	{
		// Token: 0x06001551 RID: 5457 RVA: 0x0001018D File Offset: 0x0000E38D
		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x06001552 RID: 5458 RVA: 0x00072FCC File Offset: 0x000711CC
		private void OnTakeDamage(DamageInfo damageInfo)
		{
			if (this.targetStateMachine && base.isServer && this.characterBody)
			{
				float num = damageInfo.crit ? (damageInfo.damage * 2f) : damageInfo.damage;
				if ((damageInfo.damageType & DamageType.Freeze2s) != DamageType.Generic)
				{
					this.SetFrozen(2f);
					return;
				}
				if (!this.characterBody.healthComponent.isFrozen)
				{
					if ((damageInfo.damageType & DamageType.Stun1s) != DamageType.Generic)
					{
						this.SetStun(1f);
						return;
					}
					if (num > this.characterBody.maxHealth * this.hitThreshold)
					{
						this.SetPain();
					}
				}
			}
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x0007307C File Offset: 0x0007127C
		public void SetStun(float duration)
		{
			if (this.targetStateMachine)
			{
				StunState stunState = new StunState();
				stunState.stunDuration = duration;
				this.targetStateMachine.SetInterruptState(stunState, InterruptPriority.Pain);
			}
			EntityStateMachine[] array = this.idleStateMachine;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetNextState(new Idle());
			}
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x000730D4 File Offset: 0x000712D4
		public void SetFrozen(float duration)
		{
			if (this.targetStateMachine)
			{
				FrozenState frozenState = new FrozenState();
				frozenState.freezeDuration = duration;
				this.targetStateMachine.SetInterruptState(frozenState, InterruptPriority.Death);
			}
			EntityStateMachine[] array = this.idleStateMachine;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetNextState(new Idle());
			}
		}

		// Token: 0x06001555 RID: 5461 RVA: 0x0007312C File Offset: 0x0007132C
		public void SetPain()
		{
			if (this.targetStateMachine)
			{
				this.targetStateMachine.SetInterruptState(EntityState.Instantiate(this.hurtState), InterruptPriority.Pain);
			}
			EntityStateMachine[] array = this.idleStateMachine;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetNextState(new Idle());
			}
		}

		// Token: 0x06001557 RID: 5463 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001890 RID: 6288
		[Tooltip("The percentage of their max HP they need to take to get stunned. Ranges from 0-1.")]
		public float hitThreshold = 0.1f;

		// Token: 0x04001891 RID: 6289
		[Tooltip("The state machine to set the state of when this character is hurt.")]
		public EntityStateMachine targetStateMachine;

		// Token: 0x04001892 RID: 6290
		[Tooltip("The state machine to set to idle when this character is hurt.")]
		public EntityStateMachine[] idleStateMachine;

		// Token: 0x04001893 RID: 6291
		[Tooltip("The state to enter when this character is hurt.")]
		public SerializableEntityStateType hurtState;

		// Token: 0x04001894 RID: 6292
		private CharacterBody characterBody;
	}
}
