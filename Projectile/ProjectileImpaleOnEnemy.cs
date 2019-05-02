using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200054E RID: 1358
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileImpaleOnEnemy : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E5A RID: 7770 RVA: 0x00016306 File Offset: 0x00014506
		private void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001E5B RID: 7771 RVA: 0x00095A78 File Offset: 0x00093C78
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (!this.alive)
			{
				return;
			}
			Collider collider = impactInfo.collider;
			if (collider)
			{
				HurtBox component = collider.GetComponent<HurtBox>();
				if (component)
				{
					Vector3 position = base.transform.position;
					Vector3 estimatedPointOfImpact = impactInfo.estimatedPointOfImpact;
					Quaternion identity = Quaternion.identity;
					if (this.rigidbody)
					{
						Util.QuaternionSafeLookRotation(this.rigidbody.velocity);
					}
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.impalePrefab, component.transform);
					gameObject.transform.position = estimatedPointOfImpact;
					gameObject.transform.rotation = base.transform.rotation;
				}
			}
		}

		// Token: 0x040020E1 RID: 8417
		private bool alive = true;

		// Token: 0x040020E2 RID: 8418
		public GameObject impalePrefab;

		// Token: 0x040020E3 RID: 8419
		private Rigidbody rigidbody;
	}
}
