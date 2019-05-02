using System;

namespace RoR2.Stats
{
	// Token: 0x020004FA RID: 1274
	public struct StatEvent
	{
		// Token: 0x04001F14 RID: 7956
		public string stringValue;

		// Token: 0x04001F15 RID: 7957
		public double doubleValue;

		// Token: 0x04001F16 RID: 7958
		public ulong ulongValue;

		// Token: 0x020004FB RID: 1275
		public enum Type
		{
			// Token: 0x04001F18 RID: 7960
			Damage,
			// Token: 0x04001F19 RID: 7961
			Kill,
			// Token: 0x04001F1A RID: 7962
			Walk,
			// Token: 0x04001F1B RID: 7963
			Die,
			// Token: 0x04001F1C RID: 7964
			Lose,
			// Token: 0x04001F1D RID: 7965
			Win
		}
	}
}
