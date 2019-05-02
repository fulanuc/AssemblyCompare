using System;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000230 RID: 560
	[CreateAssetMenu(menuName = "SpawnCards")]
	public class SpawnCard : ScriptableObject
	{
		// Token: 0x06000AB9 RID: 2745 RVA: 0x00048EC0 File Offset: 0x000470C0
		public virtual GameObject DoSpawn(Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, position, rotation);
			if (this.sendOverNetwork)
			{
				NetworkServer.Spawn(gameObject);
			}
			return gameObject;
		}

		// Token: 0x04000E48 RID: 3656
		public GameObject prefab;

		// Token: 0x04000E49 RID: 3657
		public bool sendOverNetwork;

		// Token: 0x04000E4A RID: 3658
		public HullClassification hullSize;

		// Token: 0x04000E4B RID: 3659
		public MapNodeGroup.GraphType nodeGraphType;

		// Token: 0x04000E4C RID: 3660
		[EnumMask(typeof(NodeFlags))]
		public NodeFlags requiredFlags;

		// Token: 0x04000E4D RID: 3661
		[EnumMask(typeof(NodeFlags))]
		public NodeFlags forbiddenFlags;

		// Token: 0x04000E4E RID: 3662
		public bool occupyPosition;
	}
}
