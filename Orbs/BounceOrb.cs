using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200050C RID: 1292
	public class BounceOrb : Orb
	{
		// Token: 0x06001D44 RID: 7492 RVA: 0x0008F898 File Offset: 0x0008DA98
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

		// Token: 0x06001D45 RID: 7493 RVA: 0x0008F904 File Offset: 0x0008DB04
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

		// Token: 0x06001D46 RID: 7494 RVA: 0x0008F9FC File Offset: 0x0008DBFC
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

		// Token: 0x04001F51 RID: 8017
		private const float speed = 70f;

		// Token: 0x04001F52 RID: 8018
		public float damageValue;

		// Token: 0x04001F53 RID: 8019
		public GameObject attacker;

		// Token: 0x04001F54 RID: 8020
		public TeamIndex teamIndex;

		// Token: 0x04001F55 RID: 8021
		public List<HealthComponent> bouncedObjects;

		// Token: 0x04001F56 RID: 8022
		public bool isCrit;

		// Token: 0x04001F57 RID: 8023
		public float scale;

		// Token: 0x04001F58 RID: 8024
		public ProcChainMask procChainMask;

		// Token: 0x04001F59 RID: 8025
		public float procCoefficient = 0.2f;

		// Token: 0x04001F5A RID: 8026
		public DamageColorIndex damageColorIndex;
	}
}
