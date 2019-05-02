using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200055F RID: 1375
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileMageFirewallController : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001ECB RID: 7883 RVA: 0x0001683C File Offset: 0x00014A3C
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001ECC RID: 7884 RVA: 0x00016856 File Offset: 0x00014A56
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

		// Token: 0x06001ECD RID: 7885 RVA: 0x00096920 File Offset: 0x00094B20
		private void CreateWalkers()
		{
			Vector3 forward = base.transform.forward;
			forward.y = 0f;
			forward.Normalize();
			Vector3 vector = Vector3.Cross(Vector3.up, forward);
			ProjectileManager.instance.FireProjectile(this.walkerPrefab, base.transform.position + Vector3.up, Util.QuaternionSafeLookRotation(vector), this.projectileController.owner, this.projectileDamage.damage, this.projectileDamage.force, this.projectileDamage.crit, this.projectileDamage.damageColorIndex, null, -1f);
			ProjectileManager.instance.FireProjectile(this.walkerPrefab, base.transform.position + Vector3.up, Util.QuaternionSafeLookRotation(-vector), this.projectileController.owner, this.projectileDamage.damage, this.projectileDamage.force, this.projectileDamage.crit, this.projectileDamage.damageColorIndex, null, -1f);
		}

		// Token: 0x04002128 RID: 8488
		public GameObject walkerPrefab;

		// Token: 0x04002129 RID: 8489
		private ProjectileController projectileController;

		// Token: 0x0400212A RID: 8490
		private ProjectileDamage projectileDamage;

		// Token: 0x0400212B RID: 8491
		private bool consumed;
	}
}
