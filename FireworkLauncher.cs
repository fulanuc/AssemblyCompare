using System;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002F7 RID: 759
	public class FireworkLauncher : MonoBehaviour
	{
		// Token: 0x06000F65 RID: 3941 RVA: 0x0005CC30 File Offset: 0x0005AE30
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				if (this.remaining <= 0 || !this.owner)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				this.nextFireTimer -= Time.fixedDeltaTime;
				if (this.nextFireTimer <= 0f)
				{
					this.remaining--;
					this.nextFireTimer += this.launchInterval;
					this.FireMissile();
				}
			}
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x0005CCAC File Offset: 0x0005AEAC
		private void FireMissile()
		{
			CharacterBody component = this.owner.GetComponent<CharacterBody>();
			if (component)
			{
				Vector2 vector = UnityEngine.Random.insideUnitCircle * this.randomCircleRange;
				ProjectileManager.instance.FireProjectile(this.projectilePrefab, base.transform.position + new Vector3(vector.x, 0f, vector.y), Util.QuaternionSafeLookRotation(Vector3.up + new Vector3(vector.x, 0f, vector.y)), this.owner, component.damage * this.damageCoefficient, 200f, this.crit, DamageColorIndex.Item, null, -1f);
			}
		}

		// Token: 0x04001379 RID: 4985
		public GameObject projectilePrefab;

		// Token: 0x0400137A RID: 4986
		public float launchInterval = 0.1f;

		// Token: 0x0400137B RID: 4987
		public float damageCoefficient = 3f;

		// Token: 0x0400137C RID: 4988
		public float coneAngle = 10f;

		// Token: 0x0400137D RID: 4989
		public float randomCircleRange;

		// Token: 0x0400137E RID: 4990
		[HideInInspector]
		public GameObject owner;

		// Token: 0x0400137F RID: 4991
		[HideInInspector]
		public TeamIndex team;

		// Token: 0x04001380 RID: 4992
		[HideInInspector]
		public int remaining;

		// Token: 0x04001381 RID: 4993
		[HideInInspector]
		public bool crit;

		// Token: 0x04001382 RID: 4994
		private float nextFireTimer;
	}
}
