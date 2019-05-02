using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000575 RID: 1397
	public class PlaySoundOnEnter : StateMachineBehaviour
	{
		// Token: 0x06001F54 RID: 8020 RVA: 0x00016F11 File Offset: 0x00015111
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			Util.PlaySound(this.soundString, animator.gameObject);
		}

		// Token: 0x06001F55 RID: 8021 RVA: 0x00016F2E File Offset: 0x0001512E
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			Util.PlaySound(this.stopSoundString, animator.gameObject);
		}

		// Token: 0x040021C8 RID: 8648
		public string soundString;

		// Token: 0x040021C9 RID: 8649
		public string stopSoundString;
	}
}
