using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000573 RID: 1395
	public class ClearLayerWeight : StateMachineBehaviour
	{
		// Token: 0x06001F4C RID: 8012 RVA: 0x00098B30 File Offset: 0x00096D30
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

		// Token: 0x06001F4D RID: 8013 RVA: 0x00098B70 File Offset: 0x00096D70
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

		// Token: 0x040021BE RID: 8638
		public string layerName;
	}
}
