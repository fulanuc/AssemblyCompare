using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200033C RID: 828
	public class InstantiatePrefabOnStart : MonoBehaviour
	{
		// Token: 0x06001106 RID: 4358 RVA: 0x000646B4 File Offset: 0x000628B4
		public void Start()
		{
			if (!this.networkedPrefab || NetworkServer.active)
			{
				Vector3 position = this.targetTransform ? this.targetTransform.position : Vector3.zero;
				Quaternion rotation = this.copyTargetRotation ? this.targetTransform.rotation : Quaternion.identity;
				Transform parent = this.parentToTarget ? this.targetTransform : null;
				GameObject obj = UnityEngine.Object.Instantiate<GameObject>(this.prefab, position, rotation, parent);
				if (this.networkedPrefab)
				{
					NetworkServer.Spawn(obj);
				}
			}
		}

		// Token: 0x04001536 RID: 5430
		[Tooltip("The prefab to instantiate.")]
		public GameObject prefab;

		// Token: 0x04001537 RID: 5431
		[Tooltip("The object upon which the prefab will be positioned.")]
		public Transform targetTransform;

		// Token: 0x04001538 RID: 5432
		[Tooltip("The transform upon which to instantiate the prefab.")]
		public bool copyTargetRotation;

		// Token: 0x04001539 RID: 5433
		[Tooltip("Whether or not to parent the instantiated prefab to the specified transform.")]
		public bool parentToTarget;

		// Token: 0x0400153A RID: 5434
		[Tooltip("Whether or not this is a networked prefab. If so, this will only run on the server, and will be spawned over the network.")]
		public bool networkedPrefab;
	}
}
