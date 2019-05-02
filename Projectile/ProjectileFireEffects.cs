using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000545 RID: 1349
	public class ProjectileFireEffects : MonoBehaviour
	{
		// Token: 0x06001E39 RID: 7737 RVA: 0x00095034 File Offset: 0x00093234
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
				if (this.effectPrefab)
				{
					Vector3 b = new Vector3(UnityEngine.Random.Range(-this.randomOffset.x, this.randomOffset.x), UnityEngine.Random.Range(-this.randomOffset.y, this.randomOffset.y), UnityEngine.Random.Range(-this.randomOffset.z, this.randomOffset.z));
					EffectManager.instance.SimpleImpactEffect(this.effectPrefab, base.transform.position + b, Vector3.forward, true);
				}
			}
		}

		// Token: 0x040020A5 RID: 8357
		public float duration = 5f;

		// Token: 0x040020A6 RID: 8358
		public int count = 5;

		// Token: 0x040020A7 RID: 8359
		public GameObject effectPrefab;

		// Token: 0x040020A8 RID: 8360
		public Vector3 randomOffset;

		// Token: 0x040020A9 RID: 8361
		private float timer;

		// Token: 0x040020AA RID: 8362
		private float nextSpawnTimer;
	}
}
