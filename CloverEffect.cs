using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002A0 RID: 672
	public class CloverEffect : MonoBehaviour
	{
		// Token: 0x06000DC1 RID: 3521 RVA: 0x0005593C File Offset: 0x00053B3C
		private void Start()
		{
			CharacterBody body = base.GetComponentInParent<CharacterModel>().body;
			this.characterBody = body.GetComponent<CharacterBody>();
		}

		// Token: 0x06000DC2 RID: 3522 RVA: 0x00055964 File Offset: 0x00053B64
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

		// Token: 0x040011A3 RID: 4515
		public GameObject triggerEffect;

		// Token: 0x040011A4 RID: 4516
		private CharacterBody characterBody;

		// Token: 0x040011A5 RID: 4517
		private GameObject triggerEffectInstance;

		// Token: 0x040011A6 RID: 4518
		private bool trigger;
	}
}
