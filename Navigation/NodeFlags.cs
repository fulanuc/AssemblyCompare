using System;

namespace RoR2.Navigation
{
	// Token: 0x02000526 RID: 1318
	[Flags]
	public enum NodeFlags : byte
	{
		// Token: 0x04001FE2 RID: 8162
		None = 0,
		// Token: 0x04001FE3 RID: 8163
		NoCeiling = 1,
		// Token: 0x04001FE4 RID: 8164
		TeleporterOK = 2,
		// Token: 0x04001FE5 RID: 8165
		NoCharacterSpawn = 4,
		// Token: 0x04001FE6 RID: 8166
		NoChestSpawn = 8,
		// Token: 0x04001FE7 RID: 8167
		NoShrineSpawn = 16
	}
}
