using System;

namespace RoR2.ConVar
{
	// Token: 0x0200067E RID: 1662
	public class IntConVar : BaseConVar
	{
		// Token: 0x17000324 RID: 804
		// (get) Token: 0x0600251E RID: 9502 RVA: 0x0001B015 File Offset: 0x00019215
		// (set) Token: 0x0600251F RID: 9503 RVA: 0x0001B01D File Offset: 0x0001921D
		public int value { get; protected set; }

		// Token: 0x06002520 RID: 9504 RVA: 0x000090A8 File Offset: 0x000072A8
		public IntConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x06002521 RID: 9505 RVA: 0x000B0080 File Offset: 0x000AE280
		public override void SetString(string newValue)
		{
			int value;
			if (TextSerialization.TryParseInvariant(newValue, out value))
			{
				this.value = value;
			}
		}

		// Token: 0x06002522 RID: 9506 RVA: 0x0001B026 File Offset: 0x00019226
		public override string GetString()
		{
			return TextSerialization.ToStringInvariant(this.value);
		}
	}
}
