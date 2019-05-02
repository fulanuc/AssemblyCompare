using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200031D RID: 797
	public class BullseyeSearch
	{
		// Token: 0x1700016A RID: 362
		// (set) Token: 0x0600107A RID: 4218 RVA: 0x0000CA40 File Offset: 0x0000AC40
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

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x0600107B RID: 4219 RVA: 0x0000CA68 File Offset: 0x0000AC68
		private bool filterByDistance
		{
			get
			{
				return this.minDistanceFilter > 0f || this.maxDistanceFilter < float.PositiveInfinity;
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x0600107C RID: 4220 RVA: 0x0000CA86 File Offset: 0x0000AC86
		private bool filterByAngle
		{
			get
			{
				return this.minThetaDot > BullseyeSearch.fullVisionMinThetaDot;
			}
		}

		// Token: 0x0600107D RID: 4221 RVA: 0x00062A48 File Offset: 0x00060C48
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

		// Token: 0x0600107E RID: 4222 RVA: 0x00062AFC File Offset: 0x00060CFC
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

		// Token: 0x0600107F RID: 4223 RVA: 0x00062BC4 File Offset: 0x00060DC4
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

		// Token: 0x06001080 RID: 4224 RVA: 0x00062C50 File Offset: 0x00060E50
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

		// Token: 0x06001081 RID: 4225 RVA: 0x00062CF4 File Offset: 0x00060EF4
		public void FilterOutGameObject(GameObject gameObject)
		{
			this.candidatesEnumerable = from v in this.candidatesEnumerable
			where v.hurtBox.healthComponent.gameObject != gameObject
			select v;
		}

		// Token: 0x06001082 RID: 4226 RVA: 0x00062D2C File Offset: 0x00060F2C
		public IEnumerable<HurtBox> GetResults()
		{
			IEnumerable<BullseyeSearch.CandidateInfo> source = this.candidatesEnumerable;
			if (this.filterByLoS)
			{
				source = from candidateInfo in this.candidatesEnumerable
				where this.CheckLoS(candidateInfo.position)
				select candidateInfo;
			}
			if (this.viewer)
			{
				source = from candidateInfo in this.candidatesEnumerable
				where this.CheckVisisble(candidateInfo.hurtBox.healthComponent.gameObject)
				select candidateInfo;
			}
			return from candidateInfo in source
			select candidateInfo.hurtBox;
		}

		// Token: 0x06001083 RID: 4227 RVA: 0x00062DAC File Offset: 0x00060FAC
		private bool CheckLoS(Vector3 targetPosition)
		{
			Vector3 direction = targetPosition - this.searchOrigin;
			RaycastHit raycastHit;
			return !Physics.Raycast(this.searchOrigin, direction, out raycastHit, direction.magnitude, LayerIndex.world.mask, this.queryTriggerInteraction);
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x00062DF8 File Offset: 0x00060FF8
		private bool CheckVisisble(GameObject gameObject)
		{
			CharacterBody component = gameObject.GetComponent<CharacterBody>();
			return !component || component.GetVisibilityLevel(this.viewer) >= VisibilityLevel.Revealed;
		}

		// Token: 0x0400148B RID: 5259
		public CharacterBody viewer;

		// Token: 0x0400148C RID: 5260
		public Vector3 searchOrigin;

		// Token: 0x0400148D RID: 5261
		public Vector3 searchDirection;

		// Token: 0x0400148E RID: 5262
		private static readonly float fullVisionMinThetaDot = Mathf.Cos(3.14159274f);

		// Token: 0x0400148F RID: 5263
		private float minThetaDot = -1f;

		// Token: 0x04001490 RID: 5264
		public float minDistanceFilter;

		// Token: 0x04001491 RID: 5265
		public float maxDistanceFilter = float.PositiveInfinity;

		// Token: 0x04001492 RID: 5266
		public TeamMask teamMaskFilter = TeamMask.allButNeutral;

		// Token: 0x04001493 RID: 5267
		public bool filterByLoS = true;

		// Token: 0x04001494 RID: 5268
		public QueryTriggerInteraction queryTriggerInteraction;

		// Token: 0x04001495 RID: 5269
		public BullseyeSearch.SortMode sortMode = BullseyeSearch.SortMode.Distance;

		// Token: 0x04001496 RID: 5270
		private IEnumerable<BullseyeSearch.CandidateInfo> candidatesEnumerable;

		// Token: 0x0200031E RID: 798
		private struct CandidateInfo
		{
			// Token: 0x04001497 RID: 5271
			public HurtBox hurtBox;

			// Token: 0x04001498 RID: 5272
			public Vector3 position;

			// Token: 0x04001499 RID: 5273
			public float dot;

			// Token: 0x0400149A RID: 5274
			public float distanceSqr;
		}

		// Token: 0x0200031F RID: 799
		public enum SortMode
		{
			// Token: 0x0400149C RID: 5276
			None,
			// Token: 0x0400149D RID: 5277
			Distance,
			// Token: 0x0400149E RID: 5278
			Angle,
			// Token: 0x0400149F RID: 5279
			DistanceAndAngle
		}

		// Token: 0x02000320 RID: 800
		// (Invoke) Token: 0x0600108C RID: 4236
		private delegate BullseyeSearch.CandidateInfo Selector(HurtBox hurtBox);
	}
}
