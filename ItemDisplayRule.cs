using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200022B RID: 555
	[Serializable]
	public struct ItemDisplayRule
	{
		// Token: 0x04000E22 RID: 3618
		public ItemDisplayRuleType ruleType;

		// Token: 0x04000E23 RID: 3619
		public GameObject followerPrefab;

		// Token: 0x04000E24 RID: 3620
		public string childName;

		// Token: 0x04000E25 RID: 3621
		public Vector3 localPos;

		// Token: 0x04000E26 RID: 3622
		public Vector3 localAngles;

		// Token: 0x04000E27 RID: 3623
		public Vector3 localScale;

		// Token: 0x04000E28 RID: 3624
		public LimbFlags limbMask;
	}
}
