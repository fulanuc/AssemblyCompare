using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200052B RID: 1323
	public class TitanRechargeOrb : Orb
	{
		// Token: 0x06001DEB RID: 7659 RVA: 0x0009183C File Offset: 0x0008FA3C
		public override void Begin()
		{
			base.duration = 1f;
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/HealthOrbEffect"), effectData, true);
		}

		// Token: 0x06001DEC RID: 7660 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnArrival()
		{
		}

		// Token: 0x04001FFB RID: 8187
		public int targetRockInt;

		// Token: 0x04001FFC RID: 8188
		public TitanRockController titanRockController;
	}
}
