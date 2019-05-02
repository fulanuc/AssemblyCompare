using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000384 RID: 900
	public class ParticleSystemRandomColor : MonoBehaviour
	{
		// Token: 0x060012CA RID: 4810 RVA: 0x0006A6CC File Offset: 0x000688CC
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

		// Token: 0x060012CB RID: 4811 RVA: 0x0006A72C File Offset: 0x0006892C
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

		// Token: 0x04001688 RID: 5768
		public Color[] colors;

		// Token: 0x04001689 RID: 5769
		public ParticleSystem[] particleSystems;
	}
}
