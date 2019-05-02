using System;

namespace RoR2
{
	// Token: 0x020002AB RID: 683
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class ConCommandAttribute : Attribute
	{
		// Token: 0x040011E9 RID: 4585
		public string commandName;

		// Token: 0x040011EA RID: 4586
		public ConVarFlags flags;

		// Token: 0x040011EB RID: 4587
		public string helpText = "";
	}
}
