using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200054A RID: 1354
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileImpactEventCaller : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E50 RID: 7760 RVA: 0x0001626F File Offset: 0x0001446F
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			ProjectileImpactEvent projectileImpactEvent = this.impactEvent;
			if (projectileImpactEvent == null)
			{
				return;
			}
			projectileImpactEvent.Invoke(impactInfo);
		}

		// Token: 0x040020BD RID: 8381
		public ProjectileImpactEvent impactEvent;
	}
}
