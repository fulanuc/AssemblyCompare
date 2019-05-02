using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200046D RID: 1133
	internal static class ProcChainMaskNetworkWriterExtension
	{
		// Token: 0x06001966 RID: 6502 RVA: 0x00012E4A File Offset: 0x0001104A
		public static void Write(this NetworkWriter writer, ProcChainMask procChainMask)
		{
			writer.Write(procChainMask.mask);
		}
	}
}
