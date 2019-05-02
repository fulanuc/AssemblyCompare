using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000540 RID: 1344
	public class ProjectileDamage : MonoBehaviour
	{
		// Token: 0x04002079 RID: 8313
		[HideInInspector]
		public float damage;

		// Token: 0x0400207A RID: 8314
		[HideInInspector]
		public bool crit;

		// Token: 0x0400207B RID: 8315
		[HideInInspector]
		public float force;

		// Token: 0x0400207C RID: 8316
		[HideInInspector]
		public DamageColorIndex damageColorIndex;

		// Token: 0x0400207D RID: 8317
		public DamageType damageType;
	}
}
