using System;

namespace RoR2.ConVar
{
	// Token: 0x02000691 RID: 1681
	public class FloatConVar : BaseConVar
	{
		// Token: 0x17000337 RID: 823
		// (get) Token: 0x060025BA RID: 9658 RVA: 0x0001B766 File Offset: 0x00019966
		// (set) Token: 0x060025BB RID: 9659 RVA: 0x0001B76E File Offset: 0x0001996E
		public float value { get; protected set; }

		// Token: 0x060025BC RID: 9660 RVA: 0x000090CD File Offset: 0x000072CD
		public FloatConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x060025BD RID: 9661 RVA: 0x000B1790 File Offset: 0x000AF990
		public override void SetString(string newValue)
		{
			float num;
			if (TextSerialization.TryParseInvariant(newValue, out num) && !float.IsNaN(num) && !float.IsInfinity(num))
			{
				this.value = num;
			}
		}

		// Token: 0x060025BE RID: 9662 RVA: 0x0001B777 File Offset: 0x00019977
		public override string GetString()
		{
			return TextSerialization.ToStringInvariant(this.value);
		}
	}
}
