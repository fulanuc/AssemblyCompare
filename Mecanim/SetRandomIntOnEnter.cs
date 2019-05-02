using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000579 RID: 1401
	public class SetRandomIntOnEnter : StateMachineBehaviour
	{
		// Token: 0x06001F5E RID: 8030 RVA: 0x00016F76 File Offset: 0x00015176
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			animator.SetInteger(this.intParameterName, UnityEngine.Random.Range(this.rangeMin, this.rangeMax + 1));
		}

		// Token: 0x040021D2 RID: 8658
		public string intParameterName;

		// Token: 0x040021D3 RID: 8659
		public int rangeMin;

		// Token: 0x040021D4 RID: 8660
		public int rangeMax;
	}
}
