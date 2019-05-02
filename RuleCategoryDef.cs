using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200048A RID: 1162
	public class RuleCategoryDef
	{
		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06001A20 RID: 6688 RVA: 0x00013637 File Offset: 0x00011837
		public bool isHidden
		{
			get
			{
				Func<bool> func = this.hiddenTest;
				return func != null && func();
			}
		}

		// Token: 0x04001D31 RID: 7473
		public int position;

		// Token: 0x04001D32 RID: 7474
		public string displayToken;

		// Token: 0x04001D33 RID: 7475
		public string emptyTipToken;

		// Token: 0x04001D34 RID: 7476
		public Color color;

		// Token: 0x04001D35 RID: 7477
		public List<RuleDef> children = new List<RuleDef>();

		// Token: 0x04001D36 RID: 7478
		public Func<bool> hiddenTest;
	}
}
