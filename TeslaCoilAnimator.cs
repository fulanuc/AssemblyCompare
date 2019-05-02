using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000405 RID: 1029
	public class TeslaCoilAnimator : MonoBehaviour
	{
		// Token: 0x06001715 RID: 5909 RVA: 0x00078BAC File Offset: 0x00076DAC
		private void Start()
		{
			CharacterModel componentInParent = base.GetComponentInParent<CharacterModel>();
			if (componentInParent)
			{
				this.characterBody = componentInParent.body;
			}
		}

		// Token: 0x06001716 RID: 5910 RVA: 0x00011627 File Offset: 0x0000F827
		private void FixedUpdate()
		{
			if (this.characterBody)
			{
				this.activeEffectParent.SetActive(this.characterBody.HasBuff(BuffIndex.TeslaField));
			}
		}

		// Token: 0x04001A14 RID: 6676
		public GameObject activeEffectParent;

		// Token: 0x04001A15 RID: 6677
		private CharacterBody characterBody;
	}
}
