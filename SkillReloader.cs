using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003EC RID: 1004
	[RequireComponent(typeof(NetworkIdentity))]
	public class SkillReloader : MonoBehaviour
	{
		// Token: 0x0600160B RID: 5643 RVA: 0x000109A4 File Offset: 0x0000EBA4
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x000109B2 File Offset: 0x0000EBB2
		private void Start()
		{
			this.timer = 0f;
		}

		// Token: 0x0600160D RID: 5645 RVA: 0x000752B4 File Offset: 0x000734B4
		private void FixedUpdate()
		{
			if (Util.HasEffectiveAuthority(this.networkIdentity))
			{
				bool flag = this.stateMachine.state.GetType() == typeof(Idle) && !this.stateMachine.HasPendingState();
				if (this.skill.stock < this.skill.maxStock && flag)
				{
					this.timer += Time.fixedDeltaTime;
				}
				else
				{
					this.timer = 0f;
				}
				if (this.timer >= this.reloadDelay || (this.skill.stock == 0 && flag))
				{
					this.stateMachine.SetNextState(EntityState.Instantiate(this.reloadState));
				}
			}
		}

		// Token: 0x04001942 RID: 6466
		private NetworkIdentity networkIdentity;

		// Token: 0x04001943 RID: 6467
		public GenericSkill skill;

		// Token: 0x04001944 RID: 6468
		public EntityStateMachine stateMachine;

		// Token: 0x04001945 RID: 6469
		public SerializableEntityStateType reloadState;

		// Token: 0x04001946 RID: 6470
		public float reloadDelay = 0.2f;

		// Token: 0x04001947 RID: 6471
		private float timer;
	}
}
