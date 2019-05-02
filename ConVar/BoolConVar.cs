using System;

namespace RoR2.ConVar
{
	// Token: 0x0200067D RID: 1661
	public class BoolConVar : BaseConVar
	{
		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06002518 RID: 9496 RVA: 0x0001AFE6 File Offset: 0x000191E6
		// (set) Token: 0x06002519 RID: 9497 RVA: 0x0001AFEE File Offset: 0x000191EE
		public bool value { get; protected set; }

		// Token: 0x0600251A RID: 9498 RVA: 0x000090A8 File Offset: 0x000072A8
		public BoolConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x0600251B RID: 9499 RVA: 0x0001AFF7 File Offset: 0x000191F7
		public void SetBool(bool newValue)
		{
			this.value = newValue;
		}

		// Token: 0x0600251C RID: 9500 RVA: 0x000B005C File Offset: 0x000AE25C
		public override void SetString(string newValue)
		{
			int num;
			if (TextSerialization.TryParseInvariant(newValue, out num))
			{
				this.value = (num != 0);
			}
		}

		// Token: 0x0600251D RID: 9501 RVA: 0x0001B000 File Offset: 0x00019200
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
