using System;
using System.Collections.Generic;

namespace RoR2.Stats
{
	// Token: 0x02000505 RID: 1285
	public class PerBodyStatDef
	{
		// Token: 0x06001D38 RID: 7480 RVA: 0x0008EC00 File Offset: 0x0008CE00
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

		// Token: 0x06001D39 RID: 7481 RVA: 0x000156FD File Offset: 0x000138FD
		private PerBodyStatDef(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
			this.displayValueFormatter = displayValueFormatter;
		}

		// Token: 0x06001D3A RID: 7482 RVA: 0x0008ECC0 File Offset: 0x0008CEC0
		private static PerBodyStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			PerBodyStatDef perBodyStatDef = new PerBodyStatDef(prefix, recordType, dataType, displayValueFormatter);
			PerBodyStatDef.instancesList.Add(perBodyStatDef);
			return perBodyStatDef;
		}

		// Token: 0x06001D3B RID: 7483 RVA: 0x0008ECE4 File Offset: 0x0008CEE4
		public StatDef FindStatDef(string bodyName)
		{
			StatDef result;
			this.bodyNameToStatDefDictionary.TryGetValue(bodyName, out result);
			return result;
		}

		// Token: 0x04001F28 RID: 7976
		private readonly string prefix;

		// Token: 0x04001F29 RID: 7977
		private readonly StatRecordType recordType;

		// Token: 0x04001F2A RID: 7978
		private readonly StatDataType dataType;

		// Token: 0x04001F2B RID: 7979
		private readonly StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x04001F2C RID: 7980
		private readonly Dictionary<string, StatDef> bodyNameToStatDefDictionary = new Dictionary<string, StatDef>();

		// Token: 0x04001F2D RID: 7981
		private static readonly List<PerBodyStatDef> instancesList = new List<PerBodyStatDef>();

		// Token: 0x04001F2E RID: 7982
		public static readonly PerBodyStatDef totalTimeAlive = PerBodyStatDef.Register("totalTimeAlive", StatRecordType.Sum, StatDataType.Double, null);

		// Token: 0x04001F2F RID: 7983
		public static readonly PerBodyStatDef totalWins = PerBodyStatDef.Register("totalWins", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F30 RID: 7984
		public static readonly PerBodyStatDef longestRun = PerBodyStatDef.Register("longestRun", StatRecordType.Max, StatDataType.Double, new StatDef.DisplayValueFormatterDelegate(StatDef.TimeMMSSDisplayValueFormatter));

		// Token: 0x04001F31 RID: 7985
		public static readonly PerBodyStatDef damageDealtTo = PerBodyStatDef.Register("damageDealtTo", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F32 RID: 7986
		public static readonly PerBodyStatDef damageDealtAs = PerBodyStatDef.Register("damageDealtAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F33 RID: 7987
		public static readonly PerBodyStatDef damageTakenFrom = PerBodyStatDef.Register("damageTakenFrom", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F34 RID: 7988
		public static readonly PerBodyStatDef damageTakenAs = PerBodyStatDef.Register("damageTakenAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F35 RID: 7989
		public static readonly PerBodyStatDef killsAgainst = PerBodyStatDef.Register("killsAgainst", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F36 RID: 7990
		public static readonly PerBodyStatDef killsAgainstElite = PerBodyStatDef.Register("killsAgainstElite", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F37 RID: 7991
		public static readonly PerBodyStatDef deathsFrom = PerBodyStatDef.Register("deathsFrom", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F38 RID: 7992
		public static readonly PerBodyStatDef killsAs = PerBodyStatDef.Register("killsAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F39 RID: 7993
		public static readonly PerBodyStatDef deathsAs = PerBodyStatDef.Register("deathsAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F3A RID: 7994
		public static readonly PerBodyStatDef timesPicked = PerBodyStatDef.Register("timesPicked", StatRecordType.Sum, StatDataType.ULong, null);
	}
}
