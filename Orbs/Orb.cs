using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000526 RID: 1318
	public class Orb
	{
		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06001DCA RID: 7626 RVA: 0x00015CEB File Offset: 0x00013EEB
		// (set) Token: 0x06001DCB RID: 7627 RVA: 0x00015CF3 File Offset: 0x00013EF3
		public float duration { get; protected set; }

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06001DCC RID: 7628 RVA: 0x00015CFC File Offset: 0x00013EFC
		protected float distanceToTarget
		{
			get
			{
				if (this.target)
				{
					return Vector3.Distance(this.target.transform.position, this.origin);
				}
				return 0f;
			}
		}

		// Token: 0x06001DCD RID: 7629 RVA: 0x000025DA File Offset: 0x000007DA
		public virtual void Begin()
		{
		}

		// Token: 0x06001DCE RID: 7630 RVA: 0x000025DA File Offset: 0x000007DA
		public virtual void OnArrival()
		{
		}

		// Token: 0x04001FD6 RID: 8150
		public Vector3 origin;

		// Token: 0x04001FD7 RID: 8151
		public HurtBox target;

		// Token: 0x04001FD9 RID: 8153
		public float arrivalTime;

		// Token: 0x04001FDA RID: 8154
		public Orb nextOrb;
	}
}
