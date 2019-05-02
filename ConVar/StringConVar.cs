using System;

namespace RoR2.ConVar
{
	// Token: 0x02000680 RID: 1664
	public class StringConVar : BaseConVar
	{
		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06002528 RID: 9512 RVA: 0x0001B051 File Offset: 0x00019251
		// (set) Token: 0x06002529 RID: 9513 RVA: 0x0001B059 File Offset: 0x00019259
		public string value { get; protected set; }

		// Token: 0x0600252A RID: 9514 RVA: 0x000090A8 File Offset: 0x000072A8
		public StringConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x0600252B RID: 9515 RVA: 0x0001B062 File Offset: 0x00019262
		public override void SetString(string newValue)
		{
			this.value = newValue;
		}

		// Token: 0x0600252C RID: 9516 RVA: 0x0001B06B File Offset: 0x0001926B
		public override string GetString()
		{
			return this.value;
		}
	}
}
