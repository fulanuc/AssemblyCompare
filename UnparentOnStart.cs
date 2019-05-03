using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000411 RID: 1041
	public class UnparentOnStart : MonoBehaviour
	{
		// Token: 0x06001749 RID: 5961 RVA: 0x00011676 File Offset: 0x0000F876
		private void Start()
		{
			base.transform.parent = null;
		}
	}
}
