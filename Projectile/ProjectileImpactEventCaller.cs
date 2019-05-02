using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000559 RID: 1369
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileImpactEventCaller : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001EBA RID: 7866 RVA: 0x0001674E File Offset: 0x0001494E
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			ProjectileImpactEvent projectileImpactEvent = this.impactEvent;
			if (projectileImpactEvent == null)
			{
				return;
			}
			projectileImpactEvent.Invoke(impactInfo);
		}

		// Token: 0x040020FB RID: 8443
		public ProjectileImpactEvent impactEvent;
	}
}
