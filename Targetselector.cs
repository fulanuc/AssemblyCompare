using System;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020006E9 RID: 1769
	[RequireComponent(typeof(TeamComponent))]
	public class Targetselector : MonoBehaviour
	{
		// Token: 0x0600278E RID: 10126 RVA: 0x000B8474 File Offset: 0x000B6674
		private void SearchForTarget(Ray aimRay)
		{
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			this.TeamComp = base.GetComponent<TeamComponent>();
			bullseyeSearch.teamMaskFilter = TeamMask.all;
			bullseyeSearch.teamMaskFilter.RemoveTeam(this.TeamComp.teamIndex);
			bullseyeSearch.filterByLoS = false;
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = aimRay.direction;
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.maxDistanceFilter = 1000000f;
			bullseyeSearch.maxAngleFilter = 1E+08f;
			bullseyeSearch.RefreshCandidates();
			this.trackingTarget = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
		}

		// Token: 0x0600278F RID: 10127 RVA: 0x000B850C File Offset: 0x000B670C
		public HurtBox SearchForTarget1(Ray aimRay)
		{
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			this.TeamComp = base.GetComponent<TeamComponent>();
			bullseyeSearch.teamMaskFilter = TeamMask.all;
			bullseyeSearch.teamMaskFilter.RemoveTeam(this.TeamComp.teamIndex);
			bullseyeSearch.filterByLoS = false;
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = aimRay.direction;
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.maxDistanceFilter = 1000000f;
			bullseyeSearch.maxAngleFilter = 1E+08f;
			bullseyeSearch.RefreshCandidates();
			this.trackingTarget = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
			return this.trackingTarget;
		}

		// Token: 0x040029AD RID: 10669
		public TeamComponent TeamComp;

		// Token: 0x040029AE RID: 10670
		public HurtBox trackingTarget;
	}
}
