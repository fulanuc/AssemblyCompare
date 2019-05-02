using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RoR2.CharacterAI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000464 RID: 1124
	public static class MasterCatalog
	{
		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06001950 RID: 6480 RVA: 0x00012E28 File Offset: 0x00011028
		public static IEnumerable<CharacterMaster> allMasters
		{
			get
			{
				return MasterCatalog.masterPrefabMasterComponents;
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06001951 RID: 6481 RVA: 0x00012E2F File Offset: 0x0001102F
		public static IEnumerable<CharacterMaster> allAiMasters
		{
			get
			{
				return MasterCatalog.aiMasterPrefabs;
			}
		}

		// Token: 0x06001952 RID: 6482 RVA: 0x00012E36 File Offset: 0x00011036
		public static GameObject GetMasterPrefab(int index)
		{
			if (index < 0)
			{
				return null;
			}
			return MasterCatalog.masterPrefabs[index];
		}

		// Token: 0x06001953 RID: 6483 RVA: 0x00082914 File Offset: 0x00080B14
		public static int FindMasterIndex([NotNull] string bodyName)
		{
			int result;
			if (MasterCatalog.nameToIndexMap.TryGetValue(bodyName, out result))
			{
				return result;
			}
			return -1;
		}

		// Token: 0x06001954 RID: 6484 RVA: 0x00012E45 File Offset: 0x00011045
		public static int FindMasterIndex(GameObject bodyObject)
		{
			if (!bodyObject)
			{
				return -1;
			}
			return MasterCatalog.FindMasterIndex(bodyObject.name);
		}

		// Token: 0x06001955 RID: 6485 RVA: 0x00082934 File Offset: 0x00080B34
		public static GameObject FindMasterPrefab([NotNull] string bodyName)
		{
			int num = MasterCatalog.FindMasterIndex(bodyName);
			if (num != -1)
			{
				return MasterCatalog.GetMasterPrefab(num);
			}
			return null;
		}

		// Token: 0x06001956 RID: 6486 RVA: 0x00082954 File Offset: 0x00080B54
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			MasterCatalog.masterPrefabs = Resources.LoadAll<GameObject>("Prefabs/CharacterMasters/");
			MasterCatalog.masterPrefabMasterComponents = new CharacterMaster[MasterCatalog.masterPrefabs.Length];
			for (int i = 0; i < MasterCatalog.masterPrefabs.Length; i++)
			{
				MasterCatalog.nameToIndexMap.Add(MasterCatalog.masterPrefabs[i].name, i);
				MasterCatalog.nameToIndexMap.Add(MasterCatalog.masterPrefabs[i].name + "(Clone)", i);
				MasterCatalog.masterPrefabMasterComponents[i] = MasterCatalog.masterPrefabs[i].GetComponent<CharacterMaster>();
			}
			MasterCatalog.aiMasterPrefabs = (from master in MasterCatalog.masterPrefabMasterComponents
			where master.GetComponent<BaseAI>()
			select master).ToArray<CharacterMaster>();
			MasterCatalog.availability.MakeAvailable();
		}

		// Token: 0x04001C8B RID: 7307
		public static ResourceAvailability availability = default(ResourceAvailability);

		// Token: 0x04001C8C RID: 7308
		private static GameObject[] masterPrefabs;

		// Token: 0x04001C8D RID: 7309
		private static CharacterMaster[] masterPrefabMasterComponents;

		// Token: 0x04001C8E RID: 7310
		private static CharacterMaster[] aiMasterPrefabs;

		// Token: 0x04001C8F RID: 7311
		private static readonly Dictionary<string, int> nameToIndexMap = new Dictionary<string, int>();
	}
}
