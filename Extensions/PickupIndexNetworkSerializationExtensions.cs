using System;
using UnityEngine.Networking;

namespace RoR2.Extensions
{
	// Token: 0x02000518 RID: 1304
	public static class PickupIndexNetworkSerializationExtensions
	{
		// Token: 0x06001DA4 RID: 7588 RVA: 0x00015B84 File Offset: 0x00013D84
		public static void Write(this NetworkWriter writer, PickupIndex value)
		{
			PickupIndex.WriteToNetworkWriter(writer, value);
		}

		// Token: 0x06001DA5 RID: 7589 RVA: 0x00015B8D File Offset: 0x00013D8D
		public static PickupIndex ReadPickupIndex(this NetworkReader reader)
		{
			return PickupIndex.ReadFromNetworkReader(reader);
		}
	}
}
