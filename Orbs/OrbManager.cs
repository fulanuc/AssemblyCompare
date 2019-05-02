using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200051B RID: 1307
	public class OrbManager : MonoBehaviour
	{
		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06001D7C RID: 7548 RVA: 0x000159EC File Offset: 0x00013BEC
		// (set) Token: 0x06001D7D RID: 7549 RVA: 0x000159F3 File Offset: 0x00013BF3
		public static OrbManager instance { get; private set; }

		// Token: 0x06001D7E RID: 7550 RVA: 0x000159FB File Offset: 0x00013BFB
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

		// Token: 0x06001D7F RID: 7551 RVA: 0x00015A2F File Offset: 0x00013C2F
		private void OnDisable()
		{
			if (OrbManager.instance == this)
			{
				OrbManager.instance = null;
			}
		}

		// Token: 0x06001D80 RID: 7552 RVA: 0x00090988 File Offset: 0x0008EB88
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

		// Token: 0x06001D81 RID: 7553 RVA: 0x00090A64 File Offset: 0x0008EC64
		public void AddOrb(Orb orb)
		{
			orb.Begin();
			orb.arrivalTime = this.time;
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

		// Token: 0x04001FB9 RID: 8121
		private float time;

		// Token: 0x04001FBA RID: 8122
		private List<Orb> travelingOrbs = new List<Orb>();

		// Token: 0x04001FBB RID: 8123
		private float nextOrbArrival = float.PositiveInfinity;

		// Token: 0x04001FBC RID: 8124
		private List<IOrbFixedUpdateBehavior> orbsWithFixedUpdateBehavior = new List<IOrbFixedUpdateBehavior>();
	}
}
