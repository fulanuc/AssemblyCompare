using System;

namespace RoR2.ConVar
{
	// Token: 0x0200068F RID: 1679
	public class BoolConVar : BaseConVar
	{
		// Token: 0x17000335 RID: 821
		// (get) Token: 0x060025AF RID: 9647 RVA: 0x0001B719 File Offset: 0x00019919
		// (set) Token: 0x060025B0 RID: 9648 RVA: 0x0001B721 File Offset: 0x00019921
		public bool value { get; protected set; }

		// Token: 0x060025B1 RID: 9649 RVA: 0x000090CD File Offset: 0x000072CD
		public BoolConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x060025B2 RID: 9650 RVA: 0x0001B72A File Offset: 0x0001992A
		public void SetBool(bool newValue)
		{
			this.value = newValue;
		}

		// Token: 0x060025B3 RID: 9651 RVA: 0x000B174C File Offset: 0x000AF94C
		public override void SetString(string newValue)
		{
			int num;
			if (TextSerialization.TryParseInvariant(newValue, out num))
			{
				this.value = (num != 0);
			}
		}

		// Token: 0x060025B4 RID: 9652 RVA: 0x0001B733 File Offset: 0x00019933
		public override string GetString()
		{
			if (!this.value)
			{
				return "0";
			}
			return "1";
		}
	}
}
