using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003DB RID: 987
	public class SetDontDestroyOnLoad : MonoBehaviour
	{
		// Token: 0x0600157D RID: 5501 RVA: 0x0000E026 File Offset: 0x0000C226
		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}
