using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000573 RID: 1395
	public class GenericSceneSpawnPoint : MonoBehaviour
	{
		// Token: 0x06001F2A RID: 7978 RVA: 0x00098F48 File Offset: 0x00097148
		private void Start()
		{
			if (NetworkServer.active)
			{
				NetworkServer.Spawn(UnityEngine.Object.Instantiate<GameObject>(this.networkedObjectPrefab, base.transform.position, base.transform.rotation));
				base.gameObject.SetActive(false);
				return;
			}
			base.gameObject.SetActive(false);
		}

		// Token: 0x040021CB RID: 8651
		public GameObject networkedObjectPrefab;
	}
}
