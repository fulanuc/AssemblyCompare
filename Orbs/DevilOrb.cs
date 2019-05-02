using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200050F RID: 1295
	public class DevilOrb : Orb
	{
		// Token: 0x06001D4C RID: 7500 RVA: 0x0008FC4C File Offset: 0x0008DE4C
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

		// Token: 0x06001D4D RID: 7501 RVA: 0x0008FCB8 File Offset: 0x0008DEB8
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

		// Token: 0x06001D4E RID: 7502 RVA: 0x0008FD8C File Offset: 0x0008DF8C
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

		// Token: 0x04001F67 RID: 8039
		private const float speed = 30f;

		// Token: 0x04001F68 RID: 8040
		public float damageValue;

		// Token: 0x04001F69 RID: 8041
		public GameObject attacker;

		// Token: 0x04001F6A RID: 8042
		public TeamIndex teamIndex;

		// Token: 0x04001F6B RID: 8043
		public bool isCrit;

		// Token: 0x04001F6C RID: 8044
		public float scale;

		// Token: 0x04001F6D RID: 8045
		public ProcChainMask procChainMask;

		// Token: 0x04001F6E RID: 8046
		public float procCoefficient = 0.2f;

		// Token: 0x04001F6F RID: 8047
		public DamageColorIndex damageColorIndex;
	}
}
