using System;
using System.Collections.Generic;

namespace RoR2.Stats
{
	// Token: 0x020004F6 RID: 1270
	public class PerBodyStatDef
	{
		// Token: 0x06001CD1 RID: 7377 RVA: 0x0008DE74 File Offset: 0x0008C074
		public static void RegisterStatDefs(string[] bodyNames)
		{
			foreach (PerBodyStatDef perBodyStatDef in PerBodyStatDef.instancesList)
			{
				foreach (string text in bodyNames)
				{
					StatDef value = StatDef.Register(perBodyStatDef.prefix + "." + text, perBodyStatDef.recordType, perBodyStatDef.dataType, 0.0, perBodyStatDef.displayValueFormatter);
					perBodyStatDef.bodyNameToStatDefDictionary.Add(text, value);
					perBodyStatDef.bodyNameToStatDefDictionary.Add(text + "(Clone)", value);
				}
			}
		}

		// Token: 0x06001CD2 RID: 7378 RVA: 0x00015254 File Offset: 0x00013454
		private PerBodyStatDef(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
			this.displayValueFormatter = displayValueFormatter;
		}

		// Token: 0x06001CD3 RID: 7379 RVA: 0x0008DF34 File Offset: 0x0008C134
		private static PerBodyStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			PerBodyStatDef perBodyStatDef = new PerBodyStatDef(prefix, recordType, dataType, displayValueFormatter);
			PerBodyStatDef.instancesList.Add(perBodyStatDef);
			return perBodyStatDef;
		}

		// Token: 0x06001CD4 RID: 7380 RVA: 0x0008DF58 File Offset: 0x0008C158
		public StatDef FindStatDef(string bodyName)
		{
			StatDef result;
			this.bodyNameToStatDefDictionary.TryGetValue(bodyName, out result);
			return result;
		}

		// Token: 0x04001EEA RID: 7914
		private readonly string prefix;

		// Token: 0x04001EEB RID: 7915
		private readonly StatRecordType recordType;

		// Token: 0x04001EEC RID: 7916
		private readonly StatDataType dataType;

		// Token: 0x04001EED RID: 7917
		private readonly StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x04001EEE RID: 7918
		private readonly Dictionary<string, StatDef> bodyNameToStatDefDictionary = new Dictionary<string, StatDef>();

		// Token: 0x04001EEF RID: 7919
		private static readonly List<PerBodyStatDef> instancesList = new List<PerBodyStatDef>();

		// Token: 0x04001EF0 RID: 7920
		public static readonly PerBodyStatDef totalTimeAlive = PerBodyStatDef.Register("totalTimeAlive", StatRecordType.Sum, StatDataType.Double, null);

		// Token: 0x04001EF1 RID: 7921
		public static readonly PerBodyStatDef totalWins = PerBodyStatDef.Register("totalWins", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EF2 RID: 7922
		public static readonly PerBodyStatDef longestRun = PerBodyStatDef.Register("longestRun", StatRecordType.Max, StatDataType.Double, new StatDef.DisplayValueFormatterDelegate(StatDef.TimeMMSSDisplayValueFormatter));

		// Token: 0x04001EF3 RID: 7923
		public static readonly PerBodyStatDef damageDealtTo = PerBodyStatDef.Register("damageDealtTo", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EF4 RID: 7924
		public static readonly PerBodyStatDef damageDealtAs = PerBodyStatDef.Register("damageDealtAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EF5 RID: 7925
		public static readonly PerBodyStatDef damageTakenFrom = PerBodyStatDef.Register("damageTakenFrom", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EF6 RID: 7926
		public static readonly PerBodyStatDef damageTakenAs = PerBodyStatDef.Register("damageTakenAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EF7 RID: 7927
		public static readonly PerBodyStatDef killsAgainst = PerBodyStatDef.Register("killsAgainst", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EF8 RID: 7928
		public static readonly PerBodyStatDef killsAgainstElite = PerBodyStatDef.Register("killsAgainstElite", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EF9 RID: 7929
		public static readonly PerBodyStatDef deathsFrom = PerBodyStatDef.Register("deathsFrom", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EFA RID: 7930
		public static readonly PerBodyStatDef killsAs = PerBodyStatDef.Register("killsAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EFB RID: 7931
		public static readonly PerBodyStatDef deathsAs = PerBodyStatDef.Register("deathsAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EFC RID: 7932
		public static readonly PerBodyStatDef timesPicked = PerBodyStatDef.Register("timesPicked", StatRecordType.Sum, StatDataType.ULong, null);
	}
}
