using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200038E RID: 910
	public class PlacementUtility : MonoBehaviour
	{
		// Token: 0x06001312 RID: 4882 RVA: 0x000025DA File Offset: 0x000007DA
		private void Start()
		{
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x000025DA File Offset: 0x000007DA
		private void Update()
		{
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x0000E9DF File Offset: 0x0000CBDF
		public void PlacePrefab(Vector3 targetPosition, Quaternion rotation)
		{
			this.prefabPlacement;
		}

		// Token: 0x040016C7 RID: 5831
		public Transform targetParent;

		// Token: 0x040016C8 RID: 5832
		public GameObject prefabPlacement;

		// Token: 0x040016C9 RID: 5833
		public bool normalToSurface;

		// Token: 0x040016CA RID: 5834
		public bool flipForwardDirection;

		// Token: 0x040016CB RID: 5835
		public float minScale = 1f;

		// Token: 0x040016CC RID: 5836
		public float maxScale = 2f;

		// Token: 0x040016CD RID: 5837
		public float minXRotation;

		// Token: 0x040016CE RID: 5838
		public float maxXRotation;

		// Token: 0x040016CF RID: 5839
		public float minYRotation;

		// Token: 0x040016D0 RID: 5840
		public float maxYRotation;

		// Token: 0x040016D1 RID: 5841
		public float minZRotation;

		// Token: 0x040016D2 RID: 5842
		public float maxZRotation;
	}
}
