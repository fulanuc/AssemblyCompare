using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002D0 RID: 720
	public class DetachParticleOnDestroyAndEndEmission : MonoBehaviour
	{
		// Token: 0x06000E86 RID: 3718 RVA: 0x00059068 File Offset: 0x00057268
		private void OnDisable()
		{
			if (this.particleSystem)
			{
				this.particleSystem.emission.enabled = false;
				this.particleSystem.Stop();
				this.particleSystem.transform.SetParent(null);
			}
		}

		// Token: 0x0400127D RID: 4733
		public ParticleSystem particleSystem;
	}
}
