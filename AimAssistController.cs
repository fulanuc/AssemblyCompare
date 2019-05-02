using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200024F RID: 591
	[RequireComponent(typeof(CameraRigController))]
	public class AimAssistController : MonoBehaviour
	{
		// Token: 0x06000B18 RID: 2840 RVA: 0x00008EF7 File Offset: 0x000070F7
		private void Awake()
		{
			this.cameraRigController = base.GetComponent<CameraRigController>();
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x000025F6 File Offset: 0x000007F6
		private void CollectTargets()
		{
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x00008F05 File Offset: 0x00007105
		private void Update()
		{
			this.cameraRigController.target;
		}

		// Token: 0x04000F12 RID: 3858
		private CameraRigController cameraRigController;

		// Token: 0x02000250 RID: 592
		private struct BullseyeDescriptor
		{
			// Token: 0x04000F13 RID: 3859
			public Vector3 position;

			// Token: 0x04000F14 RID: 3860
			public Quaternion rotation;

			// Token: 0x04000F15 RID: 3861
			public Vector3 scale;
		}

		// Token: 0x02000251 RID: 593
		private struct GenerateScreenSpaceDataJob : IJobParallelFor
		{
			// Token: 0x06000B1C RID: 2844 RVA: 0x000025F6 File Offset: 0x000007F6
			public void Execute(int index)
			{
			}

			// Token: 0x04000F16 RID: 3862
			[ReadOnly]
			public Rect screenCoords;

			// Token: 0x04000F17 RID: 3863
			[ReadOnly]
			public NativeArray<AimAssistController.GenerateScreenSpaceDataJob.Input> targetBuffer;

			// Token: 0x04000F18 RID: 3864
			public NativeArray<AimAssistController.GenerateScreenSpaceDataJob.Output> resultBuffer;

			// Token: 0x02000252 RID: 594
			public struct Input
			{
				// Token: 0x04000F19 RID: 3865
				public GameObject associatedObject;

				// Token: 0x04000F1A RID: 3866
				public AimAssistController.BullseyeDescriptor BullseyeDescriptor;
			}

			// Token: 0x02000253 RID: 595
			public struct Output
			{
				// Token: 0x04000F1B RID: 3867
				public float score;
			}
		}
	}
}
