using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000383 RID: 899
	[RequireComponent(typeof(EffectComponent))]
	public class ParticleSystemColorFromEffectData : MonoBehaviour
	{
		// Token: 0x060012C8 RID: 4808 RVA: 0x0006A660 File Offset: 0x00068860
		private void Start()
		{
			Color color = this.effectComponent.effectData.color;
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				this.particleSystems[i].main.startColor = color;
				this.particleSystems[i].Clear();
				this.particleSystems[i].Play();
			}
		}

		// Token: 0x04001686 RID: 5766
		public ParticleSystem[] particleSystems;

		// Token: 0x04001687 RID: 5767
		public EffectComponent effectComponent;
	}
}
