using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000548 RID: 1352
	public struct ProjectileImpactInfo
	{
		// Token: 0x04002089 RID: 8329
		public Collider collider;

		// Token: 0x0400208A RID: 8330
		public Vector3 estimatedPointOfImpact;

		// Token: 0x0400208B RID: 8331
		public Vector3 estimatedImpactNormal;
	}
}
