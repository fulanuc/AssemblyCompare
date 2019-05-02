using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000544 RID: 1348
	public class ProjectileFireChildren : MonoBehaviour
	{
		// Token: 0x06001E36 RID: 7734 RVA: 0x000160F9 File Offset: 0x000142F9
		private void Start()
		{
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.projectileController = base.GetComponent<ProjectileController>();
		}

		// Token: 0x06001E37 RID: 7735 RVA: 0x00094EA0 File Offset: 0x000930A0
		private void Update()
		{
			this.timer += Time.deltaTime;
			this.nextSpawnTimer += Time.deltaTime;
			if (this.timer >= this.duration)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (this.nextSpawnTimer >= this.duration / (float)this.count)
			{
				this.nextSpawnTimer -= this.duration / (float)this.count;
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.childProjectilePrefab, base.transform.position, Util.QuaternionSafeLookRotation(base.transform.forward));
				ProjectileController component = gameObject.GetComponent<ProjectileController>();
				if (component)
				{
					component.procChainMask = this.projectileController.procChainMask;
					component.procCoefficient = this.projectileController.procCoefficient * this.childProcCoefficient;
					component.Networkowner = this.projectileController.owner;
				}
				gameObject.GetComponent<TeamFilter>().teamIndex = base.GetComponent<TeamFilter>().teamIndex;
				ProjectileDamage component2 = gameObject.GetComponent<ProjectileDamage>();
				if (component2)
				{
					component2.damage = this.projectileDamage.damage * this.childDamageCoefficient;
					component2.crit = this.projectileDamage.crit;
					component2.force = this.projectileDamage.force;
					component2.damageColorIndex = this.projectileDamage.damageColorIndex;
				}
				if (!this.ignoreParentForChainController)
				{
					ChainController component3 = gameObject.GetComponent<ChainController>();
					if (component3)
					{
						component3.pastTargetList.Add(base.transform.parent);
					}
				}
				NetworkServer.Spawn(gameObject);
			}
		}

		// Token: 0x0400209B RID: 8347
		public float duration = 5f;

		// Token: 0x0400209C RID: 8348
		public int count = 5;

		// Token: 0x0400209D RID: 8349
		public GameObject childProjectilePrefab;

		// Token: 0x0400209E RID: 8350
		private float timer;

		// Token: 0x0400209F RID: 8351
		private float nextSpawnTimer;

		// Token: 0x040020A0 RID: 8352
		public float childDamageCoefficient = 1f;

		// Token: 0x040020A1 RID: 8353
		public float childProcCoefficient = 1f;

		// Token: 0x040020A2 RID: 8354
		private ProjectileDamage projectileDamage;

		// Token: 0x040020A3 RID: 8355
		private ProjectileController projectileController;

		// Token: 0x040020A4 RID: 8356
		public bool ignoreParentForChainController;
	}
}
