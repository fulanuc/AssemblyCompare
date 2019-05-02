using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000517 RID: 1303
	public class Orb
	{
		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06001D62 RID: 7522 RVA: 0x00015822 File Offset: 0x00013A22
		// (set) Token: 0x06001D63 RID: 7523 RVA: 0x0001582A File Offset: 0x00013A2A
		public float duration { get; protected set; }

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06001D64 RID: 7524 RVA: 0x00015833 File Offset: 0x00013A33
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

		// Token: 0x06001D65 RID: 7525 RVA: 0x000025F6 File Offset: 0x000007F6
		public virtual void Begin()
		{
		}

		// Token: 0x06001D66 RID: 7526 RVA: 0x000025F6 File Offset: 0x000007F6
		public virtual void OnArrival()
		{
		}

		// Token: 0x04001F98 RID: 8088
		public Vector3 origin;

		// Token: 0x04001F99 RID: 8089
		public HurtBox target;

		// Token: 0x04001F9B RID: 8091
		public float arrivalTime;

		// Token: 0x04001F9C RID: 8092
		public Orb nextOrb;
	}
}
