using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000566 RID: 1382
	public class PlaySoundOnEnter : StateMachineBehaviour
	{
		// Token: 0x06001EEA RID: 7914 RVA: 0x00016A32 File Offset: 0x00014C32
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			Util.PlaySound(this.soundString, animator.gameObject);
		}

		// Token: 0x06001EEB RID: 7915 RVA: 0x00016A4F File Offset: 0x00014C4F
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			Util.PlaySound(this.stopSoundString, animator.gameObject);
		}

		// Token: 0x0400218A RID: 8586
		public string soundString;

		// Token: 0x0400218B RID: 8587
		public string stopSoundString;
	}
}
