using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000550 RID: 1360
	[RequireComponent(typeof(ProjectileDamage))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileMageFirewallController : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E61 RID: 7777 RVA: 0x0001635D File Offset: 0x0001455D
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001E62 RID: 7778 RVA: 0x00016377 File Offset: 0x00014577
		void IProjectileImpactBehavior.OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (this.consumed)
			{
				return;
			}
			this.consumed = true;
			this.CreateWalkers();
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06001E63 RID: 7779 RVA: 0x00095C04 File Offset: 0x00093E04
		private void CreateWalkers()
		{
			Vector3 forward = base.transform.forward;
			forward.y = 0f;
			forward.Normalize();
			Vector3 vector = Vector3.Cross(Vector3.up, forward);
			ProjectileManager.instance.FireProjectile(this.walkerPrefab, base.transform.position + Vector3.up, Util.QuaternionSafeLookRotation(vector), this.projectileController.owner, this.projectileDamage.damage, this.projectileDamage.force, this.projectileDamage.crit, this.projectileDamage.damageColorIndex, null, -1f);
			ProjectileManager.instance.FireProjectile(this.walkerPrefab, base.transform.position + Vector3.up, Util.QuaternionSafeLookRotation(-vector), this.projectileController.owner, this.projectileDamage.damage, this.projectileDamage.force, this.projectileDamage.crit, this.projectileDamage.damageColorIndex, null, -1f);
		}

		// Token: 0x040020EA RID: 8426
		public GameObject walkerPrefab;

		// Token: 0x040020EB RID: 8427
		private ProjectileController projectileController;

		// Token: 0x040020EC RID: 8428
		private ProjectileDamage projectileDamage;

		// Token: 0x040020ED RID: 8429
		private bool consumed;
	}
}
