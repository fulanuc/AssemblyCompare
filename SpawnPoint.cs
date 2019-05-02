using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003EA RID: 1002
	public class SpawnPoint : MonoBehaviour
	{
		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x060015DE RID: 5598 RVA: 0x00010682 File Offset: 0x0000E882
		public static ReadOnlyCollection<SpawnPoint> readOnlyInstancesList
		{
			get
			{
				return SpawnPoint._readOnlyInstancesList;
			}
		}

		// Token: 0x060015DF RID: 5599 RVA: 0x00010689 File Offset: 0x0000E889
		private void OnEnable()
		{
			SpawnPoint.instancesList.Add(this);
		}

		// Token: 0x060015E0 RID: 5600 RVA: 0x00075294 File Offset: 0x00073494
		public static SpawnPoint ConsumeSpawnPoint()
		{
			if (SpawnPoint.instancesList.Count == 0)
			{
				return null;
			}
			SpawnPoint spawnPoint = null;
			for (int i = 0; i < SpawnPoint.readOnlyInstancesList.Count; i++)
			{
				if (!SpawnPoint.readOnlyInstancesList[i].consumed)
				{
					spawnPoint = SpawnPoint.readOnlyInstancesList[i];
					SpawnPoint.readOnlyInstancesList[i].consumed = true;
					break;
				}
			}
			if (!spawnPoint)
			{
				for (int j = 0; j < SpawnPoint.readOnlyInstancesList.Count; j++)
				{
					SpawnPoint.readOnlyInstancesList[j].consumed = false;
				}
				spawnPoint = SpawnPoint.readOnlyInstancesList[0];
			}
			return spawnPoint;
		}

		// Token: 0x060015E1 RID: 5601 RVA: 0x00010696 File Offset: 0x0000E896
		private void OnDisable()
		{
			SpawnPoint.instancesList.Remove(this);
		}

		// Token: 0x04001932 RID: 6450
		private static List<SpawnPoint> instancesList = new List<SpawnPoint>();

		// Token: 0x04001933 RID: 6451
		private static ReadOnlyCollection<SpawnPoint> _readOnlyInstancesList = new ReadOnlyCollection<SpawnPoint>(SpawnPoint.instancesList);

		// Token: 0x04001934 RID: 6452
		[Tooltip("Flagged when a player spawns on this position, to stop overlapping spawn positions")]
		public bool consumed;
	}
}
