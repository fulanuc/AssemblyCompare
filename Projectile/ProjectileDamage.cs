using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200054F RID: 1359
	public class ProjectileDamage : MonoBehaviour
	{
		// Token: 0x040020B7 RID: 8375
		[HideInInspector]
		public float damage;

		// Token: 0x040020B8 RID: 8376
		[HideInInspector]
		public bool crit;

		// Token: 0x040020B9 RID: 8377
		[HideInInspector]
		public float force;

		// Token: 0x040020BA RID: 8378
		[HideInInspector]
		public DamageColorIndex damageColorIndex;

		// Token: 0x040020BB RID: 8379
		public DamageType damageType;
	}
}
