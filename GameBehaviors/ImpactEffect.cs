using System;
using UnityEngine;

namespace RoR2.GameBehaviors
{
	// Token: 0x0200057B RID: 1403
	[RequireComponent(typeof(EffectComponent))]
	internal class ImpactEffect : MonoBehaviour
	{
		// Token: 0x06001F63 RID: 8035 RVA: 0x00098EB0 File Offset: 0x000970B0
		private void Start()
		{
			EffectComponent component = base.GetComponent<EffectComponent>();
			Color color = (component.effectData != null) ? component.effectData.color : Color.white;
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				this.particleSystems[i].main.startColor = color;
				this.particleSystems[i].Play();
			}
		}

		// Token: 0x040021D8 RID: 8664
		public ParticleSystem[] particleSystems;
	}
}
