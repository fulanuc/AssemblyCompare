using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003BD RID: 957
	public class RTPCController : MonoBehaviour
	{
		// Token: 0x06001459 RID: 5209 RVA: 0x000704F4 File Offset: 0x0006E6F4
		private void Start()
		{
			if (this.akSoundString.Length > 0)
			{
				Util.PlaySound(this.akSoundString, base.gameObject, this.rtpcString, this.rtpcValue);
				return;
			}
			AkSoundEngine.SetRTPCValue(this.rtpcString, this.rtpcValue, base.gameObject);
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x0000F6B2 File Offset: 0x0000D8B2
		private void FixedUpdate()
		{
			if (this.useCurveInstead)
			{
				this.fixedAge += Time.fixedDeltaTime;
				AkSoundEngine.SetRTPCValue(this.rtpcString, this.rtpcValueCurve.Evaluate(this.fixedAge), base.gameObject);
			}
		}

		// Token: 0x040017F7 RID: 6135
		public string akSoundString;

		// Token: 0x040017F8 RID: 6136
		public string rtpcString;

		// Token: 0x040017F9 RID: 6137
		public float rtpcValue;

		// Token: 0x040017FA RID: 6138
		public bool useCurveInstead;

		// Token: 0x040017FB RID: 6139
		public AnimationCurve rtpcValueCurve;

		// Token: 0x040017FC RID: 6140
		private float fixedAge;
	}
}
