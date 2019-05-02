using System;

namespace RoR2.ConVar
{
	// Token: 0x0200067F RID: 1663
	public class FloatConVar : BaseConVar
	{
		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06002523 RID: 9507 RVA: 0x0001B033 File Offset: 0x00019233
		// (set) Token: 0x06002524 RID: 9508 RVA: 0x0001B03B File Offset: 0x0001923B
		public float value { get; protected set; }

		// Token: 0x06002525 RID: 9509 RVA: 0x000090A8 File Offset: 0x000072A8
		public FloatConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x06002526 RID: 9510 RVA: 0x000B00A0 File Offset: 0x000AE2A0
		public override void SetString(string newValue)
		{
			float num;
			if (TextSerialization.TryParseInvariant(newValue, out num) && !float.IsNaN(num) && !float.IsInfinity(num))
			{
				this.value = num;
			}
		}

		// Token: 0x06002527 RID: 9511 RVA: 0x0001B044 File Offset: 0x00019244
		public override string GetString()
		{
			return TextSerialization.ToStringInvariant(this.value);
		}
	}
}
