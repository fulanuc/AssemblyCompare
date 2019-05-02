using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200022B RID: 555
	[Serializable]
	public struct ItemDisplayRule
	{
		// Token: 0x04000E26 RID: 3622
		public ItemDisplayRuleType ruleType;

		// Token: 0x04000E27 RID: 3623
		public GameObject followerPrefab;

		// Token: 0x04000E28 RID: 3624
		public string childName;

		// Token: 0x04000E29 RID: 3625
		public Vector3 localPos;

		// Token: 0x04000E2A RID: 3626
		public Vector3 localAngles;

		// Token: 0x04000E2B RID: 3627
		public Vector3 localScale;

		// Token: 0x04000E2C RID: 3628
		public LimbFlags limbMask;
	}
}
