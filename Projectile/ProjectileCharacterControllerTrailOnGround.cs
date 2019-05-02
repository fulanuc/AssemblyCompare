using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200053E RID: 1342
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(ProjectileDamage))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileCharacterControllerTrailOnGround : MonoBehaviour
	{
		// Token: 0x06001E0F RID: 7695 RVA: 0x00015EF8 File Offset: 0x000140F8
		private void Awake()
		{
			this.characterController = base.GetComponent<CharacterController>();
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001E10 RID: 7696 RVA: 0x00094634 File Offset: 0x00092834
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

		// Token: 0x06001E11 RID: 7697 RVA: 0x00015F1E File Offset: 0x0001411E
		private void OnDestroy()
		{
			this.DiscontinueTrail();
		}

		// Token: 0x06001E12 RID: 7698 RVA: 0x00015F26 File Offset: 0x00014126
		private void DiscontinueTrail()
		{
			if (this.currentTrailObject)
			{
				this.currentTrailObject.AddComponent<DestroyOnTimer>().duration = 1f;
				this.currentTrailObject = null;
			}
		}

		// Token: 0x04002064 RID: 8292
		public GameObject trailPrefab;

		// Token: 0x04002065 RID: 8293
		public float damageToTrailDpsFactor = 1f;

		// Token: 0x04002066 RID: 8294
		private CharacterController characterController;

		// Token: 0x04002067 RID: 8295
		private ProjectileController projectileController;

		// Token: 0x04002068 RID: 8296
		private ProjectileDamage projectileDamage;

		// Token: 0x04002069 RID: 8297
		private GameObject currentTrailObject;
	}
}
