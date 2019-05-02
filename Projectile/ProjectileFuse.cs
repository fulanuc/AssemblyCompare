using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000556 RID: 1366
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileFuse : MonoBehaviour
	{
		// Token: 0x06001EAF RID: 7855 RVA: 0x0001669D File Offset: 0x0001489D
		private void Awake()
		{
			if (!NetworkServer.active)
			{
				base.enabled = false;
			}
		}

		// Token: 0x06001EB0 RID: 7856 RVA: 0x000166AD File Offset: 0x000148AD
		private void FixedUpdate()
		{
			this.fuse -= Time.fixedDeltaTime;
			if (this.fuse <= 0f)
			{
				base.enabled = false;
				this.onFuse.Invoke();
			}
		}

		// Token: 0x040020F1 RID: 8433
		public float fuse;

		// Token: 0x040020F2 RID: 8434
		public UnityEvent onFuse;
	}
}
