using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000565 RID: 1381
	public class CrouchMecanim : MonoBehaviour
	{
		// Token: 0x06001EE5 RID: 7909 RVA: 0x000169F7 File Offset: 0x00014BF7
		private void Awake()
		{
			this.crouchLayer = this.animator.GetLayerIndex("Crouch, Additive");
		}

		// Token: 0x06001EE6 RID: 7910 RVA: 0x00097E94 File Offset: 0x00096094
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

		// Token: 0x06001EE7 RID: 7911 RVA: 0x00016A0F File Offset: 0x00014C0F
		private void Update()
		{
			this.animator.SetFloat("crouchCycleOffset", this.crouchCycle, this.smoothdamp, Time.deltaTime);
		}

		// Token: 0x06001EE8 RID: 7912 RVA: 0x00097FD0 File Offset: 0x000961D0
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.up * this.duckHeight);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, base.transform.position + -base.transform.up * this.initialVerticalOffset);
		}

		// Token: 0x04002181 RID: 8577
		public float duckHeight;

		// Token: 0x04002182 RID: 8578
		public Animator animator;

		// Token: 0x04002183 RID: 8579
		public float smoothdamp;

		// Token: 0x04002184 RID: 8580
		public float initialVerticalOffset;

		// Token: 0x04002185 RID: 8581
		public Transform crouchOriginOverride;

		// Token: 0x04002186 RID: 8582
		private int crouchLayer;

		// Token: 0x04002187 RID: 8583
		private float crouchCycle;

		// Token: 0x04002188 RID: 8584
		private const float crouchRaycastFrequency = 2f;

		// Token: 0x04002189 RID: 8585
		private float crouchStopwatch;
	}
}
