using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000243 RID: 579
	public class EnumMaskAttribute : PropertyAttribute
	{
		// Token: 0x06000AE7 RID: 2791 RVA: 0x00008C98 File Offset: 0x00006E98
		public EnumMaskAttribute(Type enumType)
		{
			this.enumType = enumType;
		}

		// Token: 0x04000EAC RID: 3756
		public Type enumType;
	}
}
