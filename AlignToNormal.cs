using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000255 RID: 597
	public class AlignToNormal : MonoBehaviour
	{
		// Token: 0x06000B26 RID: 2854 RVA: 0x0004B1A8 File Offset: 0x000493A8
		private void Start()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position + base.transform.up * this.offsetDistance, -base.transform.up, out raycastHit, this.maxDistance, LayerIndex.world.mask))
			{
				base.transform.position = raycastHit.point;
				if (!this.changePositionOnly)
				{
					base.transform.up = raycastHit.normal;
				}
			}
		}

		// Token: 0x04000F29 RID: 3881
		[Tooltip("The amount to raycast down from.")]
		public float maxDistance;

		// Token: 0x04000F2A RID: 3882
		[Tooltip("The amount to pull the object out of the ground initially to test.")]
		public float offsetDistance;

		// Token: 0x04000F2B RID: 3883
		[Tooltip("Send to floor only - don't change normals.")]
		public bool changePositionOnly;
	}
}
