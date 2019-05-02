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
		// Token: 0x06000B1B RID: 2843 RVA: 0x00008F1C File Offset: 0x0000711C
		private void Awake()
		{
			this.cameraRigController = base.GetComponent<CameraRigController>();
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x000025DA File Offset: 0x000007DA
		private void CollectTargets()
		{
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x00008F2A File Offset: 0x0000712A
		private void Update()
		{
			this.cameraRigController.target;
		}

		// Token: 0x04000F18 RID: 3864
		private CameraRigController cameraRigController;

		// Token: 0x02000250 RID: 592
		private struct BullseyeDescriptor
		{
			// Token: 0x04000F19 RID: 3865
			public Vector3 position;

			// Token: 0x04000F1A RID: 3866
			public Quaternion rotation;

			// Token: 0x04000F1B RID: 3867
			public Vector3 scale;
		}

		// Token: 0x02000251 RID: 593
		private struct GenerateScreenSpaceDataJob : IJobParallelFor
		{
			// Token: 0x06000B1F RID: 2847 RVA: 0x000025DA File Offset: 0x000007DA
			public void Execute(int index)
			{
			}

			// Token: 0x04000F1C RID: 3868
			[ReadOnly]
			public Rect screenCoords;

			// Token: 0x04000F1D RID: 3869
			[ReadOnly]
			public NativeArray<AimAssistController.GenerateScreenSpaceDataJob.Input> targetBuffer;

			// Token: 0x04000F1E RID: 3870
			public NativeArray<AimAssistController.GenerateScreenSpaceDataJob.Output> resultBuffer;

			// Token: 0x02000252 RID: 594
			public struct Input
			{
				// Token: 0x04000F1F RID: 3871
				public GameObject associatedObject;

				// Token: 0x04000F20 RID: 3872
				public AimAssistController.BullseyeDescriptor BullseyeDescriptor;
			}

			// Token: 0x02000253 RID: 595
			public struct Output
			{
				// Token: 0x04000F21 RID: 3873
				public float score;
			}
		}
	}
}
