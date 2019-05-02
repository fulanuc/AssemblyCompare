using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002FF RID: 767
	public class GenericDisplayNameProvider : MonoBehaviour, IDisplayNameProvider
	{
		// Token: 0x06000F91 RID: 3985 RVA: 0x0000BF6E File Offset: 0x0000A16E
		public string GetDisplayName()
		{
			return Language.GetString(this.displayToken);
		}

		// Token: 0x040013A8 RID: 5032
		public string displayToken;
	}
}
