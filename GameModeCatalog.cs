using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000430 RID: 1072
	public static class GameModeCatalog
	{
		// Token: 0x060017ED RID: 6125 RVA: 0x00011D99 File Offset: 0x0000FF99
		static GameModeCatalog()
		{
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(GameModeCatalog.LoadGameModes));
		}

		// Token: 0x060017EE RID: 6126 RVA: 0x0007D198 File Offset: 0x0007B398
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

		// Token: 0x060017EF RID: 6127 RVA: 0x0007D2B8 File Offset: 0x0007B4B8
		public static Run FindGameModePrefabComponent(string name)
		{
			Run result;
			GameModeCatalog.nameToPrefabComponents.TryGetValue(name, out result);
			return result;
		}

		// Token: 0x060017F0 RID: 6128 RVA: 0x00011DCF File Offset: 0x0000FFCF
		public static Run GetGameModePrefabComponent(int index)
		{
			if (index < 0 || index >= GameModeCatalog.indexToPrefabComponents.Length)
			{
				return null;
			}
			return GameModeCatalog.indexToPrefabComponents[index];
		}

		// Token: 0x060017F1 RID: 6129 RVA: 0x0007D2D4 File Offset: 0x0007B4D4
		public static int FindGameModeIndex(string name)
		{
			int result;
			GameModeCatalog.nameToIndex.TryGetValue(name, out result);
			return result;
		}

		// Token: 0x04001B33 RID: 6963
		private static readonly Dictionary<string, int> nameToIndex = new Dictionary<string, int>();

		// Token: 0x04001B34 RID: 6964
		private static Run[] indexToPrefabComponents;

		// Token: 0x04001B35 RID: 6965
		private static readonly Dictionary<string, Run> nameToPrefabComponents = new Dictionary<string, Run>();

		// Token: 0x04001B36 RID: 6966
		public static ResourceAvailability availability;
	}
}
