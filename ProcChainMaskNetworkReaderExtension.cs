using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200046E RID: 1134
	internal static class ProcChainMaskNetworkReaderExtension
	{
		// Token: 0x06001967 RID: 6503 RVA: 0x000835FC File Offset: 0x000817FC
		public static ProcChainMask ReadProcChainMask(this NetworkReader reader)
		{
			return new ProcChainMask
			{
				mask = reader.ReadUInt16()
			};
		}
	}
}
