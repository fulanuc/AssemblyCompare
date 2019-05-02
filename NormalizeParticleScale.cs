using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000379 RID: 889
	[ExecuteInEditMode]
	[RequireComponent(typeof(ParticleSystem))]
	public class NormalizeParticleScale : MonoBehaviour
	{
		// Token: 0x060012B9 RID: 4793 RVA: 0x0000E588 File Offset: 0x0000C788
		public void OnEnable()
		{
			this.UpdateParticleSystem();
		}

		// Token: 0x060012BA RID: 4794 RVA: 0x00069E04 File Offset: 0x00068004
		private void UpdateParticleSystem()
		{
			if (!this.particleSystem)
			{
				this.particleSystem = base.GetComponent<ParticleSystem>();
			}
			ParticleSystem.MainModule main = this.particleSystem.main;
			ParticleSystem.MinMaxCurve startSize = main.startSize;
			Vector3 lossyScale = base.transform.lossyScale;
			float num = Mathf.Max(new float[]
			{
				lossyScale.x,
				lossyScale.y,
				lossyScale.z
			});
			startSize.constantMin /= num;
			startSize.constantMax /= num;
			main.startSize = startSize;
		}

		// Token: 0x04001653 RID: 5715
		private ParticleSystem particleSystem;
	}
}
