using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace RoR2.Stats
{
	// Token: 0x02000508 RID: 1288
	public class PerStageStatDef
	{
		// Token: 0x06001D47 RID: 7495 RVA: 0x0008EF38 File Offset: 0x0008D138
		public static void RegisterStatDefs()
		{
			foreach (PerStageStatDef perStageStatDef in PerStageStatDef.instancesList)
			{
				foreach (SceneDef sceneDef in SceneCatalog.allSceneDefs)
				{
					string sceneName = sceneDef.sceneName;
					StatDef value = StatDef.Register(perStageStatDef.prefix + "." + sceneName, perStageStatDef.recordType, perStageStatDef.dataType, 0.0, perStageStatDef.displayValueFormatter);
					perStageStatDef.keyToStatDef[sceneName] = value;
				}
			}
		}

		// Token: 0x06001D48 RID: 7496 RVA: 0x00015804 File Offset: 0x00013A04
		private PerStageStatDef(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
			this.displayValueFormatter = (displayValueFormatter ?? new StatDef.DisplayValueFormatterDelegate(StatDef.DefaultDisplayValueFormatter));
		}

		// Token: 0x06001D49 RID: 7497 RVA: 0x0008F000 File Offset: 0x0008D200
		[NotNull]
		private static PerStageStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			PerStageStatDef perStageStatDef = new PerStageStatDef(prefix, recordType, dataType, displayValueFormatter);
			PerStageStatDef.instancesList.Add(perStageStatDef);
			return perStageStatDef;
		}

		// Token: 0x06001D4A RID: 7498 RVA: 0x0008F024 File Offset: 0x0008D224
		[CanBeNull]
		public StatDef FindStatDef(string key)
		{
			StatDef result;
			if (this.keyToStatDef.TryGetValue(key, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x04001F4A RID: 8010
		private readonly string prefix;

		// Token: 0x04001F4B RID: 8011
		private readonly StatRecordType recordType;

		// Token: 0x04001F4C RID: 8012
		private readonly StatDataType dataType;

		// Token: 0x04001F4D RID: 8013
		private readonly Dictionary<string, StatDef> keyToStatDef = new Dictionary<string, StatDef>();

		// Token: 0x04001F4E RID: 8014
		private StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x04001F4F RID: 8015
		private static readonly List<PerStageStatDef> instancesList = new List<PerStageStatDef>();

		// Token: 0x04001F50 RID: 8016
		public static readonly PerStageStatDef totalTimesVisited = PerStageStatDef.Register("totalTimesVisited", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F51 RID: 8017
		public static readonly PerStageStatDef totalTimesCleared = PerStageStatDef.Register("totalTimesCleared", StatRecordType.Sum, StatDataType.ULong, null);
	}
}
