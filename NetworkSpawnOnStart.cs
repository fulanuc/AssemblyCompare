using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000370 RID: 880
	public class NetworkSpawnOnStart : MonoBehaviour
	{
		// Token: 0x06001238 RID: 4664 RVA: 0x0000DEC7 File Offset: 0x0000C0C7
		private void Start()
		{
			if (NetworkServer.active)
			{
				NetworkServer.Spawn(base.gameObject);
			}
		}
	}
}
