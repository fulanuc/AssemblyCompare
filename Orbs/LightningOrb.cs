using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000523 RID: 1315
	public class LightningOrb : Orb
	{
		// Token: 0x06001DC1 RID: 7617 RVA: 0x00090DB0 File Offset: 0x0008EFB0
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

		// Token: 0x06001DC2 RID: 7618 RVA: 0x00090E74 File Offset: 0x0008F074
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

		// Token: 0x06001DC3 RID: 7619 RVA: 0x00091088 File Offset: 0x0008F288
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

		// Token: 0x04001FB6 RID: 8118
		public float speed = 100f;

		// Token: 0x04001FB7 RID: 8119
		public float damageValue;

		// Token: 0x04001FB8 RID: 8120
		public GameObject attacker;

		// Token: 0x04001FB9 RID: 8121
		public int bouncesRemaining;

		// Token: 0x04001FBA RID: 8122
		public List<HealthComponent> bouncedObjects;

		// Token: 0x04001FBB RID: 8123
		public TeamIndex teamIndex;

		// Token: 0x04001FBC RID: 8124
		public bool isCrit;

		// Token: 0x04001FBD RID: 8125
		public ProcChainMask procChainMask;

		// Token: 0x04001FBE RID: 8126
		public float procCoefficient = 1f;

		// Token: 0x04001FBF RID: 8127
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001FC0 RID: 8128
		public float range = 20f;

		// Token: 0x04001FC1 RID: 8129
		public float damageCoefficientPerBounce = 1f;

		// Token: 0x04001FC2 RID: 8130
		private bool canBounceOnSameTarget;

		// Token: 0x04001FC3 RID: 8131
		public LightningOrb.LightningType lightningType;

		// Token: 0x04001FC4 RID: 8132
		private BullseyeSearch search;

		// Token: 0x02000524 RID: 1316
		public enum LightningType
		{
			// Token: 0x04001FC6 RID: 8134
			Ukulele,
			// Token: 0x04001FC7 RID: 8135
			Tesla,
			// Token: 0x04001FC8 RID: 8136
			BFG,
			// Token: 0x04001FC9 RID: 8137
			PaladinBarrier,
			// Token: 0x04001FCA RID: 8138
			HuntressGlaive,
			// Token: 0x04001FCB RID: 8139
			Count
		}
	}
}
