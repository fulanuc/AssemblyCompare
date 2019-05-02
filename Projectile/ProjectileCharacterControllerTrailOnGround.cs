using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200054D RID: 1357
	[RequireComponent(typeof(ProjectileDamage))]
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(CharacterController))]
	public class ProjectileCharacterControllerTrailOnGround : MonoBehaviour
	{
		// Token: 0x06001E79 RID: 7801 RVA: 0x000163D7 File Offset: 0x000145D7
		private void Awake()
		{
			this.characterController = base.GetComponent<CharacterController>();
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001E7A RID: 7802 RVA: 0x00095350 File Offset: 0x00093550
		private void FixedUpdate()
		{
			if (this.characterController.isGrounded)
			{
				if (!this.currentTrailObject)
				{
					this.currentTrailObject = UnityEngine.Object.Instantiate<GameObject>(this.trailPrefab, base.transform.position, base.transform.rotation);
					DamageTrail component = this.currentTrailObject.GetComponent<DamageTrail>();
					component.damagePerSecond = this.projectileDamage.damage * this.damageToTrailDpsFactor;
					component.owner = this.projectileController.owner;
				}
				this.currentTrailObject.transform.position = base.transform.position;
				return;
			}
			this.DiscontinueTrail();
		}

		// Token: 0x06001E7B RID: 7803 RVA: 0x000163FD File Offset: 0x000145FD
		private void OnDestroy()
		{
			this.DiscontinueTrail();
		}

		// Token: 0x06001E7C RID: 7804 RVA: 0x00016405 File Offset: 0x00014605
		private void DiscontinueTrail()
		{
			if (this.currentTrailObject)
			{
				this.currentTrailObject.AddComponent<DestroyOnTimer>().duration = 1f;
				this.currentTrailObject = null;
			}
		}

		// Token: 0x040020A2 RID: 8354
		public GameObject trailPrefab;

		// Token: 0x040020A3 RID: 8355
		public float damageToTrailDpsFactor = 1f;

		// Token: 0x040020A4 RID: 8356
		private CharacterController characterController;

		// Token: 0x040020A5 RID: 8357
		private ProjectileController projectileController;

		// Token: 0x040020A6 RID: 8358
		private ProjectileDamage projectileDamage;

		// Token: 0x040020A7 RID: 8359
		private GameObject currentTrailObject;
	}
}
