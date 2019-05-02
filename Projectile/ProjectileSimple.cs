using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200055C RID: 1372
	public class ProjectileSimple : MonoBehaviour
	{
		// Token: 0x06001EB8 RID: 7864 RVA: 0x00016816 File Offset: 0x00014A16
		private void Awake()
		{
			this.transform = base.transform;
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001EB9 RID: 7865 RVA: 0x00016830 File Offset: 0x00014A30
		private void Start()
		{
			this.UpdateVelocity();
		}

		// Token: 0x06001EBA RID: 7866 RVA: 0x000972B8 File Offset: 0x000954B8
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

		// Token: 0x06001EBB RID: 7867 RVA: 0x00097338 File Offset: 0x00095538
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

		// Token: 0x0400214D RID: 8525
		public float velocity;

		// Token: 0x0400214E RID: 8526
		public float lifetime = 5f;

		// Token: 0x0400214F RID: 8527
		public bool updateAfterFiring;

		// Token: 0x04002150 RID: 8528
		public bool enableVelocityOverLifetime;

		// Token: 0x04002151 RID: 8529
		public AnimationCurve velocityOverLifetime;

		// Token: 0x04002152 RID: 8530
		private float stopwatch;

		// Token: 0x04002153 RID: 8531
		private Rigidbody rigidbody;

		// Token: 0x04002154 RID: 8532
		private new Transform transform;
	}
}
