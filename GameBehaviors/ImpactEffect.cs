using System;
using UnityEngine;

namespace RoR2.GameBehaviors
{
	// Token: 0x0200056C RID: 1388
	[RequireComponent(typeof(EffectComponent))]
	internal class ImpactEffect : MonoBehaviour
	{
		// Token: 0x06001EF9 RID: 7929 RVA: 0x00098194 File Offset: 0x00096394
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

		// Token: 0x0400219A RID: 8602
		public ParticleSystem[] particleSystems;
	}
}
