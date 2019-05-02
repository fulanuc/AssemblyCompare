using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000617 RID: 1559
	public class MPEventSystemLocator : MonoBehaviour
	{
		// Token: 0x1700031C RID: 796
		// (get) Token: 0x0600235A RID: 9050 RVA: 0x00019C90 File Offset: 0x00017E90
		// (set) Token: 0x0600235B RID: 9051 RVA: 0x00019C98 File Offset: 0x00017E98
		public MPEventSystemProvider eventSystemProvider { get; private set; }

		// Token: 0x0600235C RID: 9052 RVA: 0x00019CA1 File Offset: 0x00017EA1
		private void Awake()
		{
			this.eventSystemProvider = base.GetComponentInParent<MPEventSystemProvider>();
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x0600235D RID: 9053 RVA: 0x00019CAF File Offset: 0x00017EAF
		public MPEventSystem eventSystem
		{
			get
			{
				if (!this.eventSystemProvider)
				{
					return null;
				}
				return this.eventSystemProvider.resolvedEventSystem;
			}
		}
	}
}
