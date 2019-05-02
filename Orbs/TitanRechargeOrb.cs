using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200051C RID: 1308
	public class TitanRechargeOrb : Orb
	{
		// Token: 0x06001D83 RID: 7555 RVA: 0x00090AC0 File Offset: 0x0008ECC0
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

		// Token: 0x06001D84 RID: 7556 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnArrival()
		{
		}

		// Token: 0x04001FBD RID: 8125
		public int targetRockInt;

		// Token: 0x04001FBE RID: 8126
		public TitanRockController titanRockController;
	}
}
