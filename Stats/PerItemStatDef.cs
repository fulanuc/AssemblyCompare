using System;
using System.Collections.Generic;

namespace RoR2.Stats
{
	// Token: 0x02000506 RID: 1286
	public class PerItemStatDef
	{
		// Token: 0x06001D3D RID: 7485 RVA: 0x0008ED04 File Offset: 0x0008CF04
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

		// Token: 0x06001D3E RID: 7486 RVA: 0x0001575B File Offset: 0x0001395B
		private PerItemStatDef(string prefix, StatRecordType recordType, StatDataType dataType)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
		}

		// Token: 0x06001D3F RID: 7487 RVA: 0x0008EDD0 File Offset: 0x0008CFD0
		private static PerItemStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType)
		{
			PerItemStatDef perItemStatDef = new PerItemStatDef(prefix, recordType, dataType);
			PerItemStatDef.instancesList.Add(perItemStatDef);
			return perItemStatDef;
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x00015785 File Offset: 0x00013985
		public StatDef FindStatDef(ItemIndex key)
		{
			return this.keyToStatDef[(int)key];
		}

		// Token: 0x04001F3B RID: 7995
		private readonly string prefix;

		// Token: 0x04001F3C RID: 7996
		private readonly StatRecordType recordType;

		// Token: 0x04001F3D RID: 7997
		private readonly StatDataType dataType;

		// Token: 0x04001F3E RID: 7998
		private readonly StatDef[] keyToStatDef = new StatDef[78];

		// Token: 0x04001F3F RID: 7999
		private static readonly List<PerItemStatDef> instancesList = new List<PerItemStatDef>();

		// Token: 0x04001F40 RID: 8000
		public static readonly PerItemStatDef totalCollected = PerItemStatDef.Register("totalCollected", StatRecordType.Sum, StatDataType.ULong);

		// Token: 0x04001F41 RID: 8001
		public static readonly PerItemStatDef highestCollected = PerItemStatDef.Register("highestCollected", StatRecordType.Max, StatDataType.ULong);
	}
}
