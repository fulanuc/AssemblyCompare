using System;
using System.Collections.Generic;

namespace RoR2.Stats
{
	// Token: 0x02000507 RID: 1287
	public class PerEquipmentStatDef
	{
		// Token: 0x06001D42 RID: 7490 RVA: 0x0008EDF4 File Offset: 0x0008CFF4
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

		// Token: 0x06001D43 RID: 7491 RVA: 0x0008EEC4 File Offset: 0x0008D0C4
		private PerEquipmentStatDef(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
			this.displayValueFormatter = (displayValueFormatter ?? new StatDef.DisplayValueFormatterDelegate(StatDef.DefaultDisplayValueFormatter));
		}

		// Token: 0x06001D44 RID: 7492 RVA: 0x0008EF14 File Offset: 0x0008D114
		private static PerEquipmentStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			PerEquipmentStatDef perEquipmentStatDef = new PerEquipmentStatDef(prefix, recordType, dataType, displayValueFormatter);
			PerEquipmentStatDef.instancesList.Add(perEquipmentStatDef);
			return perEquipmentStatDef;
		}

		// Token: 0x06001D45 RID: 7493 RVA: 0x000157CA File Offset: 0x000139CA
		public StatDef FindStatDef(EquipmentIndex key)
		{
			return this.keyToStatDef[(int)key];
		}

		// Token: 0x04001F42 RID: 8002
		private readonly string prefix;

		// Token: 0x04001F43 RID: 8003
		private readonly StatRecordType recordType;

		// Token: 0x04001F44 RID: 8004
		private readonly StatDataType dataType;

		// Token: 0x04001F45 RID: 8005
		private readonly StatDef[] keyToStatDef = new StatDef[27];

		// Token: 0x04001F46 RID: 8006
		private StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x04001F47 RID: 8007
		private static readonly List<PerEquipmentStatDef> instancesList = new List<PerEquipmentStatDef>();

		// Token: 0x04001F48 RID: 8008
		public static readonly PerEquipmentStatDef totalTimeHeld = PerEquipmentStatDef.Register("totalTimeHeld", StatRecordType.Sum, StatDataType.Double, new StatDef.DisplayValueFormatterDelegate(StatDef.TimeMMSSDisplayValueFormatter));

		// Token: 0x04001F49 RID: 8009
		public static readonly PerEquipmentStatDef totalTimesFired = PerEquipmentStatDef.Register("totalTimesFired", StatRecordType.Sum, StatDataType.ULong, null);
	}
}
