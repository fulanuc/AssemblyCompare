using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003C2 RID: 962
	public class RTPCController : MonoBehaviour
	{
		// Token: 0x06001478 RID: 5240 RVA: 0x00070730 File Offset: 0x0006E930
		private void Start()
		{
			if (this.akSoundString.Length > 0)
			{
				Util.PlaySound(this.akSoundString, base.gameObject, this.rtpcString, this.rtpcValue);
				return;
			}
			AkSoundEngine.SetRTPCValue(this.rtpcString, this.rtpcValue, base.gameObject);
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x0000F872 File Offset: 0x0000DA72
		private void FixedUpdate()
		{
			if (this.useCurveInstead)
			{
				this.fixedAge += Time.fixedDeltaTime;
				AkSoundEngine.SetRTPCValue(this.rtpcString, this.rtpcValueCurve.Evaluate(this.fixedAge), base.gameObject);
			}
		}

		// Token: 0x04001813 RID: 6163
		public string akSoundString;

		// Token: 0x04001814 RID: 6164
		public string rtpcString;

		// Token: 0x04001815 RID: 6165
		public float rtpcValue;

		// Token: 0x04001816 RID: 6166
		public bool useCurveInstead;

		// Token: 0x04001817 RID: 6167
		public AnimationCurve rtpcValueCurve;

		// Token: 0x04001818 RID: 6168
		private float fixedAge;
	}
}
