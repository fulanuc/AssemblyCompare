using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200026F RID: 623
	public class BoneShrink : MonoBehaviour
	{
		// Token: 0x06000BB3 RID: 2995 RVA: 0x00009450 File Offset: 0x00007650
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06000BB4 RID: 2996 RVA: 0x0000945E File Offset: 0x0000765E
		private void LateUpdate()
		{
			this.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		}

		// Token: 0x04000F9B RID: 3995
		private new Transform transform;
	}
}
