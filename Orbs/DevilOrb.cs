using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200051E RID: 1310
	public class DevilOrb : Orb
	{
		// Token: 0x06001DB4 RID: 7604 RVA: 0x00090A24 File Offset: 0x0008EC24
		public override void Begin()
		{
			base.duration = base.distanceToTarget / 30f;
			EffectData effectData = new EffectData
			{
				scale = this.scale,
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/DevilOrbEffect"), effectData, true);
		}

		// Token: 0x06001DB5 RID: 7605 RVA: 0x00090A90 File Offset: 0x0008EC90
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

		// Token: 0x06001DB6 RID: 7606 RVA: 0x00090B64 File Offset: 0x0008ED64
		public HurtBox PickNextTarget(Vector3 position, float range)
		{
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = position;
			bullseyeSearch.searchDirection = Vector3.zero;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.teamMaskFilter.RemoveTeam(this.teamIndex);
			bullseyeSearch.filterByLoS = false;
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.maxDistanceFilter = range;
			bullseyeSearch.RefreshCandidates();
			List<HurtBox> list = bullseyeSearch.GetResults().ToList<HurtBox>();
			if (list.Count <= 0)
			{
				return null;
			}
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		// Token: 0x04001FA5 RID: 8101
		private const float speed = 30f;

		// Token: 0x04001FA6 RID: 8102
		public float damageValue;

		// Token: 0x04001FA7 RID: 8103
		public GameObject attacker;

		// Token: 0x04001FA8 RID: 8104
		public TeamIndex teamIndex;

		// Token: 0x04001FA9 RID: 8105
		public bool isCrit;

		// Token: 0x04001FAA RID: 8106
		public float scale;

		// Token: 0x04001FAB RID: 8107
		public ProcChainMask procChainMask;

		// Token: 0x04001FAC RID: 8108
		public float procCoefficient = 0.2f;

		// Token: 0x04001FAD RID: 8109
		public DamageColorIndex damageColorIndex;
	}
}
