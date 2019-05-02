using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace RoR2.Stats
{
	// Token: 0x020004F9 RID: 1273
	public class PerStageStatDef
	{
		// Token: 0x06001CE0 RID: 7392 RVA: 0x0008E1AC File Offset: 0x0008C3AC
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

		// Token: 0x06001CE1 RID: 7393 RVA: 0x0001535B File Offset: 0x0001355B
		private PerStageStatDef(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
			this.displayValueFormatter = (displayValueFormatter ?? new StatDef.DisplayValueFormatterDelegate(StatDef.DefaultDisplayValueFormatter));
		}

		// Token: 0x06001CE2 RID: 7394 RVA: 0x0008E274 File Offset: 0x0008C474
		[NotNull]
		private static PerStageStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			PerStageStatDef perStageStatDef = new PerStageStatDef(prefix, recordType, dataType, displayValueFormatter);
			PerStageStatDef.instancesList.Add(perStageStatDef);
			return perStageStatDef;
		}

		// Token: 0x06001CE3 RID: 7395 RVA: 0x0008E298 File Offset: 0x0008C498
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

		// Token: 0x04001F0C RID: 7948
		private readonly string prefix;

		// Token: 0x04001F0D RID: 7949
		private readonly StatRecordType recordType;

		// Token: 0x04001F0E RID: 7950
		private readonly StatDataType dataType;

		// Token: 0x04001F0F RID: 7951
		private readonly Dictionary<string, StatDef> keyToStatDef = new Dictionary<string, StatDef>();

		// Token: 0x04001F10 RID: 7952
		private StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x04001F11 RID: 7953
		private static readonly List<PerStageStatDef> instancesList = new List<PerStageStatDef>();

		// Token: 0x04001F12 RID: 7954
		public static readonly PerStageStatDef totalTimesVisited = PerStageStatDef.Register("totalTimesVisited", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F13 RID: 7955
		public static readonly PerStageStatDef totalTimesCleared = PerStageStatDef.Register("totalTimesCleared", StatRecordType.Sum, StatDataType.ULong, null);
	}
}
