using System;

namespace RoR2
{
	// Token: 0x020002A9 RID: 681
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class ConCommandAttribute : Attribute
	{
		// Token: 0x040011D7 RID: 4567
		public string commandName;

		// Token: 0x040011D8 RID: 4568
		public ConVarFlags flags;

		// Token: 0x040011D9 RID: 4569
		public string helpText = "";
	}
}
