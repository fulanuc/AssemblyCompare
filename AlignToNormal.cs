using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000255 RID: 597
	public class AlignToNormal : MonoBehaviour
	{
		// Token: 0x06000B23 RID: 2851 RVA: 0x0004AF9C File Offset: 0x0004919C
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

		// Token: 0x04000F23 RID: 3875
		[Tooltip("The amount to raycast down from.")]
		public float maxDistance;

		// Token: 0x04000F24 RID: 3876
		[Tooltip("The amount to pull the object out of the ground initially to test.")]
		public float offsetDistance;

		// Token: 0x04000F25 RID: 3877
		[Tooltip("Send to floor only - don't change normals.")]
		public bool changePositionOnly;
	}
}
