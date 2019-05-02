using System;

namespace RoR2.Stats
{
	// Token: 0x02000509 RID: 1289
	public struct StatEvent
	{
		// Token: 0x04001F52 RID: 8018
		public string stringValue;

		// Token: 0x04001F53 RID: 8019
		public double doubleValue;

		// Token: 0x04001F54 RID: 8020
		public ulong ulongValue;

		// Token: 0x0200050A RID: 1290
		public enum Type
		{
			// Token: 0x04001F56 RID: 8022
			Damage,
			// Token: 0x04001F57 RID: 8023
			Kill,
			// Token: 0x04001F58 RID: 8024
			Walk,
			// Token: 0x04001F59 RID: 8025
			Die,
			// Token: 0x04001F5A RID: 8026
			Lose,
			// Token: 0x04001F5B RID: 8027
			Win
		}
	}
}
