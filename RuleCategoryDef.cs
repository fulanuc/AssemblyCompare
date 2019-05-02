using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200047D RID: 1149
	public class RuleCategoryDef
	{
		// Token: 0x17000263 RID: 611
		// (get) Token: 0x060019BE RID: 6590 RVA: 0x00013109 File Offset: 0x00011309
		public bool isHidden
		{
			get
			{
				Func<bool> func = this.hiddenTest;
				return func != null && func();
			}
		}

		// Token: 0x04001CFA RID: 7418
		public int position;

		// Token: 0x04001CFB RID: 7419
		public string displayToken;

		// Token: 0x04001CFC RID: 7420
		public string emptyTipToken;

		// Token: 0x04001CFD RID: 7421
		public Color color;

		// Token: 0x04001CFE RID: 7422
		public List<RuleDef> children = new List<RuleDef>();

		// Token: 0x04001CFF RID: 7423
		public Func<bool> hiddenTest;
	}
}
