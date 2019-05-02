using System;

namespace RoR2.ConVar
{
	// Token: 0x0200067C RID: 1660
	public abstract class BaseConVar
	{
		// Token: 0x06002515 RID: 9493 RVA: 0x000B000C File Offset: 0x000AE20C
		protected BaseConVar(string name, ConVarFlags flags, string defaultValue, string helpText)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
			this.flags = flags;
			this.defaultValue = defaultValue;
			if (helpText == null)
			{
				throw new ArgumentNullException("helpText");
			}
			this.helpText = helpText;
		}

		// Token: 0x06002516 RID: 9494
		public abstract void SetString(string newValue);

		// Token: 0x06002517 RID: 9495
		public abstract string GetString();

		// Token: 0x04002826 RID: 10278
		public string name;

		// Token: 0x04002827 RID: 10279
		public ConVarFlags flags;

		// Token: 0x04002828 RID: 10280
		public string defaultValue;

		// Token: 0x04002829 RID: 10281
		public string helpText;
	}
}
