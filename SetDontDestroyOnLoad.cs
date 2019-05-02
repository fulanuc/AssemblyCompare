using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003D5 RID: 981
	public class SetDontDestroyOnLoad : MonoBehaviour
	{
		// Token: 0x0600154F RID: 5455 RVA: 0x0000DF3D File Offset: 0x0000C13D
		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}
