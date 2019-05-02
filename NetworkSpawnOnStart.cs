using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000373 RID: 883
	public class NetworkSpawnOnStart : MonoBehaviour
	{
		// Token: 0x0600124F RID: 4687 RVA: 0x0000DFB0 File Offset: 0x0000C1B0
		private void Start()
		{
			if (NetworkServer.active)
			{
				NetworkServer.Spawn(base.gameObject);
			}
		}
	}
}
