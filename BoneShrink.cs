using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200026F RID: 623
	public class BoneShrink : MonoBehaviour
	{
		// Token: 0x06000BAA RID: 2986 RVA: 0x000093F8 File Offset: 0x000075F8
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06000BAB RID: 2987 RVA: 0x00009406 File Offset: 0x00007606
		private void LateUpdate()
		{
			this.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		}

		// Token: 0x04000F95 RID: 3989
		private new Transform transform;
	}
}
