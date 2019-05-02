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
		// Token: 0x06000ABD RID: 2749 RVA: 0x0004917C File Offset: 0x0004737C
		public virtual GameObject DoSpawn(Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, position, rotation);
			if (this.sendOverNetwork)
			{
				NetworkServer.Spawn(gameObject);
			}
			return gameObject;
		}

		// Token: 0x04000E4D RID: 3661
		public GameObject prefab;

		// Token: 0x04000E4E RID: 3662
		public bool sendOverNetwork;

		// Token: 0x04000E4F RID: 3663
		public HullClassification hullSize;

		// Token: 0x04000E50 RID: 3664
		public MapNodeGroup.GraphType nodeGraphType;

		// Token: 0x04000E51 RID: 3665
		[EnumMask(typeof(NodeFlags))]
		public NodeFlags requiredFlags;

		// Token: 0x04000E52 RID: 3666
		[EnumMask(typeof(NodeFlags))]
		public NodeFlags forbiddenFlags;

		// Token: 0x04000E53 RID: 3667
		public bool occupyPosition;
	}
}
