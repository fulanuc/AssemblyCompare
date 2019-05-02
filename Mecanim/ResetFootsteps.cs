using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000577 RID: 1399
	public class ResetFootsteps : StateMachineBehaviour
	{
		// Token: 0x06001F59 RID: 8025 RVA: 0x00016F4B File Offset: 0x0001514B
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.GetComponent<FootstepHandler>();
		}
	}
}
