using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000236 RID: 566
	public class DamageInfo
	{
		// Token: 0x06000AC0 RID: 2752 RVA: 0x00008ACC File Offset: 0x00006CCC
		public void ModifyDamageInfo(HurtBox.DamageModifier damageModifier)
		{
			switch (damageModifier)
			{
			case HurtBox.DamageModifier.Normal:
			case HurtBox.DamageModifier.SniperTarget:
				break;
			case HurtBox.DamageModifier.Weak:
				this.damageType |= DamageType.WeakPointHit;
				return;
			case HurtBox.DamageModifier.Barrier:
				this.damageType |= DamageType.BarrierBlocked;
				break;
			default:
				return;
			}
		}

		// Token: 0x04000E6F RID: 3695
		public float damage;

		// Token: 0x04000E70 RID: 3696
		public bool crit;

		// Token: 0x04000E71 RID: 3697
		public GameObject inflictor;

		// Token: 0x04000E72 RID: 3698
		public GameObject attacker;

		// Token: 0x04000E73 RID: 3699
		public Vector3 position;

		// Token: 0x04000E74 RID: 3700
		public Vector3 force;

		// Token: 0x04000E75 RID: 3701
		public bool rejected;

		// Token: 0x04000E76 RID: 3702
		public ProcChainMask procChainMask;

		// Token: 0x04000E77 RID: 3703
		public float procCoefficient = 1f;

		// Token: 0x04000E78 RID: 3704
		public DamageType damageType;

		// Token: 0x04000E79 RID: 3705
		public DamageColorIndex damageColorIndex;
	}
}
