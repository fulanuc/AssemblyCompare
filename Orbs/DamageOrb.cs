using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200051C RID: 1308
	public class DamageOrb : Orb
	{
		// Token: 0x06001DB1 RID: 7601 RVA: 0x00090884 File Offset: 0x0008EA84
		public override void Begin()
		{
			GameObject effectPrefab = null;
			if (this.damageOrbType == DamageOrb.DamageOrbType.ClayGooOrb)
			{
				this.speed = 5f;
				effectPrefab = Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/ClayGooOrbEffect");
				this.orbDamageType = DamageType.ClayGoo;
			}
			base.duration = base.distanceToTarget / this.speed;
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(effectPrefab, effectData, true);
		}

		// Token: 0x06001DB2 RID: 7602 RVA: 0x00090908 File Offset: 0x0008EB08
		public override void OnArrival()
		{
			if (this.target)
			{
				HealthComponent healthComponent = this.target.healthComponent;
				if (healthComponent)
				{
					if (this.damageOrbType == DamageOrb.DamageOrbType.ClayGooOrb)
					{
						CharacterBody component = healthComponent.GetComponent<CharacterBody>();
						if (component && (component.bodyFlags & CharacterBody.BodyFlags.ImmuneToGoo) != CharacterBody.BodyFlags.None)
						{
							healthComponent.Heal(this.damageValue, default(ProcChainMask), true);
							return;
						}
					}
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
					damageInfo.damageType = this.orbDamageType;
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
				}
			}
		}

		// Token: 0x04001F99 RID: 8089
		private float speed = 60f;

		// Token: 0x04001F9A RID: 8090
		public float damageValue;

		// Token: 0x04001F9B RID: 8091
		public GameObject attacker;

		// Token: 0x04001F9C RID: 8092
		public TeamIndex teamIndex;

		// Token: 0x04001F9D RID: 8093
		public bool isCrit;

		// Token: 0x04001F9E RID: 8094
		public ProcChainMask procChainMask;

		// Token: 0x04001F9F RID: 8095
		public float procCoefficient = 0.2f;

		// Token: 0x04001FA0 RID: 8096
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001FA1 RID: 8097
		public DamageOrb.DamageOrbType damageOrbType;

		// Token: 0x04001FA2 RID: 8098
		private DamageType orbDamageType;

		// Token: 0x0200051D RID: 1309
		public enum DamageOrbType
		{
			// Token: 0x04001FA4 RID: 8100
			ClayGooOrb
		}
	}
}
