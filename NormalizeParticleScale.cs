using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000376 RID: 886
	[RequireComponent(typeof(ParticleSystem))]
	[ExecuteInEditMode]
	public class NormalizeParticleScale : MonoBehaviour
	{
		// Token: 0x060012A2 RID: 4770 RVA: 0x0000E490 File Offset: 0x0000C690
		public void OnEnable()
		{
			this.UpdateParticleSystem();
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x00069AD0 File Offset: 0x00067CD0
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

		// Token: 0x0400163A RID: 5690
		private ParticleSystem particleSystem;
	}
}
