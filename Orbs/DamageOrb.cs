using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200050D RID: 1293
	public class DamageOrb : Orb
	{
		// Token: 0x06001D49 RID: 7497 RVA: 0x0008FAAC File Offset: 0x0008DCAC
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

		// Token: 0x06001D4A RID: 7498 RVA: 0x0008FB30 File Offset: 0x0008DD30
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

		// Token: 0x04001F5B RID: 8027
		private float speed = 60f;

		// Token: 0x04001F5C RID: 8028
		public float damageValue;

		// Token: 0x04001F5D RID: 8029
		public GameObject attacker;

		// Token: 0x04001F5E RID: 8030
		public TeamIndex teamIndex;

		// Token: 0x04001F5F RID: 8031
		public bool isCrit;

		// Token: 0x04001F60 RID: 8032
		public ProcChainMask procChainMask;

		// Token: 0x04001F61 RID: 8033
		public float procCoefficient = 0.2f;

		// Token: 0x04001F62 RID: 8034
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001F63 RID: 8035
		public DamageOrb.DamageOrbType damageOrbType;

		// Token: 0x04001F64 RID: 8036
		private DamageType orbDamageType;

		// Token: 0x0200050E RID: 1294
		public enum DamageOrbType
		{
			// Token: 0x04001F66 RID: 8038
			ClayGooOrb
		}
	}
}
