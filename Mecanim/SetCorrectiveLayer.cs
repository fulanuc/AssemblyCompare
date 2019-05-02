using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000578 RID: 1400
	public class SetCorrectiveLayer : StateMachineBehaviour
	{
		// Token: 0x06001F5B RID: 8027 RVA: 0x00016F59 File Offset: 0x00015159
		public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
		{
			base.OnStateMachineEnter(animator, stateMachinePathHash);
		}

		// Token: 0x06001F5C RID: 8028 RVA: 0x00098DF0 File Offset: 0x00096FF0
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			int layerIndex2 = animator.GetLayerIndex(this.referenceOverrideLayerName);
			float target = Mathf.Min(animator.GetLayerWeight(layerIndex2), this.maxWeight);
			float weight = Mathf.SmoothDamp(animator.GetLayerWeight(layerIndex), target, ref this.smoothVelocity, 0.2f);
			animator.SetLayerWeight(layerIndex, weight);
			base.OnStateUpdate(animator, stateInfo, layerIndex);
		}

		// Token: 0x040021CF RID: 8655
		public string referenceOverrideLayerName;

		// Token: 0x040021D0 RID: 8656
		public float maxWeight = 1f;

		// Token: 0x040021D1 RID: 8657
		private float smoothVelocity;
	}
}
