using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003FF RID: 1023
	public class TeslaCoilAnimator : MonoBehaviour
	{
		// Token: 0x060016D5 RID: 5845 RVA: 0x00078620 File Offset: 0x00076820
		private void Start()
		{
			CharacterModel componentInParent = base.GetComponentInParent<CharacterModel>();
			if (componentInParent)
			{
				this.characterBody = componentInParent.body;
			}
		}

		// Token: 0x060016D6 RID: 5846 RVA: 0x00011202 File Offset: 0x0000F402
		private void FixedUpdate()
		{
			if (this.characterBody)
			{
				this.activeEffectParent.SetActive(this.characterBody.HasBuff(BuffIndex.TeslaField));
			}
		}

		// Token: 0x040019EB RID: 6635
		public GameObject activeEffectParent;

		// Token: 0x040019EC RID: 6636
		private CharacterBody characterBody;
	}
}
