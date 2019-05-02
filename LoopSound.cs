using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200034D RID: 845
	public class LoopSound : MonoBehaviour
	{
		// Token: 0x06001186 RID: 4486 RVA: 0x000665BC File Offset: 0x000647BC
		private void Update()
		{
			this.stopwatch += Time.deltaTime;
			if (this.stopwatch > this.repeatInterval)
			{
				this.stopwatch -= this.repeatInterval;
				Util.PlaySound(this.akSoundString, this.soundOwner.gameObject);
			}
		}

		// Token: 0x0400157D RID: 5501
		public string akSoundString;

		// Token: 0x0400157E RID: 5502
		public float repeatInterval;

		// Token: 0x0400157F RID: 5503
		public Transform soundOwner;

		// Token: 0x04001580 RID: 5504
		private float stopwatch;
	}
}
