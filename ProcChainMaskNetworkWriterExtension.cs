using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000478 RID: 1144
	internal static class ProcChainMaskNetworkWriterExtension
	{
		// Token: 0x060019C3 RID: 6595 RVA: 0x00013364 File Offset: 0x00011564
		public static void Write(this NetworkWriter writer, ProcChainMask procChainMask)
		{
			writer.Write(procChainMask.mask);
		}
	}
}
