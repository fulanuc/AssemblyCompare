using System;
using System.Collections.Generic;

namespace RoR2.Stats
{
	// Token: 0x020004F7 RID: 1271
	public class PerItemStatDef
	{
		// Token: 0x06001CD6 RID: 7382 RVA: 0x0008DF78 File Offset: 0x0008C178
		public static void RegisterStatDefs()
		{
			foreach (PerItemStatDef perItemStatDef in PerItemStatDef.instancesList)
			{
				foreach (ItemIndex itemIndex in ItemCatalog.allItems)
				{
					StatDef statDef = StatDef.Register(perItemStatDef.prefix + "." + itemIndex.ToString(), perItemStatDef.recordType, perItemStatDef.dataType, 0.0, null);
					perItemStatDef.keyToStatDef[(int)itemIndex] = statDef;
				}
			}
		}

		// Token: 0x06001CD7 RID: 7383 RVA: 0x000152B2 File Offset: 0x000134B2
		private PerItemStatDef(string prefix, StatRecordType recordType, StatDataType dataType)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
		}

		// Token: 0x06001CD8 RID: 7384 RVA: 0x0008E044 File Offset: 0x0008C244
		private static PerItemStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType)
		{
			PerItemStatDef perItemStatDef = new PerItemStatDef(prefix, recordType, dataType);
			PerItemStatDef.instancesList.Add(perItemStatDef);
			return perItemStatDef;
		}

		// Token: 0x06001CD9 RID: 7385 RVA: 0x000152DC File Offset: 0x000134DC
		public StatDef FindStatDef(ItemIndex key)
		{
			return this.keyToStatDef[(int)key];
		}

		// Token: 0x04001EFD RID: 7933
		private readonly string prefix;

		// Token: 0x04001EFE RID: 7934
		private readonly StatRecordType recordType;

		// Token: 0x04001EFF RID: 7935
		private readonly StatDataType dataType;

		// Token: 0x04001F00 RID: 7936
		private readonly StatDef[] keyToStatDef = new StatDef[78];

		// Token: 0x04001F01 RID: 7937
		private static readonly List<PerItemStatDef> instancesList = new List<PerItemStatDef>();

		// Token: 0x04001F02 RID: 7938
		public static readonly PerItemStatDef totalCollected = PerItemStatDef.Register("totalCollected", StatRecordType.Sum, StatDataType.ULong);

		// Token: 0x04001F03 RID: 7939
		public static readonly PerItemStatDef highestCollected = PerItemStatDef.Register("highestCollected", StatRecordType.Max, StatDataType.ULong);
	}
}
