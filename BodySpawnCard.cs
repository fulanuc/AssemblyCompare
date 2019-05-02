using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000222 RID: 546
	[CreateAssetMenu(menuName = "SpawnCards")]
	internal class BodySpawnCard : SpawnCard
	{
		// Token: 0x06000AA8 RID: 2728 RVA: 0x00048E38 File Offset: 0x00047038
		public override GameObject DoSpawn(Vector3 position, Quaternion rotation)
		{
			Vector3 position2 = position;
			position2.y += Util.GetBodyPrefabFootOffset(this.prefab);
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, position2, rotation);
			NetworkServer.Spawn(gameObject);
			return gameObject;
		}
	}
}
