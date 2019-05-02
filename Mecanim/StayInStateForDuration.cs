using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x0200057A RID: 1402
	public class StayInStateForDuration : StateMachineBehaviour
	{
		// Token: 0x06001F60 RID: 8032 RVA: 0x00016FA0 File Offset: 0x000151A0
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			animator.SetBool(this.deactivationBoolParameterName, true);
		}

		// Token: 0x06001F61 RID: 8033 RVA: 0x00098E48 File Offset: 0x00097048
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

		// Token: 0x040021D5 RID: 8661
		[Tooltip("The reference float - this is how long we will stay in this state")]
		public string durationFloatParameterName;

		// Token: 0x040021D6 RID: 8662
		[Tooltip("The counter float - this is exposed incase we want to reset it")]
		public string stopwatchFloatParameterName;

		// Token: 0x040021D7 RID: 8663
		[Tooltip("The bool that will be set to 'false' once the duration is up, and 'true' when entering this state.")]
		public string deactivationBoolParameterName;
	}
}
