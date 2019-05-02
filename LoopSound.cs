using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000350 RID: 848
	public class LoopSound : MonoBehaviour
	{
		// Token: 0x0600119D RID: 4509 RVA: 0x000668F4 File Offset: 0x00064AF4
		private void Update()
		{
			this.stopwatch += Time.deltaTime;
			if (this.stopwatch > this.repeatInterval)
			{
				this.stopwatch -= this.repeatInterval;
				Util.PlaySound(this.akSoundString, this.soundOwner.gameObject);
			}
		}

		// Token: 0x04001596 RID: 5526
		public string akSoundString;

		// Token: 0x04001597 RID: 5527
		public float repeatInterval;

		// Token: 0x04001598 RID: 5528
		public Transform soundOwner;

		// Token: 0x04001599 RID: 5529
		private float stopwatch;
	}
}
