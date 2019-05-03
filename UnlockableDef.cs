using System;
using JetBrains.Annotations;

namespace RoR2
{
	// Token: 0x020004C7 RID: 1223
	public class UnlockableDef
	{
		// Token: 0x04001E09 RID: 7689
		public string name;

		// Token: 0x04001E0A RID: 7690
		public string nameToken = "???";

		// Token: 0x04001E0B RID: 7691
		public string displayModelPath = "Prefabs/NullModel";

		// Token: 0x04001E0C RID: 7692
		public UnlockableIndex index;

		// Token: 0x04001E0D RID: 7693
		public bool hidden;

		// Token: 0x04001E0E RID: 7694
		[NotNull]
		public Func<string> getHowToUnlockString = () => "???";

		// Token: 0x04001E0F RID: 7695
		[NotNull]
		public Func<string> getUnlockedString = () => "???";
	}
}
