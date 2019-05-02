using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000257 RID: 599
	[RequireComponent(typeof(CharacterModel))]
	public class AncientWispFireController : MonoBehaviour
	{
		// Token: 0x06000B27 RID: 2855 RVA: 0x00008FA2 File Offset: 0x000071A2
		private void Awake()
		{
			this.characterModel = base.GetComponent<CharacterModel>();
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x0004B0A8 File Offset: 0x000492A8
		private void Update()
		{
			bool flag = false;
			CharacterBody body = this.characterModel.body;
			if (body)
			{
				flag = body.HasBuff(BuffIndex.EnrageAncientWisp);
			}
			if (this.normalParticles)
			{
				ParticleSystem.EmissionModule emission = this.normalParticles.emission;
				if (emission.enabled == flag)
				{
					emission.enabled = !flag;
					if (!flag)
					{
						this.normalParticles.Play();
					}
				}
			}
			if (this.rageParticles)
			{
				ParticleSystem.EmissionModule emission2 = this.rageParticles.emission;
				if (emission2.enabled != flag)
				{
					emission2.enabled = flag;
					if (flag)
					{
						this.rageParticles.Play();
					}
				}
			}
			if (this.normalLight)
			{
				this.normalLight.enabled = !flag;
			}
			if (this.rageLight)
			{
				this.rageLight.enabled = flag;
			}
		}

		// Token: 0x04000F2A RID: 3882
		public ParticleSystem normalParticles;

		// Token: 0x04000F2B RID: 3883
		public Light normalLight;

		// Token: 0x04000F2C RID: 3884
		public ParticleSystem rageParticles;

		// Token: 0x04000F2D RID: 3885
		public Light rageLight;

		// Token: 0x04000F2E RID: 3886
		private CharacterModel characterModel;
	}
}
