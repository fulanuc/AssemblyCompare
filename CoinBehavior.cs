﻿using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002A3 RID: 675
	[RequireComponent(typeof(EffectComponent))]
	public class CoinBehavior : MonoBehaviour
	{
		// Token: 0x06000DCB RID: 3531 RVA: 0x00055918 File Offset: 0x00053B18
		private void Start()
		{
			this.originalCoinCount = (int)base.GetComponent<EffectComponent>().effectData.genericFloat;
			int i = this.originalCoinCount;
			for (int j = 0; j < this.coinTiers.Length; j++)
			{
				CoinBehavior.CoinTier coinTier = this.coinTiers[j];
				int num = 0;
				while (i >= coinTier.valuePerCoin)
				{
					i -= coinTier.valuePerCoin;
					num++;
				}
				if (num > 0)
				{
					ParticleSystem.EmissionModule emission = coinTier.particleSystem.emission;
					emission.enabled = true;
					emission.SetBursts(new ParticleSystem.Burst[]
					{
						new ParticleSystem.Burst(0f, (float)num)
					});
					coinTier.particleSystem.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x040011B9 RID: 4537
		public int originalCoinCount;

		// Token: 0x040011BA RID: 4538
		public CoinBehavior.CoinTier[] coinTiers;

		// Token: 0x020002A4 RID: 676
		[Serializable]
		public struct CoinTier
		{
			// Token: 0x040011BB RID: 4539
			public ParticleSystem particleSystem;

			// Token: 0x040011BC RID: 4540
			public int valuePerCoin;
		}
	}
}
