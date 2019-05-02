using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000224 RID: 548
	[CreateAssetMenu(menuName = "SpawnCards")]
	public class CharacterSpawnCard : SpawnCard
	{
		// Token: 0x06000AAB RID: 2731 RVA: 0x000089C6 File Offset: 0x00006BC6
		public override GameObject DoSpawn(Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, position, rotation);
			NetworkServer.Spawn(gameObject);
			gameObject.GetComponent<CharacterMaster>().Respawn(position, rotation, false);
			return gameObject;
		}

		// Token: 0x04000E10 RID: 3600
		public bool noElites;
	}
}
