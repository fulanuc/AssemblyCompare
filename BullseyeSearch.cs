using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200031F RID: 799
	public class BullseyeSearch
	{
		// Token: 0x1700016F RID: 367
		// (set) Token: 0x0600108E RID: 4238 RVA: 0x0000CB24 File Offset: 0x0000AD24
		public float maxAngleFilter
		{
			set
			{
				if (value >= 180f)
				{
					this.minThetaDot = BullseyeSearch.fullVisionMinThetaDot;
					return;
				}
				this.minThetaDot = Mathf.Cos(value * 0.0174532924f);
			}
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x0600108F RID: 4239 RVA: 0x0000CB4C File Offset: 0x0000AD4C
		private bool filterByDistance
		{
			get
			{
				return this.minDistanceFilter > 0f || this.maxDistanceFilter < float.PositiveInfinity;
			}
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06001090 RID: 4240 RVA: 0x0000CB6A File Offset: 0x0000AD6A
		private bool filterByAngle
		{
			get
			{
				return this.minThetaDot > BullseyeSearch.fullVisionMinThetaDot;
			}
		}

		// Token: 0x06001091 RID: 4241 RVA: 0x00062CE0 File Offset: 0x00060EE0
		private Func<HurtBox, BullseyeSearch.CandidateInfo> GetSelector()
		{
			bool getDot = this.filterByAngle;
			bool getDistanceSqr = this.filterByDistance;
			getDistanceSqr |= (this.sortMode == BullseyeSearch.SortMode.Distance || this.sortMode == BullseyeSearch.SortMode.DistanceAndAngle);
			getDot |= (this.sortMode == BullseyeSearch.SortMode.Angle || this.sortMode == BullseyeSearch.SortMode.DistanceAndAngle);
			bool getDifference = getDot | getDistanceSqr;
			bool getPosition = (getDot | getDistanceSqr) || this.filterByLoS;
			return delegate(HurtBox hurtBox)
			{
				BullseyeSearch.CandidateInfo candidateInfo = new BullseyeSearch.CandidateInfo
				{
					hurtBox = hurtBox
				};
				if (getPosition)
				{
					candidateInfo.position = hurtBox.transform.position;
				}
				Vector3 vector = default(Vector3);
				if (getDifference)
				{
					vector = candidateInfo.position - this.searchOrigin;
				}
				if (getDot)
				{
					candidateInfo.dot = Vector3.Dot(this.searchDirection, vector.normalized);
				}
				if (getDistanceSqr)
				{
					candidateInfo.distanceSqr = vector.sqrMagnitude;
				}
				return candidateInfo;
			};
		}

		// Token: 0x06001092 RID: 4242 RVA: 0x00062D94 File Offset: 0x00060F94
		public void RefreshCandidates()
		{
			Func<HurtBox, BullseyeSearch.CandidateInfo> selector = this.GetSelector();
			this.candidatesEnumerable = (from hurtBox in HurtBox.readOnlyBullseyesList
			where this.teamMaskFilter.HasTeam(hurtBox.teamIndex)
			select hurtBox).Select(selector);
			if (this.filterByAngle)
			{
				this.candidatesEnumerable = from candidateInfo in this.candidatesEnumerable
				where candidateInfo.dot >= this.minThetaDot
				select candidateInfo;
			}
			if (this.filterByDistance)
			{
				float minDistanceSqr = this.minDistanceFilter * this.minDistanceFilter;
				float maxDistanceSqr = this.maxDistanceFilter * this.maxDistanceFilter;
				this.candidatesEnumerable = from candidateInfo in this.candidatesEnumerable
				where candidateInfo.distanceSqr >= minDistanceSqr && candidateInfo.distanceSqr <= maxDistanceSqr
				select candidateInfo;
			}
			Func<BullseyeSearch.CandidateInfo, float> sorter = this.GetSorter();
			this.candidatesEnumerable = this.candidatesEnumerable.OrderBy(sorter);
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x00062E5C File Offset: 0x0006105C
		private Func<BullseyeSearch.CandidateInfo, float> GetSorter()
		{
			switch (this.sortMode)
			{
			case BullseyeSearch.SortMode.Distance:
				return (BullseyeSearch.CandidateInfo candidateInfo) => candidateInfo.distanceSqr;
			case BullseyeSearch.SortMode.Angle:
				return (BullseyeSearch.CandidateInfo candidateInfo) => -candidateInfo.dot;
			case BullseyeSearch.SortMode.DistanceAndAngle:
				return (BullseyeSearch.CandidateInfo candidateInfo) => -candidateInfo.dot * candidateInfo.distanceSqr;
			default:
				return null;
			}
		}

		// Token: 0x06001094 RID: 4244 RVA: 0x00062EE8 File Offset: 0x000610E8
		public void FilterCandidatesByHealthFraction(float minHealthFraction = 0f, float maxHealthFraction = 1f)
		{
			if (minHealthFraction > 0f)
			{
				if (maxHealthFraction < 1f)
				{
					this.candidatesEnumerable = this.candidatesEnumerable.Where(delegate(BullseyeSearch.CandidateInfo v)
					{
						float combinedHealthFraction = v.hurtBox.healthComponent.combinedHealthFraction;
						return combinedHealthFraction >= minHealthFraction && combinedHealthFraction <= maxHealthFraction;
					});
					return;
				}
				this.candidatesEnumerable = from v in this.candidatesEnumerable
				where v.hurtBox.healthComponent.combinedHealthFraction >= minHealthFraction
				select v;
				return;
			}
			else
			{
				if (maxHealthFraction < 1f)
				{
					this.candidatesEnumerable = from v in this.candidatesEnumerable
					where v.hurtBox.healthComponent.combinedHealthFraction <= maxHealthFraction
					select v;
					return;
				}
				return;
			}
		}

		// Token: 0x06001095 RID: 4245 RVA: 0x00062F8C File Offset: 0x0006118C
		public void FilterOutGameObject(GameObject gameObject)
		{
			this.candidatesEnumerable = from v in this.candidatesEnumerable
			where v.hurtBox.healthComponent.gameObject != gameObject
			select v;
		}

		// Token: 0x06001096 RID: 4246 RVA: 0x00062FC4 File Offset: 0x000611C4
		public IEnumerable<HurtBox> GetResults()
		{
			IEnumerable<BullseyeSearch.CandidateInfo> source = this.candidatesEnumerable;
			if (this.filterByLoS)
			{
				source = from candidateInfo in source
				where this.CheckLoS(candidateInfo.position)
				select candidateInfo;
			}
			if (this.viewer)
			{
				source = from candidateInfo in source
				where this.CheckVisisble(candidateInfo.hurtBox.healthComponent.gameObject)
				select candidateInfo;
			}
			return from candidateInfo in source
			select candidateInfo.hurtBox;
		}

		// Token: 0x06001097 RID: 4247 RVA: 0x00063038 File Offset: 0x00061238
		private bool CheckLoS(Vector3 targetPosition)
		{
			Vector3 direction = targetPosition - this.searchOrigin;
			RaycastHit raycastHit;
			return !Physics.Raycast(this.searchOrigin, direction, out raycastHit, direction.magnitude, LayerIndex.world.mask, this.queryTriggerInteraction);
		}

		// Token: 0x06001098 RID: 4248 RVA: 0x00063084 File Offset: 0x00061284
		private bool CheckVisisble(GameObject gameObject)
		{
			CharacterBody component = gameObject.GetComponent<CharacterBody>();
			return !component || component.GetVisibilityLevel(this.viewer) >= VisibilityLevel.Revealed;
		}

		// Token: 0x0400149F RID: 5279
		public CharacterBody viewer;

		// Token: 0x040014A0 RID: 5280
		public Vector3 searchOrigin;

		// Token: 0x040014A1 RID: 5281
		public Vector3 searchDirection;

		// Token: 0x040014A2 RID: 5282
		private static readonly float fullVisionMinThetaDot = Mathf.Cos(3.14159274f);

		// Token: 0x040014A3 RID: 5283
		private float minThetaDot = -1f;

		// Token: 0x040014A4 RID: 5284
		public float minDistanceFilter;

		// Token: 0x040014A5 RID: 5285
		public float maxDistanceFilter = float.PositiveInfinity;

		// Token: 0x040014A6 RID: 5286
		public TeamMask teamMaskFilter = TeamMask.allButNeutral;

		// Token: 0x040014A7 RID: 5287
		public bool filterByLoS = true;

		// Token: 0x040014A8 RID: 5288
		public QueryTriggerInteraction queryTriggerInteraction;

		// Token: 0x040014A9 RID: 5289
		public BullseyeSearch.SortMode sortMode = BullseyeSearch.SortMode.Distance;

		// Token: 0x040014AA RID: 5290
		private IEnumerable<BullseyeSearch.CandidateInfo> candidatesEnumerable;

		// Token: 0x02000320 RID: 800
		private struct CandidateInfo
		{
			// Token: 0x040014AB RID: 5291
			public HurtBox hurtBox;

			// Token: 0x040014AC RID: 5292
			public Vector3 position;

			// Token: 0x040014AD RID: 5293
			public float dot;

			// Token: 0x040014AE RID: 5294
			public float distanceSqr;
		}

		// Token: 0x02000321 RID: 801
		public enum SortMode
		{
			// Token: 0x040014B0 RID: 5296
			None,
			// Token: 0x040014B1 RID: 5297
			Distance,
			// Token: 0x040014B2 RID: 5298
			Angle,
			// Token: 0x040014B3 RID: 5299
			DistanceAndAngle
		}

		// Token: 0x02000322 RID: 802
		// (Invoke) Token: 0x060010A0 RID: 4256
		private delegate BullseyeSearch.CandidateInfo Selector(HurtBox hurtBox);
	}
}
