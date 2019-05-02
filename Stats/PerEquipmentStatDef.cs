using System;
using System.Collections.Generic;

namespace RoR2.Stats
{
	// Token: 0x020004F8 RID: 1272
	public class PerEquipmentStatDef
	{
		// Token: 0x06001CDB RID: 7387 RVA: 0x0008E068 File Offset: 0x0008C268
		public static void RegisterStatDefs()
		{
			foreach (PerEquipmentStatDef perEquipmentStatDef in PerEquipmentStatDef.instancesList)
			{
				foreach (EquipmentIndex equipmentIndex in EquipmentCatalog.allEquipment)
				{
					StatDef statDef = StatDef.Register(perEquipmentStatDef.prefix + "." + equipmentIndex.ToString(), perEquipmentStatDef.recordType, perEquipmentStatDef.dataType, 0.0, perEquipmentStatDef.displayValueFormatter);
					perEquipmentStatDef.keyToStatDef[(int)equipmentIndex] = statDef;
				}
			}
		}

		// Token: 0x06001CDC RID: 7388 RVA: 0x0008E138 File Offset: 0x0008C338
		private PerEquipmentStatDef(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
			this.displayValueFormatter = (displayValueFormatter ?? new StatDef.DisplayValueFormatterDelegate(StatDef.DefaultDisplayValueFormatter));
		}

		// Token: 0x06001CDD RID: 7389 RVA: 0x0008E188 File Offset: 0x0008C388
		private static PerEquipmentStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			PerEquipmentStatDef perEquipmentStatDef = new PerEquipmentStatDef(prefix, recordType, dataType, displayValueFormatter);
			PerEquipmentStatDef.instancesList.Add(perEquipmentStatDef);
			return perEquipmentStatDef;
		}

		// Token: 0x06001CDE RID: 7390 RVA: 0x00015321 File Offset: 0x00013521
		public StatDef FindStatDef(EquipmentIndex key)
		{
			return this.keyToStatDef[(int)key];
		}

		// Token: 0x04001F04 RID: 7940
		private readonly string prefix;

		// Token: 0x04001F05 RID: 7941
		private readonly StatRecordType recordType;

		// Token: 0x04001F06 RID: 7942
		private readonly StatDataType dataType;

		// Token: 0x04001F07 RID: 7943
		private readonly StatDef[] keyToStatDef = new StatDef[27];

		// Token: 0x04001F08 RID: 7944
		private StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x04001F09 RID: 7945
		private static readonly List<PerEquipmentStatDef> instancesList = new List<PerEquipmentStatDef>();

		// Token: 0x04001F0A RID: 7946
		public static readonly PerEquipmentStatDef totalTimeHeld = PerEquipmentStatDef.Register("totalTimeHeld", StatRecordType.Sum, StatDataType.Double, new StatDef.DisplayValueFormatterDelegate(StatDef.TimeMMSSDisplayValueFormatter));

		// Token: 0x04001F0B RID: 7947
		public static readonly PerEquipmentStatDef totalTimesFired = PerEquipmentStatDef.Register("totalTimesFired", StatRecordType.Sum, StatDataType.ULong, null);
	}
}
