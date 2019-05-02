using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200053C RID: 1340
	[RequireComponent(typeof(Rigidbody))]
	public class MissileController : MonoBehaviour
	{
		// Token: 0x06001E08 RID: 7688 RVA: 0x00094374 File Offset: 0x00092574
		private void Awake()
		{
			if (!NetworkServer.active)
			{
				base.enabled = false;
				return;
			}
			this.transform = base.transform;
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.torquePID = base.GetComponent<QuaternionPID>();
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x06001E09 RID: 7689 RVA: 0x000943C0 File Offset: 0x000925C0
		private void FixedUpdate()
		{
			this.timer += Time.fixedDeltaTime;
			if (this.timer < this.giveupTimer)
			{
				this.rigidbody.velocity = this.transform.forward * this.maxVelocity;
				if (this.target && this.timer >= this.delayTimer)
				{
					this.rigidbody.velocity = this.transform.forward * (this.maxVelocity + this.timer * this.acceleration);
					Vector3 vector = this.target.transform.position + UnityEngine.Random.insideUnitSphere * this.turbulence - this.transform.position;
					if (vector != Vector3.zero)
					{
						Quaternion rotation = this.transform.rotation;
						Quaternion targetQuat = Util.QuaternionSafeLookRotation(vector);
						this.torquePID.inputQuat = rotation;
						this.torquePID.targetQuat = targetQuat;
						this.rigidbody.angularVelocity = this.torquePID.UpdatePID();
					}
				}
			}
			if (!this.target)
			{
				this.target = this.FindTarget();
			}
			else
			{
				HealthComponent component = this.target.GetComponent<HealthComponent>();
				if (component && !component.alive)
				{
					this.target = this.FindTarget();
				}
			}
			if (this.timer > this.deathTimer)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06001E0A RID: 7690 RVA: 0x00094544 File Offset: 0x00092744
		private Transform FindTarget()
		{
			this.search.searchOrigin = this.transform.position;
			this.search.searchDirection = this.transform.forward;
			this.search.teamMaskFilter.RemoveTeam(this.teamFilter.teamIndex);
			this.search.RefreshCandidates();
			HurtBox hurtBox = this.search.GetResults().FirstOrDefault<HurtBox>();
			if (hurtBox == null)
			{
				return null;
			}
			return hurtBox.transform;
		}

		// Token: 0x04002050 RID: 8272
		private new Transform transform;

		// Token: 0x04002051 RID: 8273
		private Rigidbody rigidbody;

		// Token: 0x04002052 RID: 8274
		private TeamFilter teamFilter;

		// Token: 0x04002053 RID: 8275
		public Transform target;

		// Token: 0x04002054 RID: 8276
		public float maxVelocity;

		// Token: 0x04002055 RID: 8277
		public float rollVelocity;

		// Token: 0x04002056 RID: 8278
		public float acceleration;

		// Token: 0x04002057 RID: 8279
		public float delayTimer;

		// Token: 0x04002058 RID: 8280
		public float giveupTimer = 8f;

		// Token: 0x04002059 RID: 8281
		public float deathTimer = 10f;

		// Token: 0x0400205A RID: 8282
		private float timer;

		// Token: 0x0400205B RID: 8283
		private QuaternionPID torquePID;

		// Token: 0x0400205C RID: 8284
		public float turbulence;

		// Token: 0x0400205D RID: 8285
		public float maxSeekDistance = 40f;

		// Token: 0x0400205E RID: 8286
		private BullseyeSearch search = new BullseyeSearch();
	}
}
