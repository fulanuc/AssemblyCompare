using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000574 RID: 1396
	public class CrouchMecanim : MonoBehaviour
	{
		// Token: 0x06001F4F RID: 8015 RVA: 0x00016ED6 File Offset: 0x000150D6
		private void Awake()
		{
			this.crouchLayer = this.animator.GetLayerIndex("Crouch, Additive");
		}

		// Token: 0x06001F50 RID: 8016 RVA: 0x00098BB0 File Offset: 0x00096DB0
		private void FixedUpdate()
		{
			this.crouchStopwatch -= Time.fixedDeltaTime;
			if (this.crouchStopwatch <= 0f)
			{
				this.crouchStopwatch = 0.5f;
				RaycastHit raycastHit;
				bool flag;
				if (!this.crouchOriginOverride)
				{
					flag = Physics.Raycast(new Ray(base.transform.position - base.transform.up * this.initialVerticalOffset, base.transform.up), out raycastHit, this.duckHeight + this.initialVerticalOffset, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
				}
				else
				{
					flag = Physics.Raycast(new Ray(this.crouchOriginOverride.position - this.crouchOriginOverride.up * this.initialVerticalOffset, this.crouchOriginOverride.up), out raycastHit, this.duckHeight + this.initialVerticalOffset, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
				}
				this.crouchCycle = 0f;
				if (flag)
				{
					this.crouchCycle = Mathf.Clamp01(1f - (raycastHit.distance - this.initialVerticalOffset) / this.duckHeight);
				}
			}
		}

		// Token: 0x06001F51 RID: 8017 RVA: 0x00016EEE File Offset: 0x000150EE
		private void Update()
		{
			this.animator.SetFloat("crouchCycleOffset", this.crouchCycle, this.smoothdamp, Time.deltaTime);
		}

		// Token: 0x06001F52 RID: 8018 RVA: 0x00098CEC File Offset: 0x00096EEC
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.up * this.duckHeight);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, base.transform.position + -base.transform.up * this.initialVerticalOffset);
		}

		// Token: 0x040021BF RID: 8639
		public float duckHeight;

		// Token: 0x040021C0 RID: 8640
		public Animator animator;

		// Token: 0x040021C1 RID: 8641
		public float smoothdamp;

		// Token: 0x040021C2 RID: 8642
		public float initialVerticalOffset;

		// Token: 0x040021C3 RID: 8643
		public Transform crouchOriginOverride;

		// Token: 0x040021C4 RID: 8644
		private int crouchLayer;

		// Token: 0x040021C5 RID: 8645
		private float crouchCycle;

		// Token: 0x040021C6 RID: 8646
		private const float crouchRaycastFrequency = 2f;

		// Token: 0x040021C7 RID: 8647
		private float crouchStopwatch;
	}
}
