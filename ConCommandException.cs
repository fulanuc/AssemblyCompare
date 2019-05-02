using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Facepunch.Steamworks;

namespace RoR2
{
	// Token: 0x020002AC RID: 684
	[Serializable]
	public class ConCommandException : Exception
	{
		// Token: 0x06000DE7 RID: 3559 RVA: 0x0000AC27 File Offset: 0x00008E27
		public ConCommandException()
		{
		}

		// Token: 0x06000DE8 RID: 3560 RVA: 0x0000AC2F File Offset: 0x00008E2F
		public ConCommandException(string message) : base(message)
		{
		}

		// Token: 0x06000DE9 RID: 3561 RVA: 0x0000AC38 File Offset: 0x00008E38
		public ConCommandException(string message, Exception inner) : base(message, inner)
		{
		}

		// Token: 0x06000DEA RID: 3562 RVA: 0x0000AC42 File Offset: 0x00008E42
		protected ConCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000DEB RID: 3563 RVA: 0x0000AC4C File Offset: 0x00008E4C
		public static void CheckSteamworks()
		{
			if (Client.Instance == null)
			{
				throw new ConCommandException("Steamworks not available.");
			}
		}

		// Token: 0x06000DEC RID: 3564 RVA: 0x0000AC60 File Offset: 0x00008E60
		public static void CheckArgumentCount(List<string> args, int requiredArgCount)
		{
			if (args.Count < requiredArgCount)
			{
				throw new ConCommandException(string.Format("{0} argument(s) required, {1} argument(s) provided.", requiredArgCount, args.Count));
			}
		}
	}
}
