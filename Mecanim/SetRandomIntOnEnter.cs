using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x0200056A RID: 1386
	public class SetRandomIntOnEnter : StateMachineBehaviour
	{
		// Token: 0x06001EF4 RID: 7924 RVA: 0x00016A97 File Offset: 0x00014C97
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			animator.SetInteger(this.intParameterName, UnityEngine.Random.Range(this.rangeMin, this.rangeMax + 1));
		}

		// Token: 0x04002194 RID: 8596
		public string intParameterName;

		// Token: 0x04002195 RID: 8597
		public int rangeMin;

		// Token: 0x04002196 RID: 8598
		public int rangeMax;
	}
}
