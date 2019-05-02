using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000605 RID: 1541
	public class MPEventSystemLocator : MonoBehaviour
	{
		// Token: 0x1700030A RID: 778
		// (get) Token: 0x060022CA RID: 8906 RVA: 0x000195D9 File Offset: 0x000177D9
		// (set) Token: 0x060022CB RID: 8907 RVA: 0x000195E1 File Offset: 0x000177E1
		public MPEventSystemProvider eventSystemProvider { get; private set; }

		// Token: 0x060022CC RID: 8908 RVA: 0x000195EA File Offset: 0x000177EA
		private void Awake()
		{
			this.eventSystemProvider = base.GetComponentInParent<MPEventSystemProvider>();
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x060022CD RID: 8909 RVA: 0x000195F8 File Offset: 0x000177F8
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
