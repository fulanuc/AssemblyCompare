using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2.Stats
{
	// Token: 0x02000502 RID: 1282
	public class StatDef
	{
		// Token: 0x06001D27 RID: 7463 RVA: 0x0008E528 File Offset: 0x0008C728
		[CanBeNull]
		public static StatDef Find(string statName)
		{
			StatDef result;
			StatDef.nameToStatDef.TryGetValue(statName, out result);
			return result;
		}

		// Token: 0x06001D28 RID: 7464 RVA: 0x0008E544 File Offset: 0x0008C744
		private StatDef(string name, StatRecordType recordType, StatDataType dataType, double pointValue, StatDef.DisplayValueFormatterDelegate displayValueFormatter)
		{
			this.name = name;
			this.recordType = recordType;
			this.dataType = dataType;
			this.pointValue = pointValue;
			this.displayValueFormatter = displayValueFormatter;
			this.displayToken = "STATNAME_" + name.ToUpper(CultureInfo.InvariantCulture);
		}

		// Token: 0x06001D2A RID: 7466 RVA: 0x000156BE File Offset: 0x000138BE
		[SystemInitializer(new Type[]
		{
			typeof(BodyCatalog),
			typeof(SceneCatalog)
		})]
		private static void Init()
		{
			BodyCatalog.availability.CallWhenAvailable(delegate
			{
				StatDef.bodyNames = (from gameObject in BodyCatalog.allBodyPrefabs
				select gameObject.name).ToArray<string>();
				PerBodyStatDef.RegisterStatDefs(StatDef.bodyNames);
				PerItemStatDef.RegisterStatDefs();
				PerEquipmentStatDef.RegisterStatDefs();
				PerStageStatDef.RegisterStatDefs();
			});
		}

		// Token: 0x06001D2B RID: 7467 RVA: 0x0008E980 File Offset: 0x0008CB80
		public static StatDef Register(string name, StatRecordType recordType, StatDataType dataType, double pointValue, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			if (displayValueFormatter == null)
			{
				displayValueFormatter = new StatDef.DisplayValueFormatterDelegate(StatDef.DefaultDisplayValueFormatter);
			}
			StatDef statDef = new StatDef(name, recordType, dataType, pointValue, displayValueFormatter)
			{
				index = StatDef.allStatDefs.Count
			};
			StatDef.allStatDefs.Add(statDef);
			StatDef.nameToStatDef.Add(statDef.name, statDef);
			return statDef;
		}

		// Token: 0x06001D2C RID: 7468 RVA: 0x000156E9 File Offset: 0x000138E9
		public static string DefaultDisplayValueFormatter(ref StatField statField)
		{
			return statField.ToLocalNumeric();
		}

		// Token: 0x06001D2D RID: 7469 RVA: 0x0008E9D8 File Offset: 0x0008CBD8
		public static string TimeMMSSDisplayValueFormatter(ref StatField statField)
		{
			StatDataType statDataType = statField.dataType;
			ulong num;
			if (statDataType != StatDataType.ULong)
			{
				if (statDataType != StatDataType.Double)
				{
					throw new ArgumentOutOfRangeException();
				}
				num = (ulong)statField.GetDoubleValue();
			}
			else
			{
				num = statField.GetULongValue();
			}
			ulong num2 = num / 60UL;
			ulong num3 = num - num2 * 60UL;
			return string.Format("{0:00}:{1:00}", num2, num3);
		}

		// Token: 0x06001D2E RID: 7470 RVA: 0x0008EA38 File Offset: 0x0008CC38
		public static string DistanceMarathonsDisplayValueFormatter(ref StatField statField)
		{
			StatDataType statDataType = statField.dataType;
			double num;
			if (statDataType != StatDataType.ULong)
			{
				if (statDataType != StatDataType.Double)
				{
					throw new ArgumentOutOfRangeException();
				}
				num = statField.GetDoubleValue();
			}
			else
			{
				num = statField.GetULongValue();
			}
			return string.Format(Language.GetString("STAT_VALUE_MARATHONS_FORMAT"), num * 2.3699E-05);
		}

		// Token: 0x04001EF8 RID: 7928
		public static readonly List<StatDef> allStatDefs = new List<StatDef>();

		// Token: 0x04001EF9 RID: 7929
		private static readonly Dictionary<string, StatDef> nameToStatDef = new Dictionary<string, StatDef>();

		// Token: 0x04001EFA RID: 7930
		public int index;

		// Token: 0x04001EFB RID: 7931
		public readonly string name;

		// Token: 0x04001EFC RID: 7932
		public readonly string displayToken;

		// Token: 0x04001EFD RID: 7933
		public readonly StatRecordType recordType;

		// Token: 0x04001EFE RID: 7934
		public readonly StatDataType dataType;

		// Token: 0x04001EFF RID: 7935
		public double pointValue;

		// Token: 0x04001F00 RID: 7936
		public readonly StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x04001F01 RID: 7937
		public static readonly StatDef totalGamesPlayed = StatDef.Register("totalGamesPlayed", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F02 RID: 7938
		public static readonly StatDef totalTimeAlive = StatDef.Register("totalTimeAlive", StatRecordType.Sum, StatDataType.Double, 1.0, new StatDef.DisplayValueFormatterDelegate(StatDef.TimeMMSSDisplayValueFormatter));

		// Token: 0x04001F03 RID: 7939
		public static readonly StatDef totalKills = StatDef.Register("totalKills", StatRecordType.Sum, StatDataType.ULong, 10.0, null);

		// Token: 0x04001F04 RID: 7940
		public static readonly StatDef totalDeaths = StatDef.Register("totalDeaths", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F05 RID: 7941
		public static readonly StatDef totalDamageDealt = StatDef.Register("totalDamageDealt", StatRecordType.Sum, StatDataType.ULong, 0.01, null);

		// Token: 0x04001F06 RID: 7942
		public static readonly StatDef totalDamageTaken = StatDef.Register("totalDamageTaken", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F07 RID: 7943
		public static readonly StatDef totalHealthHealed = StatDef.Register("totalHealthHealed", StatRecordType.Sum, StatDataType.ULong, 0.01, null);

		// Token: 0x04001F08 RID: 7944
		public static readonly StatDef highestDamageDealt = StatDef.Register("highestDamageDealt", StatRecordType.Max, StatDataType.ULong, 1.0, null);

		// Token: 0x04001F09 RID: 7945
		public static readonly StatDef highestLevel = StatDef.Register("highestLevel", StatRecordType.Max, StatDataType.ULong, 100.0, null);

		// Token: 0x04001F0A RID: 7946
		public static readonly StatDef goldCollected = StatDef.Register("totalGoldCollected", StatRecordType.Sum, StatDataType.ULong, 1.0, null);

		// Token: 0x04001F0B RID: 7947
		public static readonly StatDef maxGoldCollected = StatDef.Register("maxGoldCollected", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F0C RID: 7948
		public static readonly StatDef totalDistanceTraveled = StatDef.Register("totalDistanceTraveled", StatRecordType.Sum, StatDataType.Double, 0.01, new StatDef.DisplayValueFormatterDelegate(StatDef.DistanceMarathonsDisplayValueFormatter));

		// Token: 0x04001F0D RID: 7949
		public static readonly StatDef totalItemsCollected = StatDef.Register("totalItemsCollected", StatRecordType.Sum, StatDataType.ULong, 110.0, null);

		// Token: 0x04001F0E RID: 7950
		public static readonly StatDef highestItemsCollected = StatDef.Register("highestItemsCollected", StatRecordType.Max, StatDataType.ULong, 10.0, null);

		// Token: 0x04001F0F RID: 7951
		public static readonly StatDef totalStagesCompleted = StatDef.Register("totalStagesCompleted", StatRecordType.Sum, StatDataType.ULong, 100.0, null);

		// Token: 0x04001F10 RID: 7952
		public static readonly StatDef highestStagesCompleted = StatDef.Register("highestStagesCompleted", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F11 RID: 7953
		public static readonly StatDef totalPurchases = StatDef.Register("totalPurchases", StatRecordType.Sum, StatDataType.ULong, 35.0, null);

		// Token: 0x04001F12 RID: 7954
		public static readonly StatDef highestPurchases = StatDef.Register("highestPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F13 RID: 7955
		public static readonly StatDef totalGoldPurchases = StatDef.Register("totalGoldPurchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F14 RID: 7956
		public static readonly StatDef highestGoldPurchases = StatDef.Register("highestGoldPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F15 RID: 7957
		public static readonly StatDef totalBloodPurchases = StatDef.Register("totalBloodPurchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F16 RID: 7958
		public static readonly StatDef highestBloodPurchases = StatDef.Register("highestBloodPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F17 RID: 7959
		public static readonly StatDef totalLunarPurchases = StatDef.Register("totalLunarPurchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F18 RID: 7960
		public static readonly StatDef highestLunarPurchases = StatDef.Register("highestLunarPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F19 RID: 7961
		public static readonly StatDef totalTier1Purchases = StatDef.Register("totalTier1Purchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F1A RID: 7962
		public static readonly StatDef highestTier1Purchases = StatDef.Register("highestTier1Purchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F1B RID: 7963
		public static readonly StatDef totalTier2Purchases = StatDef.Register("totalTier2Purchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F1C RID: 7964
		public static readonly StatDef highestTier2Purchases = StatDef.Register("highestTier2Purchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F1D RID: 7965
		public static readonly StatDef totalTier3Purchases = StatDef.Register("totalTier3Purchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F1E RID: 7966
		public static readonly StatDef highestTier3Purchases = StatDef.Register("highestTier3Purchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F1F RID: 7967
		public static readonly StatDef totalDronesPurchased = StatDef.Register("totalDronesPurchased", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F20 RID: 7968
		public static readonly StatDef totalGreenSoupsPurchased = StatDef.Register("totalGreenSoupsPurchased", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F21 RID: 7969
		public static readonly StatDef totalRedSoupsPurchased = StatDef.Register("totalRedSoupsPurchased", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F22 RID: 7970
		public static readonly StatDef suicideHermitCrabsAchievementProgress = StatDef.Register("suicideHermitCrabsAchievementProgress", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F23 RID: 7971
		public static readonly StatDef firstTeleporterCompleted = StatDef.Register("firstTeleporterCompleted", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001F24 RID: 7972
		private static string[] bodyNames;

		// Token: 0x02000503 RID: 1283
		// (Invoke) Token: 0x06001D30 RID: 7472
		public delegate string DisplayValueFormatterDelegate(ref StatField statField);
	}
}
