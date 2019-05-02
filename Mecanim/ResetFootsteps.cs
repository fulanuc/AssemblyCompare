using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000568 RID: 1384
	public class ResetFootsteps : StateMachineBehaviour
	{
		// Token: 0x06001EEF RID: 7919 RVA: 0x00016A6C File Offset: 0x00014C6C
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.GetComponent<FootstepHandler>();
		}
	}
}
