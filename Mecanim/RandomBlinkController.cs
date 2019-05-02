using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000576 RID: 1398
	public class RandomBlinkController : MonoBehaviour
	{
		// Token: 0x06001F57 RID: 8023 RVA: 0x00098D80 File Offset: 0x00096F80
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

		// Token: 0x040021CA RID: 8650
		public Animator animator;

		// Token: 0x040021CB RID: 8651
		public string[] blinkTriggers;

		// Token: 0x040021CC RID: 8652
		public float blinkChancePerUpdate;

		// Token: 0x040021CD RID: 8653
		private float stopwatch;

		// Token: 0x040021CE RID: 8654
		private const float updateFrequency = 4f;
	}
}
