using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200035F RID: 863
	public class MidpointBetweenTwoTransforms : MonoBehaviour
	{
		// Token: 0x060011CF RID: 4559 RVA: 0x0000D8B8 File Offset: 0x0000BAB8
		public void Update()
		{
			base.transform.position = Vector3.Lerp(this.transform1.position, this.transform2.position, 0.5f);
		}

		// Token: 0x040015DC RID: 5596
		public Transform transform1;

		// Token: 0x040015DD RID: 5597
		public Transform transform2;
	}
}
