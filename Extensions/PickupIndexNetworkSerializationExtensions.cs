using System;
using UnityEngine.Networking;

namespace RoR2.Extensions
{
	// Token: 0x02000509 RID: 1289
	public static class PickupIndexNetworkSerializationExtensions
	{
		// Token: 0x06001D3C RID: 7484 RVA: 0x000156DB File Offset: 0x000138DB
		public static void Write(this NetworkWriter writer, PickupIndex value)
		{
			PickupIndex.WriteToNetworkWriter(writer, value);
		}

		// Token: 0x06001D3D RID: 7485 RVA: 0x000156E4 File Offset: 0x000138E4
		public static PickupIndex ReadPickupIndex(this NetworkReader reader)
		{
			return PickupIndex.ReadFromNetworkReader(reader);
		}
	}
}
