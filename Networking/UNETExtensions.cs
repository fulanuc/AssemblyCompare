using System;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x020005A1 RID: 1441
	public static class UNETExtensions
	{
		// Token: 0x060020A9 RID: 8361 RVA: 0x0009D5A8 File Offset: 0x0009B7A8
		public static void ForceInitialize(this NetworkConnection conn, HostTopology hostTopology)
		{
			int num = 0;
			conn.Initialize("localhost", num, num, hostTopology);
		}

		// Token: 0x04002282 RID: 8834
		private static int nextConnectionId = -1;
	}
}
