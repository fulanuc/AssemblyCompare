using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002A2 RID: 674
	public class CloverEffect : MonoBehaviour
	{
		// Token: 0x06000DC8 RID: 3528 RVA: 0x00055880 File Offset: 0x00053A80
		private void Start()
		{
			CharacterBody body = base.GetComponentInParent<CharacterModel>().body;
			this.characterBody = body.GetComponent<CharacterBody>();
		}

		// Token: 0x06000DC9 RID: 3529 RVA: 0x000558A8 File Offset: 0x00053AA8
		private void FixedUpdate()
		{
			if (this.characterBody && this.characterBody.wasLucky)
			{
				this.characterBody.wasLucky = false;
				EffectData effectData = new EffectData();
				effectData.origin = base.transform.position;
				effectData.rotation = base.transform.rotation;
				EffectManager.instance.SpawnEffect(this.triggerEffect, effectData, true);
			}
		}

		// Token: 0x040011B5 RID: 4533
		public GameObject triggerEffect;

		// Token: 0x040011B6 RID: 4534
		private CharacterBody characterBody;

		// Token: 0x040011B7 RID: 4535
		private GameObject triggerEffectInstance;

		// Token: 0x040011B8 RID: 4536
		private bool trigger;
	}
}
