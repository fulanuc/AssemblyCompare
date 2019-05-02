using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000536 RID: 1334
	[RequireComponent(typeof(ProjectileGhostController))]
	public class EngiMineGhostController : MonoBehaviour
	{
		// Token: 0x06001DF4 RID: 7668 RVA: 0x00093A50 File Offset: 0x00091C50
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

		// Token: 0x06001DF5 RID: 7669 RVA: 0x00015DF9 File Offset: 0x00013FF9
		private void Awake()
		{
			this.projectileGhostController = base.GetComponent<ProjectileGhostController>();
			this.stickIndicator.SetActive(false);
		}

		// Token: 0x06001DF6 RID: 7670 RVA: 0x00093A90 File Offset: 0x00091C90
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

		// Token: 0x04002030 RID: 8240
		private ProjectileGhostController projectileGhostController;

		// Token: 0x04002031 RID: 8241
		[Tooltip("Child object which will be enabled if the projectile is armed.")]
		public GameObject stickIndicator;

		// Token: 0x04002032 RID: 8242
		private EngiMineController cachedMineController;

		// Token: 0x04002033 RID: 8243
		private bool cachedArmed;
	}
}
