using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200037C RID: 892
	public class OmniEffect : MonoBehaviour
	{
		// Token: 0x060012C5 RID: 4805 RVA: 0x00069F0C File Offset: 0x0006810C
		private void Start()
		{
			this.radius = base.transform.localScale.x;
			foreach (OmniEffect.OmniEffectGroup omniEffectGroup in this.omniEffectGroups)
			{
				float minimumValidRadius = 0f;
				for (int j = 0; j < omniEffectGroup.omniEffectElements.Length; j++)
				{
					OmniEffect.OmniEffectElement omniEffectElement = omniEffectGroup.omniEffectElements[j];
					if (omniEffectElement.maximumValidRadius >= this.radius)
					{
						omniEffectElement.ProcessEffectElement(this.radius, minimumValidRadius);
						break;
					}
					minimumValidRadius = omniEffectElement.maximumValidRadius;
				}
			}
		}

		// Token: 0x04001657 RID: 5719
		public OmniEffect.OmniEffectGroup[] omniEffectGroups;

		// Token: 0x04001658 RID: 5720
		private float radius;

		// Token: 0x0200037D RID: 893
		[Serializable]
		public class OmniEffectElement
		{
			// Token: 0x060012C7 RID: 4807 RVA: 0x00069F98 File Offset: 0x00068198
			public void ProcessEffectElement(float radius, float minimumValidRadius)
			{
				if (this.particleSystem)
				{
					if (this.particleSystemOverrideMaterial)
					{
						ParticleSystemRenderer component = this.particleSystem.GetComponent<ParticleSystemRenderer>();
						if (this.particleSystemOverrideMaterial)
						{
							component.material = this.particleSystemOverrideMaterial;
						}
					}
					ParticleSystem.EmissionModule emission = this.particleSystem.emission;
					int num = (int)emission.GetBurst(0).maxCount + (int)((radius - minimumValidRadius) * this.bonusEmissionPerBonusRadius);
					emission.SetBurst(0, new ParticleSystem.Burst(0f, (float)num));
					if (this.particleSystemEmitParticles)
					{
						this.particleSystem.gameObject.SetActive(true);
						return;
					}
					this.particleSystem.gameObject.SetActive(false);
				}
			}

			// Token: 0x04001659 RID: 5721
			public string name;

			// Token: 0x0400165A RID: 5722
			public ParticleSystem particleSystem;

			// Token: 0x0400165B RID: 5723
			public bool particleSystemEmitParticles;

			// Token: 0x0400165C RID: 5724
			public Material particleSystemOverrideMaterial;

			// Token: 0x0400165D RID: 5725
			public float maximumValidRadius = float.PositiveInfinity;

			// Token: 0x0400165E RID: 5726
			public float bonusEmissionPerBonusRadius;
		}

		// Token: 0x0200037E RID: 894
		[Serializable]
		public class OmniEffectGroup
		{
			// Token: 0x0400165F RID: 5727
			public string name;

			// Token: 0x04001660 RID: 5728
			public OmniEffect.OmniEffectElement[] omniEffectElements;
		}
	}
}
