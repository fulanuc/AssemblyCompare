using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000519 RID: 1305
	public class ArrowOrb : Orb
	{
		// Token: 0x06001DA6 RID: 7590 RVA: 0x00090474 File Offset: 0x0008E674
		public override void Begin()
		{
			base.duration = base.distanceToTarget / 60f;
			EffectData effectData = new EffectData
			{
				scale = this.scale,
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/ArrowOrbEffect"), effectData, true);
		}

		// Token: 0x06001DA7 RID: 7591 RVA: 0x000904E0 File Offset: 0x0008E6E0
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

		// Token: 0x04001F85 RID: 8069
		private const float speed = 60f;

		// Token: 0x04001F86 RID: 8070
		public float damageValue;

		// Token: 0x04001F87 RID: 8071
		public GameObject attacker;

		// Token: 0x04001F88 RID: 8072
		public TeamIndex teamIndex;

		// Token: 0x04001F89 RID: 8073
		public bool isCrit;

		// Token: 0x04001F8A RID: 8074
		public float scale;

		// Token: 0x04001F8B RID: 8075
		public ProcChainMask procChainMask;

		// Token: 0x04001F8C RID: 8076
		public float procCoefficient = 0.2f;

		// Token: 0x04001F8D RID: 8077
		public DamageColorIndex damageColorIndex;
	}
}
