using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000224 RID: 548
	[CreateAssetMenu(menuName = "SpawnCards")]
	public class CharacterSpawnCard : SpawnCard
	{
		// Token: 0x06000AA7 RID: 2727 RVA: 0x000089A2 File Offset: 0x00006BA2
		public override GameObject DoSpawn(Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, position, rotation);
			NetworkServer.Spawn(gameObject);
			gameObject.GetComponent<CharacterMaster>().Respawn(position, rotation);
			return gameObject;
		}

		// Token: 0x04000E0C RID: 3596
		public bool noElites;
	}
}
