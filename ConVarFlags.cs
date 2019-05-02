using System;

namespace RoR2
{
	// Token: 0x020002A8 RID: 680
	[Flags]
	public enum ConVarFlags
	{
		// Token: 0x040011D1 RID: 4561
		None = 0,
		// Token: 0x040011D2 RID: 4562
		ExecuteOnServer = 1,
		// Token: 0x040011D3 RID: 4563
		SenderMustBeServer = 2,
		// Token: 0x040011D4 RID: 4564
		Archive = 4,
		// Token: 0x040011D5 RID: 4565
		Cheat = 8,
		// Token: 0x040011D6 RID: 4566
		Engine = 16
	}
}
