using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2.Stats
{
	// Token: 0x020004F3 RID: 1267
	public class StatDef
	{
		// Token: 0x06001CC0 RID: 7360 RVA: 0x0008D7A0 File Offset: 0x0008B9A0
		[CanBeNull]
		public static StatDef Find(string statName)
		{
			StatDef result;
			StatDef.nameToStatDef.TryGetValue(statName, out result);
			return result;
		}

		// Token: 0x06001CC1 RID: 7361 RVA: 0x0008D7BC File Offset: 0x0008B9BC
		private StatDef(string name, StatRecordType recordType, StatDataType dataType, double pointValue, StatDef.DisplayValueFormatterDelegate displayValueFormatter)
		{
			this.name = name;
			this.recordType = recordType;
			this.dataType = dataType;
			this.pointValue = pointValue;
			this.displayValueFormatter = displayValueFormatter;
			this.displayToken = "STATNAME_" + name.ToUpper();
		}

		// Token: 0x06001CC3 RID: 7363 RVA: 0x0001520F File Offset: 0x0001340F
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

		// Token: 0x06001CC4 RID: 7364 RVA: 0x0008DBF4 File Offset: 0x0008BDF4
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

		// Token: 0x06001CC5 RID: 7365 RVA: 0x0001523A File Offset: 0x0001343A
		public static string DefaultDisplayValueFormatter(ref StatField statField)
		{
			return statField.ToString();
		}

		// Token: 0x06001CC6 RID: 7366 RVA: 0x0008DC4C File Offset: 0x0008BE4C
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

		// Token: 0x06001CC7 RID: 7367 RVA: 0x0008DCAC File Offset: 0x0008BEAC
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

		// Token: 0x04001EBA RID: 7866
		public static readonly List<StatDef> allStatDefs = new List<StatDef>();

		// Token: 0x04001EBB RID: 7867
		private static readonly Dictionary<string, StatDef> nameToStatDef = new Dictionary<string, StatDef>();

		// Token: 0x04001EBC RID: 7868
		public int index;

		// Token: 0x04001EBD RID: 7869
		public readonly string name;

		// Token: 0x04001EBE RID: 7870
		public readonly string displayToken;

		// Token: 0x04001EBF RID: 7871
		public readonly StatRecordType recordType;

		// Token: 0x04001EC0 RID: 7872
		public readonly StatDataType dataType;

		// Token: 0x04001EC1 RID: 7873
		public double pointValue;

		// Token: 0x04001EC2 RID: 7874
		public readonly StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x04001EC3 RID: 7875
		public static readonly StatDef totalGamesPlayed = StatDef.Register("totalGamesPlayed", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EC4 RID: 7876
		public static readonly StatDef totalTimeAlive = StatDef.Register("totalTimeAlive", StatRecordType.Sum, StatDataType.Double, 1.0, new StatDef.DisplayValueFormatterDelegate(StatDef.TimeMMSSDisplayValueFormatter));

		// Token: 0x04001EC5 RID: 7877
		public static readonly StatDef totalKills = StatDef.Register("totalKills", StatRecordType.Sum, StatDataType.ULong, 10.0, null);

		// Token: 0x04001EC6 RID: 7878
		public static readonly StatDef totalDeaths = StatDef.Register("totalDeaths", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EC7 RID: 7879
		public static readonly StatDef totalDamageDealt = StatDef.Register("totalDamageDealt", StatRecordType.Sum, StatDataType.ULong, 0.01, null);

		// Token: 0x04001EC8 RID: 7880
		public static readonly StatDef totalDamageTaken = StatDef.Register("totalDamageTaken", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EC9 RID: 7881
		public static readonly StatDef totalHealthHealed = StatDef.Register("totalHealthHealed", StatRecordType.Sum, StatDataType.ULong, 0.01, null);

		// Token: 0x04001ECA RID: 7882
		public static readonly StatDef highestDamageDealt = StatDef.Register("highestDamageDealt", StatRecordType.Max, StatDataType.ULong, 1.0, null);

		// Token: 0x04001ECB RID: 7883
		public static readonly StatDef highestLevel = StatDef.Register("highestLevel", StatRecordType.Max, StatDataType.ULong, 100.0, null);

		// Token: 0x04001ECC RID: 7884
		public static readonly StatDef goldCollected = StatDef.Register("totalGoldCollected", StatRecordType.Sum, StatDataType.ULong, 1.0, null);

		// Token: 0x04001ECD RID: 7885
		public static readonly StatDef maxGoldCollected = StatDef.Register("maxGoldCollected", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001ECE RID: 7886
		public static readonly StatDef totalDistanceTraveled = StatDef.Register("totalDistanceTraveled", StatRecordType.Sum, StatDataType.Double, 0.01, new StatDef.DisplayValueFormatterDelegate(StatDef.DistanceMarathonsDisplayValueFormatter));

		// Token: 0x04001ECF RID: 7887
		public static readonly StatDef totalItemsCollected = StatDef.Register("totalItemsCollected", StatRecordType.Sum, StatDataType.ULong, 110.0, null);

		// Token: 0x04001ED0 RID: 7888
		public static readonly StatDef highestItemsCollected = StatDef.Register("highestItemsCollected", StatRecordType.Max, StatDataType.ULong, 10.0, null);

		// Token: 0x04001ED1 RID: 7889
		public static readonly StatDef totalStagesCompleted = StatDef.Register("totalStagesCompleted", StatRecordType.Sum, StatDataType.ULong, 100.0, null);

		// Token: 0x04001ED2 RID: 7890
		public static readonly StatDef highestStagesCompleted = StatDef.Register("highestStagesCompleted", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001ED3 RID: 7891
		public static readonly StatDef totalPurchases = StatDef.Register("totalPurchases", StatRecordType.Sum, StatDataType.ULong, 35.0, null);

		// Token: 0x04001ED4 RID: 7892
		public static readonly StatDef highestPurchases = StatDef.Register("highestPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001ED5 RID: 7893
		public static readonly StatDef totalGoldPurchases = StatDef.Register("totalGoldPurchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001ED6 RID: 7894
		public static readonly StatDef highestGoldPurchases = StatDef.Register("highestGoldPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001ED7 RID: 7895
		public static readonly StatDef totalBloodPurchases = StatDef.Register("totalBloodPurchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001ED8 RID: 7896
		public static readonly StatDef highestBloodPurchases = StatDef.Register("highestBloodPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001ED9 RID: 7897
		public static readonly StatDef totalLunarPurchases = StatDef.Register("totalLunarPurchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EDA RID: 7898
		public static readonly StatDef highestLunarPurchases = StatDef.Register("highestLunarPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EDB RID: 7899
		public static readonly StatDef totalTier1Purchases = StatDef.Register("totalTier1Purchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EDC RID: 7900
		public static readonly StatDef highestTier1Purchases = StatDef.Register("highestTier1Purchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EDD RID: 7901
		public static readonly StatDef totalTier2Purchases = StatDef.Register("totalTier2Purchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EDE RID: 7902
		public static readonly StatDef highestTier2Purchases = StatDef.Register("highestTier2Purchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EDF RID: 7903
		public static readonly StatDef totalTier3Purchases = StatDef.Register("totalTier3Purchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE0 RID: 7904
		public static readonly StatDef highestTier3Purchases = StatDef.Register("highestTier3Purchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE1 RID: 7905
		public static readonly StatDef totalDronesPurchased = StatDef.Register("totalDronesPurchased", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE2 RID: 7906
		public static readonly StatDef totalGreenSoupsPurchased = StatDef.Register("totalGreenSoupsPurchased", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE3 RID: 7907
		public static readonly StatDef totalRedSoupsPurchased = StatDef.Register("totalRedSoupsPurchased", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE4 RID: 7908
		public static readonly StatDef suicideHermitCrabsAchievementProgress = StatDef.Register("suicideHermitCrabsAchievementProgress", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE5 RID: 7909
		public static readonly StatDef firstTeleporterCompleted = StatDef.Register("firstTeleporterCompleted", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE6 RID: 7910
		private static string[] bodyNames;

		// Token: 0x020004F4 RID: 1268
		// (Invoke) Token: 0x06001CC9 RID: 7369
		public delegate string DisplayValueFormatterDelegate(ref StatField statField);
	}
}
