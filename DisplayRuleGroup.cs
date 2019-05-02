using System;

namespace RoR2
{
	// Token: 0x0200022C RID: 556
	[Serializable]
	public struct DisplayRuleGroup
	{
		// Token: 0x06000AB3 RID: 2739 RVA: 0x00048DC8 File Offset: 0x00046FC8
		public void AddDisplayRule(ItemDisplayRule itemDisplayRule)
		{
			int num = ((this.rules != null) ? this.rules.Length : 0) + 1;
			ItemDisplayRule[] array = new ItemDisplayRule[num];
			if (num != 0 && this.rules != null)
			{
				this.rules.CopyTo(array, 0);
			}
			array[num - 1] = itemDisplayRule;
			this.rules = array;
		}

		// Token: 0x04000E29 RID: 3625
		public ItemDisplayRule[] rules;
	}
}
