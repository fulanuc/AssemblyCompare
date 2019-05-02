using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000510 RID: 1296
	public class HealOrb : Orb
	{
		// Token: 0x06001D50 RID: 7504 RVA: 0x0008FE10 File Offset: 0x0008E010
		public override void Begin()
		{
			float scale = this.scaleOrb ? (this.healValue / 10f) : 1f;
			base.duration = base.distanceToTarget / 20f;
			EffectData effectData = new EffectData
			{
				scale = scale,
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/HealthOrbEffect"), effectData, true);
		}

		// Token: 0x06001D51 RID: 7505 RVA: 0x0008FE94 File Offset: 0x0008E094
		public override void OnArrival()
		{
			if (this.target)
			{
				HealthComponent healthComponent = this.target.healthComponent;
				if (healthComponent)
				{
					healthComponent.Heal(this.healValue, default(ProcChainMask), true);
				}
			}
		}

		// Token: 0x04001F70 RID: 8048
		private const float speed = 20f;

		// Token: 0x04001F71 RID: 8049
		public float healValue;

		// Token: 0x04001F72 RID: 8050
		public bool scaleOrb = true;
	}
}
