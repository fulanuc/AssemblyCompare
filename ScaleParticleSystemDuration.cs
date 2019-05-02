using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003CA RID: 970
	public class ScaleParticleSystemDuration : MonoBehaviour
	{
		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06001527 RID: 5415 RVA: 0x00010101 File Offset: 0x0000E301
		// (set) Token: 0x06001526 RID: 5414 RVA: 0x000100E9 File Offset: 0x0000E2E9
		public float newDuration
		{
			get
			{
				return this._newDuration;
			}
			set
			{
				if (this._newDuration != value)
				{
					this._newDuration = value;
					this.UpdateParticleDurations();
				}
			}
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x00010109 File Offset: 0x0000E309
		private void Start()
		{
			this.UpdateParticleDurations();
		}

		// Token: 0x06001529 RID: 5417 RVA: 0x00072068 File Offset: 0x00070268
		private void UpdateParticleDurations()
		{
			float simulationSpeed = this.initialDuration / this._newDuration;
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				ParticleSystem particleSystem = this.particleSystems[i];
				if (particleSystem)
				{
					particleSystem.main.simulationSpeed = simulationSpeed;
				}
			}
		}

		// Token: 0x04001860 RID: 6240
		public float initialDuration = 1f;

		// Token: 0x04001861 RID: 6241
		private float _newDuration = 1f;

		// Token: 0x04001862 RID: 6242
		public ParticleSystem[] particleSystems;
	}
}
