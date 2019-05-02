using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200050A RID: 1290
	public class ArrowOrb : Orb
	{
		// Token: 0x06001D3E RID: 7486 RVA: 0x0008F6A4 File Offset: 0x0008D8A4
		public override void Begin()
		{
			base.duration = 0.1f;
			EffectData effectData = new EffectData
			{
				scale = this.scale,
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/ArrowOrbEffect"), effectData, true);
		}

		// Token: 0x06001D3F RID: 7487 RVA: 0x0008F708 File Offset: 0x0008D908
		public override void OnArrival()
		{
			if (this.target)
			{
				HealthComponent healthComponent = this.target.healthComponent;
				if (healthComponent)
				{
					DamageInfo damageInfo = new DamageInfo();
					damageInfo.damage = this.damageValue;
					damageInfo.attacker = this.attacker;
					damageInfo.inflictor = null;
					damageInfo.force = Vector3.zero;
					damageInfo.crit = this.isCrit;
					damageInfo.procChainMask = this.procChainMask;
					damageInfo.procCoefficient = this.procCoefficient;
					damageInfo.position = this.target.transform.position;
					damageInfo.damageColorIndex = this.damageColorIndex;
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
				}
			}
		}

		// Token: 0x04001F47 RID: 8007
		private const float speed = 60f;

		// Token: 0x04001F48 RID: 8008
		public float damageValue;

		// Token: 0x04001F49 RID: 8009
		public GameObject attacker;

		// Token: 0x04001F4A RID: 8010
		public TeamIndex teamIndex;

		// Token: 0x04001F4B RID: 8011
		public bool isCrit;

		// Token: 0x04001F4C RID: 8012
		public float scale;

		// Token: 0x04001F4D RID: 8013
		public ProcChainMask procChainMask;

		// Token: 0x04001F4E RID: 8014
		public float procCoefficient = 0.2f;

		// Token: 0x04001F4F RID: 8015
		public DamageColorIndex damageColorIndex;
	}
}
