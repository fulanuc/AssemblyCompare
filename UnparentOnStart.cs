using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000417 RID: 1047
	public class UnparentOnStart : MonoBehaviour
	{
		// Token: 0x0600178C RID: 6028 RVA: 0x00011AA2 File Offset: 0x0000FCA2
		private void Start()
		{
			base.transform.parent = null;
		}
	}
}
