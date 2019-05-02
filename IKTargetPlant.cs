using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000337 RID: 823
	public class IKTargetPlant : MonoBehaviour, IIKTargetBehavior
	{
		// Token: 0x060010F7 RID: 4343 RVA: 0x0000CED0 File Offset: 0x0000B0D0
		private void Awake()
		{
			this.ikChain = base.GetComponent<IKSimpleChain>();
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x0000CEDE File Offset: 0x0000B0DE
		public void UpdateIKState(int targetState)
		{
			if (this.ikState != IKTargetPlant.IKState.Reset)
			{
				this.ikState = (IKTargetPlant.IKState)targetState;
			}
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x0000CEF0 File Offset: 0x0000B0F0
		public Vector3 GetArcPosition(Vector3 start, Vector3 end, float arcHeight, float t)
		{
			return Vector3.Lerp(start, end, Mathf.Sin(t * 3.14159274f * 0.5f)) + new Vector3(0f, Mathf.Sin(t * 3.14159274f) * arcHeight, 0f);
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x00064338 File Offset: 0x00062538
		public void UpdateIKTargetPosition()
		{
			if (this.animator)
			{
				this.ikWeight = this.animator.GetFloat(this.animatorIKWeightFloat);
			}
			else
			{
				this.ikWeight = 1f;
			}
			IKTargetPlant.IKState ikstate = this.ikState;
			if (ikstate != IKTargetPlant.IKState.Plant)
			{
				if (ikstate == IKTargetPlant.IKState.Reset)
				{
					this.resetTimer += Time.deltaTime;
					this.isPlanted = false;
					this.RaycastIKTarget(base.transform.position);
					base.transform.position = this.GetArcPosition(this.plantPosition, this.targetPosition, this.arcHeight, this.resetTimer / this.timeToReset);
					if (this.resetTimer >= this.timeToReset)
					{
						this.ikState = IKTargetPlant.IKState.Plant;
						this.isPlanted = true;
						this.plantPosition = this.targetPosition;
						UnityEngine.Object.Instantiate<GameObject>(this.plantEffect, this.plantPosition, Quaternion.identity);
					}
				}
			}
			else
			{
				Vector3 position = base.transform.position;
				this.RaycastIKTarget(position);
				if (!this.isPlanted)
				{
					this.plantPosition = this.targetPosition;
					base.transform.position = this.plantPosition;
					this.isPlanted = true;
					if (this.plantEffect)
					{
						UnityEngine.Object.Instantiate<GameObject>(this.plantEffect, this.plantPosition, Quaternion.identity);
					}
				}
				else
				{
					base.transform.position = Vector3.Lerp(position, this.plantPosition, this.ikWeight);
				}
				Vector3 vector = position - base.transform.position;
				vector.y = 0f;
				if (this.ikChain.LegTooShort(this.legScale) || vector.sqrMagnitude >= this.maxXZPositionalError * this.maxXZPositionalError)
				{
					this.plantPosition = base.transform.position;
					this.ikState = IKTargetPlant.IKState.Reset;
					if (this.animator)
					{
						this.animator.SetTrigger(this.animatorLiftTrigger);
					}
					this.resetTimer = 0f;
				}
			}
			base.transform.position = Vector3.SmoothDamp(this.lastTransformPosition, base.transform.position, ref this.smoothDampRefVelocity, this.smoothDampTime);
			this.lastTransformPosition = base.transform.position;
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x00064574 File Offset: 0x00062774
		public void RaycastIKTarget(Vector3 position)
		{
			RaycastHit raycastHit;
			if (this.useSpherecast)
			{
				Physics.SphereCast(position + Vector3.up * -this.minHeight, this.spherecastRadius, Vector3.down, out raycastHit, this.maxHeight - this.minHeight, LayerIndex.world.mask);
			}
			else
			{
				Physics.Raycast(position + Vector3.up * -this.minHeight, Vector3.down, out raycastHit, this.maxHeight - this.minHeight, LayerIndex.world.mask);
			}
			if (raycastHit.collider)
			{
				this.targetPosition = raycastHit.point;
				return;
			}
			this.targetPosition = position;
		}

		// Token: 0x0400150C RID: 5388
		[Tooltip("The max offset to step up")]
		public float minHeight = -0.3f;

		// Token: 0x0400150D RID: 5389
		[Tooltip("The max offset to step down")]
		public float maxHeight = 1f;

		// Token: 0x0400150E RID: 5390
		[Tooltip("The strength of the IK as a lerp (0-1)")]
		public float ikWeight = 1f;

		// Token: 0x0400150F RID: 5391
		[Tooltip("The time to restep")]
		public float timeToReset = 0.6f;

		// Token: 0x04001510 RID: 5392
		[Tooltip("The max positional IK error before restepping")]
		public float maxXZPositionalError = 4f;

		// Token: 0x04001511 RID: 5393
		public GameObject plantEffect;

		// Token: 0x04001512 RID: 5394
		public Animator animator;

		// Token: 0x04001513 RID: 5395
		[Tooltip("The IK weight float parameter if used")]
		public string animatorIKWeightFloat;

		// Token: 0x04001514 RID: 5396
		[Tooltip("The lift animation trigger string if used")]
		public string animatorLiftTrigger;

		// Token: 0x04001515 RID: 5397
		[Tooltip("The scale of the leg for calculating if the leg is too short to reach the IK target")]
		public float legScale = 1f;

		// Token: 0x04001516 RID: 5398
		[Tooltip("The height of the step arc")]
		public float arcHeight = 1f;

		// Token: 0x04001517 RID: 5399
		[Tooltip("The smoothing duration for the IK. Higher will be smoother but will be delayed.")]
		public float smoothDampTime = 0.1f;

		// Token: 0x04001518 RID: 5400
		[Tooltip("Spherecasts will have more hits but take higher performance.")]
		public bool useSpherecast;

		// Token: 0x04001519 RID: 5401
		public float spherecastRadius = 0.5f;

		// Token: 0x0400151A RID: 5402
		public IKTargetPlant.IKState ikState;

		// Token: 0x0400151B RID: 5403
		private bool isPlanted;

		// Token: 0x0400151C RID: 5404
		private Vector3 lastTransformPosition;

		// Token: 0x0400151D RID: 5405
		private Vector3 smoothDampRefVelocity;

		// Token: 0x0400151E RID: 5406
		private Vector3 targetPosition;

		// Token: 0x0400151F RID: 5407
		private Vector3 plantPosition;

		// Token: 0x04001520 RID: 5408
		private IKSimpleChain ikChain;

		// Token: 0x04001521 RID: 5409
		private float resetTimer;

		// Token: 0x02000338 RID: 824
		public enum IKState
		{
			// Token: 0x04001523 RID: 5411
			Plant,
			// Token: 0x04001524 RID: 5412
			Reset
		}
	}
}
