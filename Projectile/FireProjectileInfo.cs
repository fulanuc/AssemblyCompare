using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000552 RID: 1362
	public struct FireProjectileInfo
	{
		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06001E6B RID: 7787 RVA: 0x000163E4 File Offset: 0x000145E4
		// (set) Token: 0x06001E6A RID: 7786 RVA: 0x000163CA File Offset: 0x000145CA
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

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06001E6D RID: 7789 RVA: 0x00016406 File Offset: 0x00014606
		// (set) Token: 0x06001E6C RID: 7788 RVA: 0x000163EC File Offset: 0x000145EC
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

		// Token: 0x040020F8 RID: 8440
		public GameObject projectilePrefab;

		// Token: 0x040020F9 RID: 8441
		public Vector3 position;

		// Token: 0x040020FA RID: 8442
		public Quaternion rotation;

		// Token: 0x040020FB RID: 8443
		public GameObject owner;

		// Token: 0x040020FC RID: 8444
		public GameObject target;

		// Token: 0x040020FD RID: 8445
		public bool useSpeedOverride;

		// Token: 0x040020FE RID: 8446
		private float _speedOverride;

		// Token: 0x040020FF RID: 8447
		public bool useFuseOverride;

		// Token: 0x04002100 RID: 8448
		private float _fuseOverride;

		// Token: 0x04002101 RID: 8449
		public float damage;

		// Token: 0x04002102 RID: 8450
		public float force;

		// Token: 0x04002103 RID: 8451
		public bool crit;

		// Token: 0x04002104 RID: 8452
		public DamageColorIndex damageColorIndex;

		// Token: 0x04002105 RID: 8453
		public ProcChainMask procChainMask;
	}
}
