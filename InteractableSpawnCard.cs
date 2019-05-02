using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000227 RID: 551
	[CreateAssetMenu(menuName = "SpawnCards")]
	public class InteractableSpawnCard : SpawnCard
	{
		// Token: 0x06000AAF RID: 2735 RVA: 0x00048F04 File Offset: 0x00047104
		public override GameObject DoSpawn(Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, position, rotation);
			Transform transform = gameObject.transform;
			RaycastHit raycastHit;
			if (this.orientToFloor && Physics.Raycast(new Ray(position + gameObject.transform.up * 3f, -transform.up), out raycastHit, 9f, LayerIndex.world.mask))
			{
				transform.up = raycastHit.normal;
			}
			transform.Rotate(Vector3.up, UnityEngine.Random.Range(0f, 360f), Space.Self);
			if (this.slightlyRandomizeOrientation)
			{
				transform.Translate(Vector3.down * 0.3f, Space.Self);
				transform.rotation *= Quaternion.Euler(UnityEngine.Random.Range(-30f, 30f), UnityEngine.Random.Range(-30f, 30f), UnityEngine.Random.Range(-30f, 30f));
			}
			NetworkServer.Spawn(gameObject);
			return gameObject;
		}

		// Token: 0x04000E15 RID: 3605
		public bool orientToFloor;

		// Token: 0x04000E16 RID: 3606
		public bool slightlyRandomizeOrientation;

		// Token: 0x04000E17 RID: 3607
		private const float raycastLength = 6f;

		// Token: 0x04000E18 RID: 3608
		private const float floorOffset = 3f;
	}
}
