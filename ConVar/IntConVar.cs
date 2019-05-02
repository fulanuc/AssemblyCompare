using System;

namespace RoR2.ConVar
{
	// Token: 0x02000690 RID: 1680
	public class IntConVar : BaseConVar
	{
		// Token: 0x17000336 RID: 822
		// (get) Token: 0x060025B5 RID: 9653 RVA: 0x0001B748 File Offset: 0x00019948
		// (set) Token: 0x060025B6 RID: 9654 RVA: 0x0001B750 File Offset: 0x00019950
		public int value { get; protected set; }

		// Token: 0x060025B7 RID: 9655 RVA: 0x000090CD File Offset: 0x000072CD
		public IntConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x060025B8 RID: 9656 RVA: 0x000B1770 File Offset: 0x000AF970
		public override void SetString(string newValue)
		{
			int value;
			if (TextSerialization.TryParseInvariant(newValue, out value))
			{
				this.value = value;
			}
		}

		// Token: 0x060025B9 RID: 9657 RVA: 0x0001B759 File Offset: 0x00019959
		public override string GetString()
		{
			return TextSerialization.ToStringInvariant(this.value);
		}
	}
}
