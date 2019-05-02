using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000548 RID: 1352
	public class ProjectileGhostCluster : MonoBehaviour
	{
		// Token: 0x06001E48 RID: 7752 RVA: 0x00095338 File Offset: 0x00093538
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

		// Token: 0x06001E49 RID: 7753 RVA: 0x000025F6 File Offset: 0x000007F6
		private void Update()
		{
		}

		// Token: 0x040020B5 RID: 8373
		public GameObject ghostClusterPrefab;

		// Token: 0x040020B6 RID: 8374
		public int clusterCount;

		// Token: 0x040020B7 RID: 8375
		public bool distributeEvenly;

		// Token: 0x040020B8 RID: 8376
		public float clusterDistance;
	}
}
