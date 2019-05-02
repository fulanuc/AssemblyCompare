using System;
using RoR2.Projectile;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002E3 RID: 739
	public class EngiMineAnimator : MonoBehaviour
	{
		// Token: 0x06000ED8 RID: 3800 RVA: 0x0005A438 File Offset: 0x00058638
		private void Start()
		{
			ProjectileGhostController component = base.GetComponent<ProjectileGhostController>();
			if (component)
			{
				this.projectileTransform = component.authorityTransform;
				this.engiMineController = this.projectileTransform.GetComponent<EngiMineController>();
				this.animator = base.GetComponentInChildren<Animator>();
			}
		}

		// Token: 0x06000ED9 RID: 3801 RVA: 0x0000B76B File Offset: 0x0000996B
		private void Update()
		{
			if (this.projectileTransform && this.engiMineController.mineState == EngiMineController.MineState.Priming && this.animator)
			{
				this.animator.SetTrigger("Arming");
			}
		}

		// Token: 0x040012D7 RID: 4823
		private Transform projectileTransform;

		// Token: 0x040012D8 RID: 4824
		private EngiMineController engiMineController;

		// Token: 0x040012D9 RID: 4825
		private Animator animator;
	}
}
