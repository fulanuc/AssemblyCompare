using System;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200031B RID: 795
	[RequireComponent(typeof(TeamComponent))]
	[RequireComponent(typeof(InputBankTest))]
	[RequireComponent(typeof(CharacterBody))]
	public class HuntressTracker : MonoBehaviour
	{
		// Token: 0x06001079 RID: 4217 RVA: 0x0000C9C0 File Offset: 0x0000ABC0
		private void Awake()
		{
			this.indicator = new Indicator(base.gameObject, Resources.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
		}

		// Token: 0x0600107A RID: 4218 RVA: 0x0000C9DD File Offset: 0x0000ABDD
		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.inputBank = base.GetComponent<InputBankTest>();
			this.teamComponent = base.GetComponent<TeamComponent>();
		}

		// Token: 0x0600107B RID: 4219 RVA: 0x0000CA03 File Offset: 0x0000AC03
		public HurtBox GetTrackingTarget()
		{
			return this.trackingTarget;
		}

		// Token: 0x0600107C RID: 4220 RVA: 0x0000CA0B File Offset: 0x0000AC0B
		private void OnEnable()
		{
			this.indicator.active = true;
		}

		// Token: 0x0600107D RID: 4221 RVA: 0x0000CA19 File Offset: 0x0000AC19
		private void OnDisable()
		{
			this.indicator.active = false;
		}

		// Token: 0x0600107E RID: 4222 RVA: 0x00062B08 File Offset: 0x00060D08
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

		// Token: 0x0600107F RID: 4223 RVA: 0x00062BA8 File Offset: 0x00060DA8
		private void SearchForTarget(Ray aimRay)
		{
			this.search.teamMaskFilter = TeamMask.all;
			this.search.teamMaskFilter.RemoveTeam(this.teamComponent.teamIndex);
			this.search.filterByLoS = true;
			this.search.searchOrigin = aimRay.origin;
			this.search.searchDirection = aimRay.direction;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.maxDistanceFilter = this.maxTrackingDistance;
			this.search.maxAngleFilter = this.maxTrackingAngle;
			this.search.RefreshCandidates();
			this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
		}

		// Token: 0x04001485 RID: 5253
		public float maxTrackingDistance = 20f;

		// Token: 0x04001486 RID: 5254
		public float maxTrackingAngle = 20f;

		// Token: 0x04001487 RID: 5255
		public float trackerUpdateFrequency = 10f;

		// Token: 0x04001488 RID: 5256
		private HurtBox trackingTarget;

		// Token: 0x04001489 RID: 5257
		private CharacterBody characterBody;

		// Token: 0x0400148A RID: 5258
		private TeamComponent teamComponent;

		// Token: 0x0400148B RID: 5259
		private InputBankTest inputBank;

		// Token: 0x0400148C RID: 5260
		private float trackerUpdateStopwatch;

		// Token: 0x0400148D RID: 5261
		private Indicator indicator;

		// Token: 0x0400148E RID: 5262
		private readonly BullseyeSearch search = new BullseyeSearch();
	}
}
