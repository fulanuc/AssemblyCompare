using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000553 RID: 1363
	public class ProjectileFireChildren : MonoBehaviour
	{
		// Token: 0x06001EA0 RID: 7840 RVA: 0x000165D8 File Offset: 0x000147D8
		private void Start()
		{
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.projectileController = base.GetComponent<ProjectileController>();
		}

		// Token: 0x06001EA1 RID: 7841 RVA: 0x00095BBC File Offset: 0x00093DBC
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

		// Token: 0x040020D9 RID: 8409
		public float duration = 5f;

		// Token: 0x040020DA RID: 8410
		public int count = 5;

		// Token: 0x040020DB RID: 8411
		public GameObject childProjectilePrefab;

		// Token: 0x040020DC RID: 8412
		private float timer;

		// Token: 0x040020DD RID: 8413
		private float nextSpawnTimer;

		// Token: 0x040020DE RID: 8414
		public float childDamageCoefficient = 1f;

		// Token: 0x040020DF RID: 8415
		public float childProcCoefficient = 1f;

		// Token: 0x040020E0 RID: 8416
		private ProjectileDamage projectileDamage;

		// Token: 0x040020E1 RID: 8417
		private ProjectileController projectileController;

		// Token: 0x040020E2 RID: 8418
		public bool ignoreParentForChainController;
	}
}
