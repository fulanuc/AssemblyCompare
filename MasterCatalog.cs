using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RoR2.CharacterAI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000459 RID: 1113
	public static class MasterCatalog
	{
		// Token: 0x17000249 RID: 585
		// (get) Token: 0x060018F4 RID: 6388 RVA: 0x0001291B File Offset: 0x00010B1B
		public static IEnumerable<CharacterMaster> allMasters
		{
			get
			{
				return MasterCatalog.masterPrefabMasterComponents;
			}
		}

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x060018F5 RID: 6389 RVA: 0x00012922 File Offset: 0x00010B22
		public static IEnumerable<CharacterMaster> allAiMasters
		{
			get
			{
				return MasterCatalog.aiMasterPrefabs;
			}
		}

		// Token: 0x060018F6 RID: 6390 RVA: 0x00012929 File Offset: 0x00010B29
		public static GameObject GetMasterPrefab(int index)
		{
			if (index < 0)
			{
				return null;
			}
			return MasterCatalog.masterPrefabs[index];
		}

		// Token: 0x060018F7 RID: 6391 RVA: 0x00081F6C File Offset: 0x0008016C
		public static int FindMasterIndex([NotNull] string bodyName)
		{
			int result;
			if (MasterCatalog.nameToIndexMap.TryGetValue(bodyName, out result))
			{
				return result;
			}
			return -1;
		}

		// Token: 0x060018F8 RID: 6392 RVA: 0x00012938 File Offset: 0x00010B38
		public static int FindMasterIndex(GameObject bodyObject)
		{
			if (!bodyObject)
			{
				return -1;
			}
			return MasterCatalog.FindMasterIndex(bodyObject.name);
		}

		// Token: 0x060018F9 RID: 6393 RVA: 0x00081F8C File Offset: 0x0008018C
		public static GameObject FindMasterPrefab([NotNull] string bodyName)
		{
			int num = MasterCatalog.FindMasterIndex(bodyName);
			if (num != -1)
			{
				return MasterCatalog.GetMasterPrefab(num);
			}
			return null;
		}

		// Token: 0x060018FA RID: 6394 RVA: 0x00081FAC File Offset: 0x000801AC
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

		// Token: 0x04001C57 RID: 7255
		public static ResourceAvailability availability = default(ResourceAvailability);

		// Token: 0x04001C58 RID: 7256
		private static GameObject[] masterPrefabs;

		// Token: 0x04001C59 RID: 7257
		private static CharacterMaster[] masterPrefabMasterComponents;

		// Token: 0x04001C5A RID: 7258
		private static CharacterMaster[] aiMasterPrefabs;

		// Token: 0x04001C5B RID: 7259
		private static readonly Dictionary<string, int> nameToIndexMap = new Dictionary<string, int>();
	}
}
