using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000567 RID: 1383
	public class RandomBlinkController : MonoBehaviour
	{
		// Token: 0x06001EED RID: 7917 RVA: 0x00098064 File Offset: 0x00096264
		private void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= 0.25f)
			{
				this.stopwatch = 0f;
				for (int i = 0; i < this.blinkTriggers.Length; i++)
				{
					if (Util.CheckRoll(this.blinkChancePerUpdate, 0f, null))
					{
						this.animator.SetTrigger(this.blinkTriggers[i]);
					}
				}
			}
		}

		// Token: 0x0400218C RID: 8588
		public Animator animator;

		// Token: 0x0400218D RID: 8589
		public string[] blinkTriggers;

		// Token: 0x0400218E RID: 8590
		public float blinkChancePerUpdate;

		// Token: 0x0400218F RID: 8591
		private float stopwatch;

		// Token: 0x04002190 RID: 8592
		private const float updateFrequency = 4f;
	}
}
