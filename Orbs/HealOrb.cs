using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200051F RID: 1311
	public class HealOrb : Orb
	{
		// Token: 0x06001DB8 RID: 7608 RVA: 0x00090BE8 File Offset: 0x0008EDE8
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

		// Token: 0x06001DB9 RID: 7609 RVA: 0x00090C6C File Offset: 0x0008EE6C
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

		// Token: 0x04001FAE RID: 8110
		private const float speed = 20f;

		// Token: 0x04001FAF RID: 8111
		public float healValue;

		// Token: 0x04001FB0 RID: 8112
		public bool scaleOrb = true;
	}
}
