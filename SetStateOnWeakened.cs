using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003DD RID: 989
	public class SetStateOnWeakened : NetworkBehaviour
	{
		// Token: 0x06001598 RID: 5528 RVA: 0x000105B7 File Offset: 0x0000E7B7
		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x06001599 RID: 5529 RVA: 0x00073800 File Offset: 0x00071A00
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

		// Token: 0x0600159A RID: 5530 RVA: 0x00073898 File Offset: 0x00071A98
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

		// Token: 0x0600159C RID: 5532 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x0600159D RID: 5533 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018BE RID: 6334
		[Tooltip("The percentage of their max HP they need to take to get weakened. Ranges from 0-1.")]
		public float weakenPercentage = 0.1f;

		// Token: 0x040018BF RID: 6335
		[Tooltip("The percentage of their max HP they deal to themselves once weakened. Ranges from 0-1.")]
		public float selfDamagePercentage;

		// Token: 0x040018C0 RID: 6336
		[Tooltip("The state machine to set the state of when this character is hurt.")]
		public EntityStateMachine targetStateMachine;

		// Token: 0x040018C1 RID: 6337
		[Tooltip("The state machine to set to idle when this character is hurt.")]
		public EntityStateMachine[] idleStateMachine;

		// Token: 0x040018C2 RID: 6338
		[Tooltip("The hurtboxes to set to not a weak point once consumed")]
		public HurtBox[] weakHurtBox;

		// Token: 0x040018C3 RID: 6339
		[Tooltip("The state to enter when this character is hurt.")]
		public SerializableEntityStateType hurtState;

		// Token: 0x040018C4 RID: 6340
		private float accumulatedDamage;

		// Token: 0x040018C5 RID: 6341
		private bool consumed;

		// Token: 0x040018C6 RID: 6342
		private CharacterBody characterBody;
	}
}
