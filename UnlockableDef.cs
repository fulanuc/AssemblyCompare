using System;
using JetBrains.Annotations;

namespace RoR2
{
	// Token: 0x020004D5 RID: 1237
	public class UnlockableDef
	{
		// Token: 0x04001E43 RID: 7747
		public string name;

		// Token: 0x04001E44 RID: 7748
		public string nameToken = "???";

		// Token: 0x04001E45 RID: 7749
		public string displayModelPath = "Prefabs/NullModel";

		// Token: 0x04001E46 RID: 7750
		public UnlockableIndex index;

		// Token: 0x04001E47 RID: 7751
		public bool hidden;

		// Token: 0x04001E48 RID: 7752
		[NotNull]
		public Func<string> getHowToUnlockString = () => "???";

		// Token: 0x04001E49 RID: 7753
		[NotNull]
		public Func<string> getUnlockedString = () => "???";
	}
}
