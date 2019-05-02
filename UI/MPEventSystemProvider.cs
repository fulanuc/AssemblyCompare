using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000618 RID: 1560
	public class MPEventSystemProvider : MonoBehaviour
	{
		// Token: 0x1700031E RID: 798
		// (get) Token: 0x0600235F RID: 9055 RVA: 0x00019CCB File Offset: 0x00017ECB
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

		// Token: 0x0400261A RID: 9754
		public MPEventSystem eventSystem;

		// Token: 0x0400261B RID: 9755
		public bool fallBackToMainEventSystem = true;
	}
}
