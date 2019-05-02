using System;

namespace RoR2.ConVar
{
	// Token: 0x02000692 RID: 1682
	public class StringConVar : BaseConVar
	{
		// Token: 0x17000338 RID: 824
		// (get) Token: 0x060025BF RID: 9663 RVA: 0x0001B784 File Offset: 0x00019984
		// (set) Token: 0x060025C0 RID: 9664 RVA: 0x0001B78C File Offset: 0x0001998C
		public string value { get; protected set; }

		// Token: 0x060025C1 RID: 9665 RVA: 0x000090CD File Offset: 0x000072CD
		public StringConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x060025C2 RID: 9666 RVA: 0x0001B795 File Offset: 0x00019995
		public override void SetString(string newValue)
		{
			this.value = newValue;
		}

		// Token: 0x060025C3 RID: 9667 RVA: 0x0001B79E File Offset: 0x0001999E
		public override string GetString()
		{
			return this.value;
		}
	}
}
