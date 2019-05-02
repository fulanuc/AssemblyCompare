using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000389 RID: 905
	public class PlacementUtility : MonoBehaviour
	{
		// Token: 0x060012F4 RID: 4852 RVA: 0x000025F6 File Offset: 0x000007F6
		private void Start()
		{
		}

		// Token: 0x060012F5 RID: 4853 RVA: 0x000025F6 File Offset: 0x000007F6
		private void Update()
		{
		}

		// Token: 0x060012F6 RID: 4854 RVA: 0x0000E854 File Offset: 0x0000CA54
		public void PlacePrefab(Vector3 targetPosition, Quaternion rotation)
		{
			this.prefabPlacement;
		}

		// Token: 0x040016AB RID: 5803
		public Transform targetParent;

		// Token: 0x040016AC RID: 5804
		public GameObject prefabPlacement;

		// Token: 0x040016AD RID: 5805
		public bool normalToSurface;

		// Token: 0x040016AE RID: 5806
		public bool flipForwardDirection;

		// Token: 0x040016AF RID: 5807
		public float minScale = 1f;

		// Token: 0x040016B0 RID: 5808
		public float maxScale = 2f;

		// Token: 0x040016B1 RID: 5809
		public float minXRotation;

		// Token: 0x040016B2 RID: 5810
		public float maxXRotation;

		// Token: 0x040016B3 RID: 5811
		public float minYRotation;

		// Token: 0x040016B4 RID: 5812
		public float maxYRotation;

		// Token: 0x040016B5 RID: 5813
		public float minZRotation;

		// Token: 0x040016B6 RID: 5814
		public float maxZRotation;
	}
}
