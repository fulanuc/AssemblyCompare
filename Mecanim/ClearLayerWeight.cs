using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000564 RID: 1380
	public class ClearLayerWeight : StateMachineBehaviour
	{
		// Token: 0x06001EE2 RID: 7906 RVA: 0x00097E14 File Offset: 0x00096014
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			int layerIndex2 = layerIndex;
			if (this.layerName.Length > 0)
			{
				layerIndex2 = animator.GetLayerIndex(this.layerName);
			}
			animator.SetLayerWeight(layerIndex2, 0f);
		}

		// Token: 0x06001EE3 RID: 7907 RVA: 0x00097E54 File Offset: 0x00096054
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			int layerIndex2 = layerIndex;
			if (this.layerName.Length > 0)
			{
				layerIndex2 = animator.GetLayerIndex(this.layerName);
			}
			animator.SetLayerWeight(layerIndex2, 1f);
		}

		// Token: 0x04002180 RID: 8576
		public string layerName;
	}
}
