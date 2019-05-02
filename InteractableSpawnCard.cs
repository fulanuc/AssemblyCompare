using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000227 RID: 551
	[CreateAssetMenu(menuName = "SpawnCards")]
	public class InteractableSpawnCard : SpawnCard
	{
		// Token: 0x06000AAB RID: 2731 RVA: 0x00048C48 File Offset: 0x00046E48
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

		// Token: 0x04000E11 RID: 3601
		public bool orientToFloor;

		// Token: 0x04000E12 RID: 3602
		public bool slightlyRandomizeOrientation;

		// Token: 0x04000E13 RID: 3603
		private const float raycastLength = 6f;

		// Token: 0x04000E14 RID: 3604
		private const float floorOffset = 3f;
	}
}
