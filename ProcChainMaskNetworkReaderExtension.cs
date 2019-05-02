using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000479 RID: 1145
	internal static class ProcChainMaskNetworkReaderExtension
	{
		// Token: 0x060019C4 RID: 6596 RVA: 0x00083FA4 File Offset: 0x000821A4
		public static ProcChainMask ReadProcChainMask(this NetworkReader reader)
		{
			return new ProcChainMask
			{
				mask = reader.ReadUInt16()
			};
		}
	}
}
