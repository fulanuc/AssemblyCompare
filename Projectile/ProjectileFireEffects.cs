using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000554 RID: 1364
	public class ProjectileFireEffects : MonoBehaviour
	{
		// Token: 0x06001EA3 RID: 7843 RVA: 0x00095D50 File Offset: 0x00093F50
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

		// Token: 0x040020E3 RID: 8419
		public float duration = 5f;

		// Token: 0x040020E4 RID: 8420
		public int count = 5;

		// Token: 0x040020E5 RID: 8421
		public GameObject effectPrefab;

		// Token: 0x040020E6 RID: 8422
		public Vector3 randomOffset;

		// Token: 0x040020E7 RID: 8423
		private float timer;

		// Token: 0x040020E8 RID: 8424
		private float nextSpawnTimer;
	}
}
