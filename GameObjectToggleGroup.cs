using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003D6 RID: 982
	[Serializable]
	public struct GameObjectToggleGroup
	{
		// Token: 0x04001895 RID: 6293
		public GameObject[] objects;

		// Token: 0x04001896 RID: 6294
		public int minEnabled;

		// Token: 0x04001897 RID: 6295
		public int maxEnabled;
	}
}
