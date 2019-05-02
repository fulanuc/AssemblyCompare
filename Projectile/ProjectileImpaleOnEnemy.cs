using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200055D RID: 1373
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileImpaleOnEnemy : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001EC4 RID: 7876 RVA: 0x000167E5 File Offset: 0x000149E5
		private void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001EC5 RID: 7877 RVA: 0x00096794 File Offset: 0x00094994
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

		// Token: 0x0400211F RID: 8479
		private bool alive = true;

		// Token: 0x04002120 RID: 8480
		public GameObject impalePrefab;

		// Token: 0x04002121 RID: 8481
		private Rigidbody rigidbody;
	}
}
