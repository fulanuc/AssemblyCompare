using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200052A RID: 1322
	public class OrbManager : MonoBehaviour
	{
		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06001DE4 RID: 7652 RVA: 0x00015EB5 File Offset: 0x000140B5
		// (set) Token: 0x06001DE5 RID: 7653 RVA: 0x00015EBC File Offset: 0x000140BC
		public static OrbManager instance { get; private set; }

		// Token: 0x06001DE6 RID: 7654 RVA: 0x00015EC4 File Offset: 0x000140C4
		private void OnEnable()
		{
			if (!OrbManager.instance)
			{
				OrbManager.instance = this;
				return;
			}
			Debug.LogErrorFormat(this, "Duplicate instance of singleton class {0}. Only one should exist at a time.", new object[]
			{
				base.GetType().Name
			});
		}

		// Token: 0x06001DE7 RID: 7655 RVA: 0x00015EF8 File Offset: 0x000140F8
		private void OnDisable()
		{
			if (OrbManager.instance == this)
			{
				OrbManager.instance = null;
			}
		}

		// Token: 0x06001DE8 RID: 7656 RVA: 0x000916FC File Offset: 0x0008F8FC
		private void FixedUpdate()
		{
			this.time += Time.fixedDeltaTime;
			for (int i = 0; i < this.orbsWithFixedUpdateBehavior.Count; i++)
			{
				this.orbsWithFixedUpdateBehavior[i].FixedUpdate();
			}
			if (this.nextOrbArrival <= this.time)
			{
				this.nextOrbArrival = float.PositiveInfinity;
				for (int j = this.travelingOrbs.Count - 1; j >= 0; j--)
				{
					Orb orb = this.travelingOrbs[j];
					if (orb.arrivalTime <= this.time)
					{
						this.travelingOrbs.RemoveAt(j);
						IOrbFixedUpdateBehavior orbFixedUpdateBehavior = orb as IOrbFixedUpdateBehavior;
						if (orbFixedUpdateBehavior != null)
						{
							this.orbsWithFixedUpdateBehavior.Remove(orbFixedUpdateBehavior);
						}
						orb.OnArrival();
					}
					else if (this.nextOrbArrival > orb.arrivalTime)
					{
						this.nextOrbArrival = orb.arrivalTime;
					}
				}
			}
		}

		// Token: 0x06001DE9 RID: 7657 RVA: 0x000917D8 File Offset: 0x0008F9D8
		public void AddOrb(Orb orb)
		{
			orb.Begin();
			orb.arrivalTime = this.time + orb.duration;
			this.travelingOrbs.Add(orb);
			IOrbFixedUpdateBehavior orbFixedUpdateBehavior = orb as IOrbFixedUpdateBehavior;
			if (orbFixedUpdateBehavior != null)
			{
				this.orbsWithFixedUpdateBehavior.Add(orbFixedUpdateBehavior);
			}
			if (this.nextOrbArrival > orb.arrivalTime)
			{
				this.nextOrbArrival = orb.arrivalTime;
			}
		}

		// Token: 0x04001FF7 RID: 8183
		private float time;

		// Token: 0x04001FF8 RID: 8184
		private List<Orb> travelingOrbs = new List<Orb>();

		// Token: 0x04001FF9 RID: 8185
		private float nextOrbArrival = float.PositiveInfinity;

		// Token: 0x04001FFA RID: 8186
		private List<IOrbFixedUpdateBehavior> orbsWithFixedUpdateBehavior = new List<IOrbFixedUpdateBehavior>();
	}
}
