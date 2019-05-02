using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000550 RID: 1360
	[RequireComponent(typeof(TeamFilter))]
	[RequireComponent(typeof(ProjectileTargetComponent))]
	public class ProjectileDirectionalTargetFinder : MonoBehaviour
	{
		// Token: 0x06001E94 RID: 7828 RVA: 0x000957DC File Offset: 0x000939DC
		private void Start()
		{
			if (!NetworkServer.active)
			{
				base.enabled = false;
				return;
			}
			this.bullseyeSearch = new BullseyeSearch();
			this.teamFilter = base.GetComponent<TeamFilter>();
			this.targetComponent = base.GetComponent<ProjectileTargetComponent>();
			this.transform = base.transform;
			this.searchTimer = 0f;
		}

		// Token: 0x06001E95 RID: 7829 RVA: 0x00095834 File Offset: 0x00093A34
		private void FixedUpdate()
		{
			this.searchTimer -= Time.fixedDeltaTime;
			if (this.searchTimer <= 0f)
			{
				this.searchTimer += this.targetSearchInterval;
				if (!this.onlySearchIfNoTarget || !this.targetComponent.target)
				{
					this.SearchForTarget();
				}
			}
		}

		// Token: 0x06001E96 RID: 7830 RVA: 0x00095894 File Offset: 0x00093A94
		private void SearchForTarget()
		{
			this.bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			this.bullseyeSearch.teamMaskFilter.RemoveTeam(this.teamFilter.teamIndex);
			this.bullseyeSearch.filterByLoS = this.testLoS;
			this.bullseyeSearch.searchOrigin = this.transform.position;
			this.bullseyeSearch.searchDirection = this.transform.forward;
			this.bullseyeSearch.maxDistanceFilter = this.lookRange;
			this.bullseyeSearch.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
			this.bullseyeSearch.maxAngleFilter = this.lookCone;
			this.bullseyeSearch.RefreshCandidates();
			IEnumerable<HurtBox> source = this.bullseyeSearch.GetResults();
			if (this.ignoreAir)
			{
				source = from v in source
				where v.healthComponent.GetComponent<CharacterMotor>()
				select v;
			}
			ProjectileTargetComponent projectileTargetComponent = this.targetComponent;
			HurtBox hurtBox = source.FirstOrDefault<HurtBox>();
			projectileTargetComponent.target = ((hurtBox != null) ? hurtBox.transform : null);
		}

		// Token: 0x040020BC RID: 8380
		[Tooltip("How far ahead the projectile should look to find a target.")]
		public float lookRange;

		// Token: 0x040020BD RID: 8381
		[Tooltip("How wide the cone of vision for this projectile is in degrees. Limit is 180.")]
		[Range(0f, 180f)]
		public float lookCone;

		// Token: 0x040020BE RID: 8382
		[Tooltip("How long before searching for a target.")]
		public float targetSearchInterval = 0.5f;

		// Token: 0x040020BF RID: 8383
		[Tooltip("Will not search for new targets once it has one.")]
		public bool onlySearchIfNoTarget;

		// Token: 0x040020C0 RID: 8384
		[Tooltip("Allows the target to be lost if it's outside the acceptable range.")]
		public bool allowTargetLoss;

		// Token: 0x040020C1 RID: 8385
		[Tooltip("If set, targets can only be found when there is a free line of sight.")]
		public bool testLoS;

		// Token: 0x040020C2 RID: 8386
		[Tooltip("Whether or not airborne characters should be ignored.")]
		public bool ignoreAir;

		// Token: 0x040020C3 RID: 8387
		private new Transform transform;

		// Token: 0x040020C4 RID: 8388
		private TeamFilter teamFilter;

		// Token: 0x040020C5 RID: 8389
		private ProjectileTargetComponent targetComponent;

		// Token: 0x040020C6 RID: 8390
		private float searchTimer;

		// Token: 0x040020C7 RID: 8391
		private BullseyeSearch bullseyeSearch;
	}
}
