using System;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x0200058E RID: 1422
	public static class UNETExtensions
	{
		// Token: 0x06002018 RID: 8216 RVA: 0x0009C07C File Offset: 0x0009A27C
		public static void ForceInitialize(this NetworkConnection conn, HostTopology hostTopology)
		{
			int num = 0;
			conn.Initialize("localhost", num, num, hostTopology);
		}

		// Token: 0x0400222A RID: 8746
		private static int nextConnectionId = -1;
	}
}
