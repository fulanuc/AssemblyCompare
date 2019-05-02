using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000436 RID: 1078
	public static class GameModeCatalog
	{
		// Token: 0x06001832 RID: 6194 RVA: 0x000121CB File Offset: 0x000103CB
		static GameModeCatalog()
		{
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(GameModeCatalog.LoadGameModes));
		}

		// Token: 0x06001833 RID: 6195 RVA: 0x0007D798 File Offset: 0x0007B998
		private static void LoadGameModes()
		{
			GameModeCatalog.indexToPrefabComponents = (from p in Resources.LoadAll<GameObject>("Prefabs/GameModes/")
			orderby p.name
			select p.GetComponent<Run>()).ToArray<Run>();
			GameModeCatalog.nameToIndex.Clear();
			GameModeCatalog.nameToPrefabComponents.Clear();
			int i = 0;
			int num = GameModeCatalog.indexToPrefabComponents.Length;
			while (i < num)
			{
				Run run = GameModeCatalog.indexToPrefabComponents[i];
				string name = run.gameObject.name;
				string key = name + "(Clone)";
				GameModeCatalog.nameToIndex.Add(name, i);
				GameModeCatalog.nameToIndex.Add(key, i);
				GameModeCatalog.nameToPrefabComponents.Add(name, run);
				GameModeCatalog.nameToPrefabComponents.Add(key, run);
				Debug.LogFormat("Registered gamemode {0} {1}", new object[]
				{
					run.gameObject.name,
					run
				});
				i++;
			}
			GameModeCatalog.availability.MakeAvailable();
		}

		// Token: 0x06001834 RID: 6196 RVA: 0x0007D8B8 File Offset: 0x0007BAB8
		public static Run FindGameModePrefabComponent(string name)
		{
			Run result;
			GameModeCatalog.nameToPrefabComponents.TryGetValue(name, out result);
			return result;
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x00012201 File Offset: 0x00010401
		public static Run GetGameModePrefabComponent(int index)
		{
			if (index < 0 || index >= GameModeCatalog.indexToPrefabComponents.Length)
			{
				return null;
			}
			return GameModeCatalog.indexToPrefabComponents[index];
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x0007D8D4 File Offset: 0x0007BAD4
		public static int FindGameModeIndex(string name)
		{
			int result;
			GameModeCatalog.nameToIndex.TryGetValue(name, out result);
			return result;
		}

		// Token: 0x04001B5F RID: 7007
		private static readonly Dictionary<string, int> nameToIndex = new Dictionary<string, int>();

		// Token: 0x04001B60 RID: 7008
		private static Run[] indexToPrefabComponents;

		// Token: 0x04001B61 RID: 7009
		private static readonly Dictionary<string, Run> nameToPrefabComponents = new Dictionary<string, Run>();

		// Token: 0x04001B62 RID: 7010
		public static ResourceAvailability availability;
	}
}
