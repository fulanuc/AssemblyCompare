using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000223 RID: 547
	[CreateAssetMenu]
	public class CharacterCameraParams : ScriptableObject
	{
		// Token: 0x04000E0B RID: 3595
		public float minPitch = -70f;

		// Token: 0x04000E0C RID: 3596
		public float maxPitch = 70f;

		// Token: 0x04000E0D RID: 3597
		public float wallCushion = 0.1f;

		// Token: 0x04000E0E RID: 3598
		public float pivotVerticalOffset = 1.6f;

		// Token: 0x04000E0F RID: 3599
		public Vector3 standardLocalCameraPos = new Vector3(0f, 0f, -5f);
	}
}
