using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000545 RID: 1349
	[RequireComponent(typeof(ProjectileGhostController))]
	public class EngiMineGhostController : MonoBehaviour
	{
		// Token: 0x06001E5E RID: 7774 RVA: 0x0009476C File Offset: 0x0009296C
		private EngiMineController LookupMineController()
		{
			if (!this.cachedMineController)
			{
				Transform authorityTransform = this.projectileGhostController.authorityTransform;
				if (authorityTransform)
				{
					this.cachedMineController = authorityTransform.GetComponent<EngiMineController>();
				}
			}
			return this.cachedMineController;
		}

		// Token: 0x06001E5F RID: 7775 RVA: 0x000162D8 File Offset: 0x000144D8
		private void Awake()
		{
			this.projectileGhostController = base.GetComponent<ProjectileGhostController>();
			this.stickIndicator.SetActive(false);
		}

		// Token: 0x06001E60 RID: 7776 RVA: 0x000947AC File Offset: 0x000929AC
		private void FixedUpdate()
		{
			bool flag = false;
			EngiMineController engiMineController = this.LookupMineController();
			if (engiMineController)
			{
				flag = (engiMineController.mineState == EngiMineController.MineState.Sticking);
			}
			if (flag != this.cachedArmed)
			{
				this.cachedArmed = flag;
				this.stickIndicator.SetActive(flag);
			}
		}

		// Token: 0x0400206E RID: 8302
		private ProjectileGhostController projectileGhostController;

		// Token: 0x0400206F RID: 8303
		[Tooltip("Child object which will be enabled if the projectile is armed.")]
		public GameObject stickIndicator;

		// Token: 0x04002070 RID: 8304
		private EngiMineController cachedMineController;

		// Token: 0x04002071 RID: 8305
		private bool cachedArmed;
	}
}
