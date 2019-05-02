using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003F0 RID: 1008
	public class SpawnPoint : MonoBehaviour
	{
		// Token: 0x170001FB RID: 507
		// (get) Token: 0x0600161B RID: 5659 RVA: 0x00010A8B File Offset: 0x0000EC8B
		public static ReadOnlyCollection<SpawnPoint> readOnlyInstancesList
		{
			get
			{
				return SpawnPoint._readOnlyInstancesList;
			}
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x00010A92 File Offset: 0x0000EC92
		private void OnEnable()
		{
			SpawnPoint.instancesList.Add(this);
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x000758CC File Offset: 0x00073ACC
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

		// Token: 0x0600161E RID: 5662 RVA: 0x00010A9F File Offset: 0x0000EC9F
		private void OnDisable()
		{
			SpawnPoint.instancesList.Remove(this);
		}

		// Token: 0x0400195B RID: 6491
		private static List<SpawnPoint> instancesList = new List<SpawnPoint>();

		// Token: 0x0400195C RID: 6492
		private static ReadOnlyCollection<SpawnPoint> _readOnlyInstancesList = new ReadOnlyCollection<SpawnPoint>(SpawnPoint.instancesList);

		// Token: 0x0400195D RID: 6493
		[Tooltip("Flagged when a player spawns on this position, to stop overlapping spawn positions")]
		public bool consumed;
	}
}
