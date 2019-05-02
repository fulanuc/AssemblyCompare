using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000557 RID: 1367
	public class ProjectileGhostCluster : MonoBehaviour
	{
		// Token: 0x06001EB2 RID: 7858 RVA: 0x00096054 File Offset: 0x00094254
		private void Start()
		{
			float num = 1f / (Mathf.Log((float)this.clusterCount, 4f) + 1f);
			Vector3 position = base.transform.position;
			for (int i = 0; i < this.clusterCount; i++)
			{
				Vector3 b;
				if (this.distributeEvenly)
				{
					b = Vector3.zero;
				}
				else
				{
					b = UnityEngine.Random.insideUnitSphere * this.clusterDistance;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ghostClusterPrefab, position + b, Quaternion.identity, base.transform);
				gameObject.transform.localScale = Vector3.one / (Mathf.Log((float)this.clusterCount, 4f) + 1f);
				TrailRenderer component = gameObject.GetComponent<TrailRenderer>();
				if (component)
				{
					component.widthMultiplier *= num;
				}
			}
		}

		// Token: 0x06001EB3 RID: 7859 RVA: 0x000025DA File Offset: 0x000007DA
		private void Update()
		{
		}

		// Token: 0x040020F3 RID: 8435
		public GameObject ghostClusterPrefab;

		// Token: 0x040020F4 RID: 8436
		public int clusterCount;

		// Token: 0x040020F5 RID: 8437
		public bool distributeEvenly;

		// Token: 0x040020F6 RID: 8438
		public float clusterDistance;
	}
}
