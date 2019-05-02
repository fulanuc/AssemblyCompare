using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000336 RID: 822
	public class IKTargetPassive : MonoBehaviour, IIKTargetBehavior
	{
		// Token: 0x060010F0 RID: 4336 RVA: 0x000025DA File Offset: 0x000007DA
		public void UpdateIKState(int targetState)
		{
		}

		// Token: 0x060010F1 RID: 4337 RVA: 0x0000CE3B File Offset: 0x0000B03B
		private void Awake()
		{
			if (this.cacheFirstPosition)
			{
				this.cachedLocalPosition = base.transform.localPosition;
			}
		}

		// Token: 0x060010F2 RID: 4338 RVA: 0x0006418C File Offset: 0x0006238C
		private void LateUpdate()
		{
			this.selfPlantTimer -= Time.deltaTime;
			if (this.selfPlant && this.selfPlantTimer <= 0f)
			{
				this.selfPlantTimer = 1f / this.selfPlantFrequency;
				this.UpdateIKTargetPosition();
			}
			this.UpdateYOffset();
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x000641E0 File Offset: 0x000623E0
		public void UpdateIKTargetPosition()
		{
			this.ResetTransformToCachedPosition();
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position + Vector3.up * -this.minHeight, Vector3.down, out raycastHit, this.maxHeight - this.minHeight, LayerIndex.world.mask))
			{
				this.targetHeightOffset = raycastHit.point.y - base.transform.position.y;
				return;
			}
			this.targetHeightOffset = 0f;
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x00064270 File Offset: 0x00062470
		public void UpdateYOffset()
		{
			float t = 1f;
			if (this.animator && this.animatorIKWeightFloat.Length > 0)
			{
				t = this.animator.GetFloat(this.animatorIKWeightFloat);
			}
			this.smoothedTargetHeightOffset = Mathf.SmoothDamp(this.smoothedTargetHeightOffset, this.targetHeightOffset, ref this.smoothdampVelocity, this.dampTime, float.PositiveInfinity, Time.deltaTime);
			this.ResetTransformToCachedPosition();
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + Mathf.Lerp(0f, this.smoothedTargetHeightOffset, t), base.transform.position.z);
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x0000CE56 File Offset: 0x0000B056
		private void ResetTransformToCachedPosition()
		{
			if (this.cacheFirstPosition)
			{
				base.transform.localPosition = new Vector3(this.cachedLocalPosition.x, this.cachedLocalPosition.y, this.cachedLocalPosition.z);
			}
		}

		// Token: 0x040014FF RID: 5375
		private float smoothedTargetHeightOffset;

		// Token: 0x04001500 RID: 5376
		private float targetHeightOffset;

		// Token: 0x04001501 RID: 5377
		private float smoothdampVelocity;

		// Token: 0x04001502 RID: 5378
		public float minHeight = -0.3f;

		// Token: 0x04001503 RID: 5379
		public float maxHeight = 1f;

		// Token: 0x04001504 RID: 5380
		public float dampTime = 0.1f;

		// Token: 0x04001505 RID: 5381
		[Tooltip("The IK weight float parameter if used")]
		public string animatorIKWeightFloat = "";

		// Token: 0x04001506 RID: 5382
		public Animator animator;

		// Token: 0x04001507 RID: 5383
		[Tooltip("The target transform will plant without any calls from external IK chains")]
		public bool selfPlant;

		// Token: 0x04001508 RID: 5384
		public float selfPlantFrequency = 5f;

		// Token: 0x04001509 RID: 5385
		[Tooltip("Whether or not to cache where the raycast begins. Used when not attached to bones, who reset themselves via animator.")]
		public bool cacheFirstPosition;

		// Token: 0x0400150A RID: 5386
		private Vector3 cachedLocalPosition;

		// Token: 0x0400150B RID: 5387
		private float selfPlantTimer;
	}
}
