using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000223 RID: 547
	[CreateAssetMenu]
	public class CharacterCameraParams : ScriptableObject
	{
		// Token: 0x04000E07 RID: 3591
		public float minPitch = -70f;

		// Token: 0x04000E08 RID: 3592
		public float maxPitch = 70f;

		// Token: 0x04000E09 RID: 3593
		public float wallCushion = 0.1f;

		// Token: 0x04000E0A RID: 3594
		public float pivotVerticalOffset = 1.6f;

		// Token: 0x04000E0B RID: 3595
		public Vector3 standardLocalCameraPos = new Vector3(0f, 0f, -5f);
	}
}
