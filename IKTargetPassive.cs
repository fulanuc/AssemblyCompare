using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000334 RID: 820
	public class IKTargetPassive : MonoBehaviour, IIKTargetBehavior
	{
		// Token: 0x060010DB RID: 4315 RVA: 0x000025F6 File Offset: 0x000007F6
		public void UpdateIKState(int targetState)
		{
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x0000CD52 File Offset: 0x0000AF52
		private void Awake()
		{
			if (this.cacheFirstPosition)
			{
				this.cachedLocalPosition = base.transform.localPosition;
			}
		}

		// Token: 0x060010DD RID: 4317 RVA: 0x00063F00 File Offset: 0x00062100
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

		// Token: 0x060010DE RID: 4318 RVA: 0x00063F54 File Offset: 0x00062154
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

		// Token: 0x060010DF RID: 4319 RVA: 0x00063FE4 File Offset: 0x000621E4
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

		// Token: 0x060010E0 RID: 4320 RVA: 0x0000CD6D File Offset: 0x0000AF6D
		private void ResetTransformToCachedPosition()
		{
			if (this.cacheFirstPosition)
			{
				base.transform.localPosition = new Vector3(this.cachedLocalPosition.x, this.cachedLocalPosition.y, this.cachedLocalPosition.z);
			}
		}

		// Token: 0x040014EB RID: 5355
		private float smoothedTargetHeightOffset;

		// Token: 0x040014EC RID: 5356
		private float targetHeightOffset;

		// Token: 0x040014ED RID: 5357
		private float smoothdampVelocity;

		// Token: 0x040014EE RID: 5358
		public float minHeight = -0.3f;

		// Token: 0x040014EF RID: 5359
		public float maxHeight = 1f;

		// Token: 0x040014F0 RID: 5360
		public float dampTime = 0.1f;

		// Token: 0x040014F1 RID: 5361
		[Tooltip("The IK weight float parameter if used")]
		public string animatorIKWeightFloat = "";

		// Token: 0x040014F2 RID: 5362
		public Animator animator;

		// Token: 0x040014F3 RID: 5363
		[Tooltip("The target transform will plant without any calls from external IK chains")]
		public bool selfPlant;

		// Token: 0x040014F4 RID: 5364
		public float selfPlantFrequency = 5f;

		// Token: 0x040014F5 RID: 5365
		[Tooltip("Whether or not to cache where the raycast begins. Used when not attached to bones, who reset themselves via animator.")]
		public bool cacheFirstPosition;

		// Token: 0x040014F6 RID: 5366
		private Vector3 cachedLocalPosition;

		// Token: 0x040014F7 RID: 5367
		private float selfPlantTimer;
	}
}
