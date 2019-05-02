using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000257 RID: 599
	[RequireComponent(typeof(CharacterModel))]
	public class AncientWispFireController : MonoBehaviour
	{
		// Token: 0x06000B2A RID: 2858 RVA: 0x00008FC7 File Offset: 0x000071C7
		private void Awake()
		{
			this.characterModel = base.GetComponent<CharacterModel>();
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x0004B2B4 File Offset: 0x000494B4
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

		// Token: 0x04000F30 RID: 3888
		public ParticleSystem normalParticles;

		// Token: 0x04000F31 RID: 3889
		public Light normalLight;

		// Token: 0x04000F32 RID: 3890
		public ParticleSystem rageParticles;

		// Token: 0x04000F33 RID: 3891
		public Light rageLight;

		// Token: 0x04000F34 RID: 3892
		private CharacterModel characterModel;
	}
}
