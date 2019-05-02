using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200051B RID: 1307
	public class BounceOrb : Orb
	{
		// Token: 0x06001DAC RID: 7596 RVA: 0x00090670 File Offset: 0x0008E870
		public override void Begin()
		{
			base.duration = base.distanceToTarget / 70f;
			EffectData effectData = new EffectData
			{
				scale = this.scale,
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/BounceOrbEffect"), effectData, true);
		}

		// Token: 0x06001DAD RID: 7597 RVA: 0x000906DC File Offset: 0x0008E8DC
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
					damageInfo.force = Vector3.Normalize(this.target.transform.position - this.origin) * -1000f;
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

		// Token: 0x06001DAE RID: 7598 RVA: 0x000907D4 File Offset: 0x0008E9D4
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
			List<HurtBox> list = (from v in bullseyeSearch.GetResults()
			where !this.bouncedObjects.Contains(v.healthComponent)
			select v).ToList<HurtBox>();
			HurtBox hurtBox = (list.Count > 0) ? list[UnityEngine.Random.Range(0, list.Count)] : null;
			if (hurtBox)
			{
				this.bouncedObjects.Add(hurtBox.healthComponent);
			}
			return hurtBox;
		}

		// Token: 0x04001F8F RID: 8079
		private const float speed = 70f;

		// Token: 0x04001F90 RID: 8080
		public float damageValue;

		// Token: 0x04001F91 RID: 8081
		public GameObject attacker;

		// Token: 0x04001F92 RID: 8082
		public TeamIndex teamIndex;

		// Token: 0x04001F93 RID: 8083
		public List<HealthComponent> bouncedObjects;

		// Token: 0x04001F94 RID: 8084
		public bool isCrit;

		// Token: 0x04001F95 RID: 8085
		public float scale;

		// Token: 0x04001F96 RID: 8086
		public ProcChainMask procChainMask;

		// Token: 0x04001F97 RID: 8087
		public float procCoefficient = 0.2f;

		// Token: 0x04001F98 RID: 8088
		public DamageColorIndex damageColorIndex;
	}
}
