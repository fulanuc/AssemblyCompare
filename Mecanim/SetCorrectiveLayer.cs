using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000569 RID: 1385
	public class SetCorrectiveLayer : StateMachineBehaviour
	{
		// Token: 0x06001EF1 RID: 7921 RVA: 0x00016A7A File Offset: 0x00014C7A
		public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
		{
			base.OnStateMachineEnter(animator, stateMachinePathHash);
		}

		// Token: 0x06001EF2 RID: 7922 RVA: 0x000980D4 File Offset: 0x000962D4
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			int layerIndex2 = animator.GetLayerIndex(this.referenceOverrideLayerName);
			float target = Mathf.Min(animator.GetLayerWeight(layerIndex2), this.maxWeight);
			float weight = Mathf.SmoothDamp(animator.GetLayerWeight(layerIndex), target, ref this.smoothVelocity, 0.2f);
			animator.SetLayerWeight(layerIndex, weight);
			base.OnStateUpdate(animator, stateInfo, layerIndex);
		}

		// Token: 0x04002191 RID: 8593
		public string referenceOverrideLayerName;

		// Token: 0x04002192 RID: 8594
		public float maxWeight = 1f;

		// Token: 0x04002193 RID: 8595
		private float smoothVelocity;
	}
}
