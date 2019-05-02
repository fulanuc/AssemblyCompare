using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200055B RID: 1371
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileProximityBeamController : MonoBehaviour
	{
		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06001EB0 RID: 7856 RVA: 0x000167C0 File Offset: 0x000149C0
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

		// Token: 0x06001EB1 RID: 7857 RVA: 0x00097034 File Offset: 0x00095234
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

		// Token: 0x06001EB2 RID: 7858 RVA: 0x000167DC File Offset: 0x000149DC
		private void ClearList()
		{
			this.previousTargets.Clear();
		}

		// Token: 0x06001EB3 RID: 7859 RVA: 0x000167E9 File Offset: 0x000149E9
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.UpdateServer();
				return;
			}
			base.enabled = false;
		}

		// Token: 0x06001EB4 RID: 7860 RVA: 0x00097098 File Offset: 0x00095298
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

		// Token: 0x06001EB5 RID: 7861 RVA: 0x000971CC File Offset: 0x000953CC
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

		// Token: 0x04002140 RID: 8512
		private ProjectileController projectileController;

		// Token: 0x04002141 RID: 8513
		private ProjectileDamage projectileDamage;

		// Token: 0x04002142 RID: 8514
		private List<HealthComponent> previousTargets;

		// Token: 0x04002143 RID: 8515
		private TeamFilter teamFilter;

		// Token: 0x04002144 RID: 8516
		public float attackInterval = 1f;

		// Token: 0x04002145 RID: 8517
		public float listClearInterval = 3f;

		// Token: 0x04002146 RID: 8518
		public float attackRange = 20f;

		// Token: 0x04002147 RID: 8519
		public float procCoefficient = 0.1f;

		// Token: 0x04002148 RID: 8520
		public float damageCoefficient = 1f;

		// Token: 0x04002149 RID: 8521
		public LightningOrb.LightningType lightningType = LightningOrb.LightningType.BFG;

		// Token: 0x0400214A RID: 8522
		private float attackTimer;

		// Token: 0x0400214B RID: 8523
		private float listClearTimer;

		// Token: 0x0400214C RID: 8524
		private readonly BullseyeSearch search = new BullseyeSearch();
	}
}
