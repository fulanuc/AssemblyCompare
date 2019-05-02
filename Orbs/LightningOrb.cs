using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000514 RID: 1300
	public class LightningOrb : Orb
	{
		// Token: 0x06001D59 RID: 7513 RVA: 0x0009003C File Offset: 0x0008E23C
		public override void Begin()
		{
			base.duration = 0.1f;
			string path = null;
			switch (this.lightningType)
			{
			case LightningOrb.LightningType.Ukulele:
				path = "Prefabs/Effects/OrbEffects/LightningOrbEffect";
				break;
			case LightningOrb.LightningType.Tesla:
				path = "Prefabs/Effects/OrbEffects/TeslaOrbEffect";
				break;
			case LightningOrb.LightningType.BFG:
				path = "Prefabs/Effects/OrbEffects/BeamSphereOrbEffect";
				base.duration = 0.4f;
				break;
			case LightningOrb.LightningType.PaladinBarrier:
				path = "Prefabs/Effects/OrbEffects/PaladinBarrierOrbEffect";
				break;
			case LightningOrb.LightningType.HuntressGlaive:
				path = "Prefabs/Effects/OrbEffects/HuntressGlaiveOrbEffect";
				base.duration = base.distanceToTarget / this.speed;
				this.canBounceOnSameTarget = true;
				break;
			}
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>(path), effectData, true);
		}

		// Token: 0x06001D5A RID: 7514 RVA: 0x00090100 File Offset: 0x0008E300
		public override void OnArrival()
		{
			if (this.target)
			{
				if (this.bouncesRemaining > 0)
				{
					if (this.canBounceOnSameTarget)
					{
						this.bouncedObjects.Clear();
					}
					if (this.bouncedObjects != null)
					{
						this.bouncedObjects.Add(this.target.healthComponent);
					}
					HurtBox hurtBox = this.PickNextTarget(this.target.transform.position);
					if (hurtBox)
					{
						LightningOrb lightningOrb = new LightningOrb();
						lightningOrb.search = this.search;
						lightningOrb.origin = this.target.transform.position;
						lightningOrb.target = hurtBox;
						lightningOrb.attacker = this.attacker;
						lightningOrb.teamIndex = this.teamIndex;
						lightningOrb.damageValue = this.damageValue * this.damageCoefficientPerBounce;
						lightningOrb.bouncesRemaining = this.bouncesRemaining - 1;
						lightningOrb.isCrit = this.isCrit;
						lightningOrb.bouncedObjects = this.bouncedObjects;
						lightningOrb.lightningType = this.lightningType;
						lightningOrb.procChainMask = this.procChainMask;
						lightningOrb.procCoefficient = this.procCoefficient;
						lightningOrb.damageColorIndex = this.damageColorIndex;
						lightningOrb.damageCoefficientPerBounce = this.damageCoefficientPerBounce;
						lightningOrb.speed = this.speed;
						lightningOrb.range = this.range;
						OrbManager.instance.AddOrb(lightningOrb);
					}
				}
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

		// Token: 0x06001D5B RID: 7515 RVA: 0x00090314 File Offset: 0x0008E514
		public HurtBox PickNextTarget(Vector3 position)
		{
			if (this.search == null)
			{
				this.search = new BullseyeSearch();
			}
			this.search.searchOrigin = position;
			this.search.searchDirection = Vector3.zero;
			this.search.teamMaskFilter = TeamMask.allButNeutral;
			this.search.teamMaskFilter.RemoveTeam(this.teamIndex);
			this.search.filterByLoS = false;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.maxDistanceFilter = this.range;
			this.search.RefreshCandidates();
			HurtBox hurtBox = (from v in this.search.GetResults()
			where !this.bouncedObjects.Contains(v.healthComponent)
			select v).FirstOrDefault<HurtBox>();
			if (hurtBox)
			{
				this.bouncedObjects.Add(hurtBox.healthComponent);
			}
			return hurtBox;
		}

		// Token: 0x04001F78 RID: 8056
		public float speed = 100f;

		// Token: 0x04001F79 RID: 8057
		public float damageValue;

		// Token: 0x04001F7A RID: 8058
		public GameObject attacker;

		// Token: 0x04001F7B RID: 8059
		public int bouncesRemaining;

		// Token: 0x04001F7C RID: 8060
		public List<HealthComponent> bouncedObjects;

		// Token: 0x04001F7D RID: 8061
		public TeamIndex teamIndex;

		// Token: 0x04001F7E RID: 8062
		public bool isCrit;

		// Token: 0x04001F7F RID: 8063
		public ProcChainMask procChainMask;

		// Token: 0x04001F80 RID: 8064
		public float procCoefficient = 1f;

		// Token: 0x04001F81 RID: 8065
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001F82 RID: 8066
		public float range = 20f;

		// Token: 0x04001F83 RID: 8067
		public float damageCoefficientPerBounce = 1f;

		// Token: 0x04001F84 RID: 8068
		private bool canBounceOnSameTarget;

		// Token: 0x04001F85 RID: 8069
		public LightningOrb.LightningType lightningType;

		// Token: 0x04001F86 RID: 8070
		private BullseyeSearch search;

		// Token: 0x02000515 RID: 1301
		public enum LightningType
		{
			// Token: 0x04001F88 RID: 8072
			Ukulele,
			// Token: 0x04001F89 RID: 8073
			Tesla,
			// Token: 0x04001F8A RID: 8074
			BFG,
			// Token: 0x04001F8B RID: 8075
			PaladinBarrier,
			// Token: 0x04001F8C RID: 8076
			HuntressGlaive,
			// Token: 0x04001F8D RID: 8077
			Count
		}
	}
}
