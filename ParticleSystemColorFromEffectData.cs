using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000388 RID: 904
	[RequireComponent(typeof(EffectComponent))]
	public class ParticleSystemColorFromEffectData : MonoBehaviour
	{
		// Token: 0x060012E8 RID: 4840 RVA: 0x0006AA04 File Offset: 0x00068C04
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

		// Token: 0x040016A2 RID: 5794
		public ParticleSystem[] particleSystems;

		// Token: 0x040016A3 RID: 5795
		public EffectComponent effectComponent;
	}
}
