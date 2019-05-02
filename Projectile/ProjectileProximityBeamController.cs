using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200056A RID: 1386
	[RequireComponent(typeof(ProjectileDamage))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileProximityBeamController : MonoBehaviour
	{
		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06001F1A RID: 7962 RVA: 0x00016C9F File Offset: 0x00014E9F
		private TeamIndex myTeamIndex
		{
			get
			{
				if (!this.teamFilter)
				{
					return TeamIndex.Neutral;
				}
				return this.teamFilter.teamIndex;
			}
		}

		// Token: 0x06001F1B RID: 7963 RVA: 0x00097D50 File Offset: 0x00095F50
		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.projectileController = base.GetComponent<ProjectileController>();
				ProjectileController projectileController = this.projectileController;
				this.teamFilter = ((projectileController != null) ? projectileController.teamFilter : null);
				this.projectileDamage = base.GetComponent<ProjectileDamage>();
				this.attackTimer = 0f;
				this.previousTargets = new List<HealthComponent>();
				return;
			}
			base.enabled = false;
		}

		// Token: 0x06001F1C RID: 7964 RVA: 0x00016CBB File Offset: 0x00014EBB
		private void ClearList()
		{
			this.previousTargets.Clear();
		}

		// Token: 0x06001F1D RID: 7965 RVA: 0x00016CC8 File Offset: 0x00014EC8
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.UpdateServer();
				return;
			}
			base.enabled = false;
		}

		// Token: 0x06001F1E RID: 7966 RVA: 0x00097DB4 File Offset: 0x00095FB4
		private void UpdateServer()
		{
			this.listClearTimer -= Time.fixedDeltaTime;
			if (this.listClearTimer <= 0f)
			{
				this.ClearList();
				this.listClearTimer = this.listClearInterval;
			}
			this.attackTimer -= Time.fixedDeltaTime;
			if (this.attackTimer <= 0f)
			{
				this.attackTimer = this.attackInterval;
				Vector3 position = base.transform.position;
				HurtBox hurtBox = this.FindNextTarget(position);
				if (hurtBox)
				{
					this.previousTargets.Add(hurtBox.healthComponent);
					LightningOrb lightningOrb = new LightningOrb();
					lightningOrb.attacker = this.projectileController.owner;
					lightningOrb.teamIndex = this.myTeamIndex;
					lightningOrb.damageValue = this.projectileDamage.damage * this.damageCoefficient;
					lightningOrb.isCrit = this.projectileDamage.crit;
					lightningOrb.origin = position;
					lightningOrb.bouncesRemaining = 0;
					lightningOrb.lightningType = this.lightningType;
					lightningOrb.procCoefficient = this.procCoefficient;
					lightningOrb.target = hurtBox;
					lightningOrb.damageColorIndex = this.projectileDamage.damageColorIndex;
					OrbManager.instance.AddOrb(lightningOrb);
				}
			}
		}

		// Token: 0x06001F1F RID: 7967 RVA: 0x00097EE8 File Offset: 0x000960E8
		public HurtBox FindNextTarget(Vector3 position)
		{
			this.search.searchOrigin = position;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.teamMaskFilter = TeamMask.allButNeutral;
			this.search.teamMaskFilter.RemoveTeam(this.myTeamIndex);
			this.search.filterByLoS = false;
			this.search.maxDistanceFilter = this.attackRange;
			this.search.RefreshCandidates();
			return this.search.GetResults().FirstOrDefault((HurtBox hurtBox) => !this.previousTargets.Contains(hurtBox.healthComponent));
		}

		// Token: 0x0400217E RID: 8574
		private ProjectileController projectileController;

		// Token: 0x0400217F RID: 8575
		private ProjectileDamage projectileDamage;

		// Token: 0x04002180 RID: 8576
		private List<HealthComponent> previousTargets;

		// Token: 0x04002181 RID: 8577
		private TeamFilter teamFilter;

		// Token: 0x04002182 RID: 8578
		public float attackInterval = 1f;

		// Token: 0x04002183 RID: 8579
		public float listClearInterval = 3f;

		// Token: 0x04002184 RID: 8580
		public float attackRange = 20f;

		// Token: 0x04002185 RID: 8581
		public float procCoefficient = 0.1f;

		// Token: 0x04002186 RID: 8582
		public float damageCoefficient = 1f;

		// Token: 0x04002187 RID: 8583
		public LightningOrb.LightningType lightningType = LightningOrb.LightningType.BFG;

		// Token: 0x04002188 RID: 8584
		private float attackTimer;

		// Token: 0x04002189 RID: 8585
		private float listClearTimer;

		// Token: 0x0400218A RID: 8586
		private readonly BullseyeSearch search = new BullseyeSearch();
	}
}
