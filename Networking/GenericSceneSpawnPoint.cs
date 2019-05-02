using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000582 RID: 1410
	public class GenericSceneSpawnPoint : MonoBehaviour
	{
		// Token: 0x06001F94 RID: 8084 RVA: 0x00099C64 File Offset: 0x00097E64
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

		// Token: 0x04002209 RID: 8713
		public GameObject networkedObjectPrefab;
	}
}
