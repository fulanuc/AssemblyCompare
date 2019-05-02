using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000539 RID: 1337
	public struct ProjectileImpactInfo
	{
		// Token: 0x0400204B RID: 8267
		public Collider collider;

		// Token: 0x0400204C RID: 8268
		public Vector3 estimatedPointOfImpact;

		// Token: 0x0400204D RID: 8269
		public Vector3 estimatedImpactNormal;
	}
}
