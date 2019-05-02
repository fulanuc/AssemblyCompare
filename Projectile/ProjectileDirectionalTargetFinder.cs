using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000541 RID: 1345
	[RequireComponent(typeof(TeamFilter))]
	[RequireComponent(typeof(ProjectileTargetComponent))]
	public class ProjectileDirectionalTargetFinder : MonoBehaviour
	{
		// Token: 0x06001E2A RID: 7722 RVA: 0x00094AC0 File Offset: 0x00092CC0
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

		// Token: 0x06001E2B RID: 7723 RVA: 0x00094B18 File Offset: 0x00092D18
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

		// Token: 0x06001E2C RID: 7724 RVA: 0x00094B78 File Offset: 0x00092D78
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

		// Token: 0x0400207E RID: 8318
		[Tooltip("How far ahead the projectile should look to find a target.")]
		public float lookRange;

		// Token: 0x0400207F RID: 8319
		[Range(0f, 180f)]
		[Tooltip("How wide the cone of vision for this projectile is in degrees. Limit is 180.")]
		public float lookCone;

		// Token: 0x04002080 RID: 8320
		[Tooltip("How long before searching for a target.")]
		public float targetSearchInterval = 0.5f;

		// Token: 0x04002081 RID: 8321
		[Tooltip("Will not search for new targets once it has one.")]
		public bool onlySearchIfNoTarget;

		// Token: 0x04002082 RID: 8322
		[Tooltip("Allows the target to be lost if it's outside the acceptable range.")]
		public bool allowTargetLoss;

		// Token: 0x04002083 RID: 8323
		[Tooltip("If set, targets can only be found when there is a free line of sight.")]
		public bool testLoS;

		// Token: 0x04002084 RID: 8324
		[Tooltip("Whether or not airborne characters should be ignored.")]
		public bool ignoreAir;

		// Token: 0x04002085 RID: 8325
		private new Transform transform;

		// Token: 0x04002086 RID: 8326
		private TeamFilter teamFilter;

		// Token: 0x04002087 RID: 8327
		private ProjectileTargetComponent targetComponent;

		// Token: 0x04002088 RID: 8328
		private float searchTimer;

		// Token: 0x04002089 RID: 8329
		private BullseyeSearch bullseyeSearch;
	}
}
