using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002FC RID: 764
	public class GenericDisplayNameProvider : MonoBehaviour, IDisplayNameProvider
	{
		// Token: 0x06000F81 RID: 3969 RVA: 0x0000BEC0 File Offset: 0x0000A0C0
		public string GetDisplayName()
		{
			return Language.GetString(this.displayToken);
		}

		// Token: 0x04001391 RID: 5009
		public string displayToken;
	}
}
