using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000547 RID: 1351
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileFuse : MonoBehaviour
	{
		// Token: 0x06001E45 RID: 7749 RVA: 0x000161BE File Offset: 0x000143BE
		private void Awake()
		{
			if (!NetworkServer.active)
			{
				base.enabled = false;
			}
		}

		// Token: 0x06001E46 RID: 7750 RVA: 0x000161CE File Offset: 0x000143CE
		private void FixedUpdate()
		{
			this.fuse -= Time.fixedDeltaTime;
			if (this.fuse <= 0f)
			{
				base.enabled = false;
				this.onFuse.Invoke();
			}
		}

		// Token: 0x040020B3 RID: 8371
		public float fuse;

		// Token: 0x040020B4 RID: 8372
		public UnityEvent onFuse;
	}
}
