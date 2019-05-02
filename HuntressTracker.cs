using System;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000319 RID: 793
	[RequireComponent(typeof(TeamComponent))]
	[RequireComponent(typeof(CharacterBody))]
	[RequireComponent(typeof(InputBankTest))]
	public class HuntressTracker : MonoBehaviour
	{
		// Token: 0x06001065 RID: 4197 RVA: 0x0000C8DC File Offset: 0x0000AADC
		private void Awake()
		{
			this.indicator = new Indicator(base.gameObject, Resources.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
		}

		// Token: 0x06001066 RID: 4198 RVA: 0x0000C8F9 File Offset: 0x0000AAF9
		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.inputBank = base.GetComponent<InputBankTest>();
			this.teamComponent = base.GetComponent<TeamComponent>();
		}

		// Token: 0x06001067 RID: 4199 RVA: 0x0000C91F File Offset: 0x0000AB1F
		public HurtBox GetTrackingTarget()
		{
			return this.trackingTarget;
		}

		// Token: 0x06001068 RID: 4200 RVA: 0x0000C927 File Offset: 0x0000AB27
		private void OnEnable()
		{
			this.indicator.active = true;
		}

		// Token: 0x06001069 RID: 4201 RVA: 0x0000C935 File Offset: 0x0000AB35
		private void OnDisable()
		{
			this.indicator.active = false;
		}

		// Token: 0x0600106A RID: 4202 RVA: 0x00062864 File Offset: 0x00060A64
		private void FixedUpdate()
		{
			this.trackerUpdateStopwatch += Time.fixedDeltaTime;
			if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency)
			{
				this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;
				HurtBox hurtBox = this.trackingTarget;
				Ray aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
				this.SearchForTarget(aimRay);
				this.indicator.targetTransform = (this.trackingTarget ? this.trackingTarget.transform : null);
			}
		}

		// Token: 0x0600106B RID: 4203 RVA: 0x00062904 File Offset: 0x00060B04
		private void SearchForTarget(Ray aimRay)
		{
			this.search.teamMaskFilter = TeamMask.all;
			this.search.teamMaskFilter.RemoveTeam(this.teamComponent.teamIndex);
			this.search.filterByLoS = false;
			this.search.searchOrigin = aimRay.origin;
			this.search.searchDirection = aimRay.direction;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.maxDistanceFilter = this.maxTrackingDistance + 1000000f;
			this.search.maxAngleFilter = this.maxTrackingAngle + 1E+08f;
			this.search.RefreshCandidates();
			this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
		}

		// Token: 0x04001471 RID: 5233
		public float maxTrackingDistance = 20f;

		// Token: 0x04001472 RID: 5234
		public float maxTrackingAngle = 20f;

		// Token: 0x04001473 RID: 5235
		public float trackerUpdateFrequency = 10f;

		// Token: 0x04001474 RID: 5236
		public HurtBox trackingTarget;

		// Token: 0x04001475 RID: 5237
		private CharacterBody characterBody;

		// Token: 0x04001476 RID: 5238
		private TeamComponent teamComponent;

		// Token: 0x04001477 RID: 5239
		private InputBankTest inputBank;

		// Token: 0x04001478 RID: 5240
		private float trackerUpdateStopwatch;

		// Token: 0x04001479 RID: 5241
		private Indicator indicator;

		// Token: 0x0400147A RID: 5242
		private readonly BullseyeSearch search = new BullseyeSearch();
	}
}
