using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000561 RID: 1377
	public struct FireProjectileInfo
	{
		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x06001ED5 RID: 7893 RVA: 0x000168C3 File Offset: 0x00014AC3
		// (set) Token: 0x06001ED4 RID: 7892 RVA: 0x000168A9 File Offset: 0x00014AA9
		public float speedOverride
		{
			get
			{
				return this._speedOverride;
			}
			set
			{
				this.useSpeedOverride = (value != -1f);
				this._speedOverride = value;
			}
		}

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06001ED7 RID: 7895 RVA: 0x000168E5 File Offset: 0x00014AE5
		// (set) Token: 0x06001ED6 RID: 7894 RVA: 0x000168CB File Offset: 0x00014ACB
		public float fuseOverride
		{
			get
			{
				return this._fuseOverride;
			}
			set
			{
				this.useFuseOverride = (value != -1f);
				this._fuseOverride = value;
			}
		}

		// Token: 0x04002136 RID: 8502
		public GameObject projectilePrefab;

		// Token: 0x04002137 RID: 8503
		public Vector3 position;

		// Token: 0x04002138 RID: 8504
		public Quaternion rotation;

		// Token: 0x04002139 RID: 8505
		public GameObject owner;

		// Token: 0x0400213A RID: 8506
		public GameObject target;

		// Token: 0x0400213B RID: 8507
		public bool useSpeedOverride;

		// Token: 0x0400213C RID: 8508
		private float _speedOverride;

		// Token: 0x0400213D RID: 8509
		public bool useFuseOverride;

		// Token: 0x0400213E RID: 8510
		private float _fuseOverride;

		// Token: 0x0400213F RID: 8511
		public float damage;

		// Token: 0x04002140 RID: 8512
		public float force;

		// Token: 0x04002141 RID: 8513
		public bool crit;

		// Token: 0x04002142 RID: 8514
		public DamageColorIndex damageColorIndex;

		// Token: 0x04002143 RID: 8515
		public ProcChainMask procChainMask;
	}
}
