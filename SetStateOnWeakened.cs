using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003D7 RID: 983
	public class SetStateOnWeakened : NetworkBehaviour
	{
		// Token: 0x0600155A RID: 5466 RVA: 0x000101AE File Offset: 0x0000E3AE
		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x0600155B RID: 5467 RVA: 0x00073180 File Offset: 0x00071380
		private void OnTakeDamage(DamageInfo damageInfo)
		{
			if (this.consumed)
			{
				return;
			}
			if (this.targetStateMachine && base.isServer && this.characterBody)
			{
				float num = damageInfo.crit ? (damageInfo.damage * 2f) : damageInfo.damage;
				if ((damageInfo.damageType & DamageType.WeakPointHit) != DamageType.Generic)
				{
					this.accumulatedDamage += num;
					if (this.accumulatedDamage > this.characterBody.maxHealth * this.weakenPercentage)
					{
						this.consumed = true;
						this.SetWeak(damageInfo);
					}
				}
			}
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x00073218 File Offset: 0x00071418
		public void SetWeak(DamageInfo damageInfo)
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
			HurtBox[] array2 = this.weakHurtBox;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].gameObject.SetActive(false);
			}
			if (this.selfDamagePercentage > 0f)
			{
				DamageInfo damageInfo2 = new DamageInfo();
				damageInfo2.damage = this.characterBody.maxHealth * this.selfDamagePercentage / 3f;
				damageInfo2.attacker = damageInfo.attacker;
				damageInfo2.crit = true;
				damageInfo2.position = damageInfo.position;
				damageInfo2.damageType = (DamageType.NonLethal | DamageType.WeakPointHit);
				this.characterBody.healthComponent.TakeDamage(damageInfo2);
			}
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001560 RID: 5472 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001895 RID: 6293
		[Tooltip("The percentage of their max HP they need to take to get weakened. Ranges from 0-1.")]
		public float weakenPercentage = 0.1f;

		// Token: 0x04001896 RID: 6294
		[Tooltip("The percentage of their max HP they deal to themselves once weakened. Ranges from 0-1.")]
		public float selfDamagePercentage;

		// Token: 0x04001897 RID: 6295
		[Tooltip("The state machine to set the state of when this character is hurt.")]
		public EntityStateMachine targetStateMachine;

		// Token: 0x04001898 RID: 6296
		[Tooltip("The state machine to set to idle when this character is hurt.")]
		public EntityStateMachine[] idleStateMachine;

		// Token: 0x04001899 RID: 6297
		[Tooltip("The hurtboxes to set to not a weak point once consumed")]
		public HurtBox[] weakHurtBox;

		// Token: 0x0400189A RID: 6298
		[Tooltip("The state to enter when this character is hurt.")]
		public SerializableEntityStateType hurtState;

		// Token: 0x0400189B RID: 6299
		private float accumulatedDamage;

		// Token: 0x0400189C RID: 6300
		private bool consumed;

		// Token: 0x0400189D RID: 6301
		private CharacterBody characterBody;
	}
}
