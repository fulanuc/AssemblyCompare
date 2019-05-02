using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200056B RID: 1387
	public class ProjectileSimple : MonoBehaviour
	{
		// Token: 0x06001F22 RID: 7970 RVA: 0x00016CF5 File Offset: 0x00014EF5
		private void Awake()
		{
			this.transform = base.transform;
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001F23 RID: 7971 RVA: 0x00016D0F File Offset: 0x00014F0F
		private void Start()
		{
			this.UpdateVelocity();
		}

		// Token: 0x06001F24 RID: 7972 RVA: 0x00097FD4 File Offset: 0x000961D4
		private void UpdateVelocity()
		{
			if (this.rigidbody)
			{
				if (this.enableVelocityOverLifetime)
				{
					this.rigidbody.velocity = this.velocity * this.velocityOverLifetime.Evaluate(this.stopwatch / this.lifetime) * this.transform.forward;
					return;
				}
				this.rigidbody.velocity = this.transform.forward * this.velocity;
			}
		}

		// Token: 0x06001F25 RID: 7973 RVA: 0x00098054 File Offset: 0x00096254
		private void Update()
		{
			if (this.updateAfterFiring || this.enableVelocityOverLifetime)
			{
				this.UpdateVelocity();
			}
			this.stopwatch += Time.deltaTime;
			if (this.stopwatch > this.lifetime)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x0400218B RID: 8587
		public float velocity;

		// Token: 0x0400218C RID: 8588
		public float lifetime = 5f;

		// Token: 0x0400218D RID: 8589
		public bool updateAfterFiring;

		// Token: 0x0400218E RID: 8590
		public bool enableVelocityOverLifetime;

		// Token: 0x0400218F RID: 8591
		public AnimationCurve velocityOverLifetime;

		// Token: 0x04002190 RID: 8592
		private float stopwatch;

		// Token: 0x04002191 RID: 8593
		private Rigidbody rigidbody;

		// Token: 0x04002192 RID: 8594
		private new Transform transform;
	}
}
