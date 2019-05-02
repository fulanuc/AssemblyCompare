using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002CF RID: 719
	public class DestroyOnUnseen : MonoBehaviour
	{
		// Token: 0x06000E83 RID: 3715 RVA: 0x0000B33A File Offset: 0x0000953A
		private void Start()
		{
			this.rend = base.GetComponentInChildren<Renderer>();
		}

		// Token: 0x06000E84 RID: 3716 RVA: 0x0000B348 File Offset: 0x00009548
		private void Update()
		{
			if (this.cull && this.rend && !this.rend.isVisible)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x0400127B RID: 4731
		public bool cull;

		// Token: 0x0400127C RID: 4732
		private Renderer rend;
	}
}
