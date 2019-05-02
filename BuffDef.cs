using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000209 RID: 521
	public class BuffDef
	{
		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000A25 RID: 2597 RVA: 0x000082C8 File Offset: 0x000064C8
		public bool isElite
		{
			get
			{
				return this.eliteIndex != EliteIndex.None;
			}
		}

		// Token: 0x04000D8F RID: 3471
		public BuffIndex buffIndex;

		// Token: 0x04000D90 RID: 3472
		public string iconPath = "Textures/ItemIcons/texNullIcon";

		// Token: 0x04000D91 RID: 3473
		public Color buffColor = Color.white;

		// Token: 0x04000D92 RID: 3474
		public bool canStack;

		// Token: 0x04000D93 RID: 3475
		public EliteIndex eliteIndex = EliteIndex.None;
	}
}
