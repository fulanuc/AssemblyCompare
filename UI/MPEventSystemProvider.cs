using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000606 RID: 1542
	public class MPEventSystemProvider : MonoBehaviour
	{
		// Token: 0x1700030C RID: 780
		// (get) Token: 0x060022CF RID: 8911 RVA: 0x00019614 File Offset: 0x00017814
		public MPEventSystem resolvedEventSystem
		{
			get
			{
				if (this.eventSystem)
				{
					return this.eventSystem;
				}
				if (this.fallBackToMainEventSystem)
				{
					return MPEventSystemManager.primaryEventSystem;
				}
				return null;
			}
		}

		// Token: 0x040025BF RID: 9663
		public MPEventSystem eventSystem;

		// Token: 0x040025C0 RID: 9664
		public bool fallBackToMainEventSystem = true;
	}
}
