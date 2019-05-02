using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000389 RID: 905
	public class ParticleSystemRandomColor : MonoBehaviour
	{
		// Token: 0x060012EA RID: 4842 RVA: 0x0006AA70 File Offset: 0x00068C70
		private void Awake()
		{
			if (this.colors.Length != 0)
			{
				Color color = this.colors[UnityEngine.Random.Range(0, this.colors.Length)];
				for (int i = 0; i < this.particleSystems.Length; i++)
				{
					this.particleSystems[i].main.startColor = color;
				}
			}
		}

		// Token: 0x060012EB RID: 4843 RVA: 0x0006AAD0 File Offset: 0x00068CD0
		[AssetCheck(typeof(ParticleSystemRandomColor))]
		private static void CheckParticleSystemRandomColor(ProjectIssueChecker projectIssueChecker, UnityEngine.Object asset)
		{
			ParticleSystemRandomColor particleSystemRandomColor = (ParticleSystemRandomColor)asset;
			for (int i = 0; i < particleSystemRandomColor.particleSystems.Length; i++)
			{
				if (!particleSystemRandomColor.particleSystems[i])
				{
					projectIssueChecker.LogErrorFormat(asset, "Null particle system in slot {0}", new object[]
					{
						i
					});
				}
			}
		}

		// Token: 0x040016A4 RID: 5796
		public Color[] colors;

		// Token: 0x040016A5 RID: 5797
		public ParticleSystem[] particleSystems;
	}
}
