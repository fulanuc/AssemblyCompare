using System;

namespace RoR2.ConVar
{
	// Token: 0x0200068E RID: 1678
	public abstract class BaseConVar
	{
		// Token: 0x060025AC RID: 9644 RVA: 0x000B16FC File Offset: 0x000AF8FC
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

		// Token: 0x060025AD RID: 9645
		public abstract void SetString(string newValue);

		// Token: 0x060025AE RID: 9646
		public abstract string GetString();

		// Token: 0x04002882 RID: 10370
		public string name;

		// Token: 0x04002883 RID: 10371
		public ConVarFlags flags;

		// Token: 0x04002884 RID: 10372
		public string defaultValue;

		// Token: 0x04002885 RID: 10373
		public string helpText;
	}
}
