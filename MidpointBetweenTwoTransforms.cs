using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200035C RID: 860
	public class MidpointBetweenTwoTransforms : MonoBehaviour
	{
		// Token: 0x060011B8 RID: 4536 RVA: 0x0000D7CF File Offset: 0x0000B9CF
		public void Update()
		{
			base.transform.position = Vector3.Lerp(this.transform1.position, this.transform2.position, 0.5f);
		}

		// Token: 0x040015C3 RID: 5571
		public Transform transform1;

		// Token: 0x040015C4 RID: 5572
		public Transform transform2;
	}
}
