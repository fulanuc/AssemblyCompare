using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003E9 RID: 1001
	[RequireComponent(typeof(Animator))]
	internal class SkateSparks : MonoBehaviour
	{
		// Token: 0x060015FE RID: 5630 RVA: 0x000108FC File Offset: 0x0000EAFC
		private void Awake()
		{
			this.animator = base.GetComponent<Animator>();
		}

		// Token: 0x060015FF RID: 5631 RVA: 0x00074F1C File Offset: 0x0007311C
		private void FixedUpdate()
		{
			float @float = this.animator.GetFloat(SkateSparks.forwardSpeedParam);
			float float2 = this.animator.GetFloat(SkateSparks.rightSpeedParam);
			bool @bool = this.animator.GetBool(SkateSparks.isGroundedParam);
			float num = (float2 - this.previousRightSpeed) * Time.fixedDeltaTime;
			float num2 = (@float - this.previousForwardSpeed) * Time.fixedDeltaTime;
			float num3 = Mathf.Sqrt(num * num + num2 * num2);
			this.sparkAccumulator += num3 * this.sparkFactor;
			if (@bool != this.previousIsGrounded)
			{
				this.sparkAccumulator += 2f * this.sparkFactor;
			}
			if (this.sparkAccumulator > 0f)
			{
				int num4 = Mathf.FloorToInt(this.sparkAccumulator);
				if (@bool)
				{
					if (this.leftParticleSystem)
					{
						this.leftParticleSystem.Emit(num4);
					}
					if (this.rightParticleSystem)
					{
						this.rightParticleSystem.Emit(num4);
					}
				}
				this.sparkAccumulator -= (float)num4;
			}
			this.previousForwardSpeed = @float;
			this.previousRightSpeed = float2;
			this.previousIsGrounded = @bool;
		}

		// Token: 0x0400192C RID: 6444
		public float sparkFactor = 1f;

		// Token: 0x0400192D RID: 6445
		public ParticleSystem leftParticleSystem;

		// Token: 0x0400192E RID: 6446
		public ParticleSystem rightParticleSystem;

		// Token: 0x0400192F RID: 6447
		private Animator animator;

		// Token: 0x04001930 RID: 6448
		private static readonly int forwardSpeedParam = Animator.StringToHash("forwardSpeed");

		// Token: 0x04001931 RID: 6449
		private static readonly int rightSpeedParam = Animator.StringToHash("rightSpeed");

		// Token: 0x04001932 RID: 6450
		private static readonly int isGroundedParam = Animator.StringToHash("isGrounded");

		// Token: 0x04001933 RID: 6451
		private float previousForwardSpeed;

		// Token: 0x04001934 RID: 6452
		private float previousRightSpeed;

		// Token: 0x04001935 RID: 6453
		private bool previousIsGrounded = true;

		// Token: 0x04001936 RID: 6454
		private float sparkAccumulator;
	}
}
