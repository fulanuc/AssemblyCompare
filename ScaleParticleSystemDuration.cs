using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003C4 RID: 964
	public class ScaleParticleSystemDuration : MonoBehaviour
	{
		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06001500 RID: 5376 RVA: 0x0000FE91 File Offset: 0x0000E091
		// (set) Token: 0x060014FF RID: 5375 RVA: 0x0000FE79 File Offset: 0x0000E079
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

		// Token: 0x06001501 RID: 5377 RVA: 0x0000FE99 File Offset: 0x0000E099
		private void Start()
		{
			this.UpdateParticleDurations();
		}

		// Token: 0x06001502 RID: 5378 RVA: 0x00071D08 File Offset: 0x0006FF08
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

		// Token: 0x04001841 RID: 6209
		public float initialDuration = 1f;

		// Token: 0x04001842 RID: 6210
		private float _newDuration = 1f;

		// Token: 0x04001843 RID: 6211
		public ParticleSystem[] particleSystems;
	}
}
