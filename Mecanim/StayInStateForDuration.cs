using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x0200056B RID: 1387
	public class StayInStateForDuration : StateMachineBehaviour
	{
		// Token: 0x06001EF6 RID: 7926 RVA: 0x00016AC1 File Offset: 0x00014CC1
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			animator.SetBool(this.deactivationBoolParameterName, true);
		}

		// Token: 0x06001EF7 RID: 7927 RVA: 0x0009812C File Offset: 0x0009632C
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, animatorStateInfo, layerIndex);
			float num = animator.GetFloat(this.stopwatchFloatParameterName);
			float @float = animator.GetFloat(this.durationFloatParameterName);
			num += Time.deltaTime;
			if (num >= @float)
			{
				animator.SetFloat(this.stopwatchFloatParameterName, 0f);
				animator.SetBool(this.deactivationBoolParameterName, false);
				return;
			}
			animator.SetFloat(this.stopwatchFloatParameterName, num);
		}

		// Token: 0x04002197 RID: 8599
		[Tooltip("The reference float - this is how long we will stay in this state")]
		public string durationFloatParameterName;

		// Token: 0x04002198 RID: 8600
		[Tooltip("The counter float - this is exposed incase we want to reset it")]
		public string stopwatchFloatParameterName;

		// Token: 0x04002199 RID: 8601
		[Tooltip("The bool that will be set to 'false' once the duration is up, and 'true' when entering this state.")]
		public string deactivationBoolParameterName;
	}
}
