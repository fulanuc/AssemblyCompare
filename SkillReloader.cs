using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E6 RID: 998
	[RequireComponent(typeof(NetworkIdentity))]
	public class SkillReloader : MonoBehaviour
	{
		// Token: 0x060015CE RID: 5582 RVA: 0x0001059B File Offset: 0x0000E79B
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
		}

		// Token: 0x060015CF RID: 5583 RVA: 0x000105A9 File Offset: 0x0000E7A9
		private void Start()
		{
			this.timer = 0f;
		}

		// Token: 0x060015D0 RID: 5584 RVA: 0x00074C7C File Offset: 0x00072E7C
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

		// Token: 0x04001919 RID: 6425
		private NetworkIdentity networkIdentity;

		// Token: 0x0400191A RID: 6426
		public GenericSkill skill;

		// Token: 0x0400191B RID: 6427
		public EntityStateMachine stateMachine;

		// Token: 0x0400191C RID: 6428
		public SerializableEntityStateType reloadState;

		// Token: 0x0400191D RID: 6429
		public float reloadDelay = 0.2f;

		// Token: 0x0400191E RID: 6430
		private float timer;
	}
}
