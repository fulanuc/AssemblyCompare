using System;

namespace RoR2.Navigation
{
	// Token: 0x02000535 RID: 1333
	[Flags]
	public enum NodeFlags : byte
	{
		// Token: 0x04002020 RID: 8224
		None = 0,
		// Token: 0x04002021 RID: 8225
		NoCeiling = 1,
		// Token: 0x04002022 RID: 8226
		TeleporterOK = 2,
		// Token: 0x04002023 RID: 8227
		NoCharacterSpawn = 4,
		// Token: 0x04002024 RID: 8228
		NoChestSpawn = 8,
		// Token: 0x04002025 RID: 8229
		NoShrineSpawn = 16
	}
}
