using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003D0 RID: 976
	[Serializable]
	public struct GameObjectToggleGroup
	{
		// Token: 0x04001873 RID: 6259
		public GameObject[] objects;

		// Token: 0x04001874 RID: 6260
		public int minEnabled;

		// Token: 0x04001875 RID: 6261
		public int maxEnabled;
	}
}
