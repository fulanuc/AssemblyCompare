using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200054B RID: 1355
	[RequireComponent(typeof(Rigidbody))]
	public class MissileController : MonoBehaviour
	{
		// Token: 0x06001E72 RID: 7794 RVA: 0x00095090 File Offset: 0x00093290
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

		// Token: 0x06001E73 RID: 7795 RVA: 0x000950DC File Offset: 0x000932DC
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

		// Token: 0x06001E74 RID: 7796 RVA: 0x00095260 File Offset: 0x00093460
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

		// Token: 0x0400208E RID: 8334
		private new Transform transform;

		// Token: 0x0400208F RID: 8335
		private Rigidbody rigidbody;

		// Token: 0x04002090 RID: 8336
		private TeamFilter teamFilter;

		// Token: 0x04002091 RID: 8337
		public Transform target;

		// Token: 0x04002092 RID: 8338
		public float maxVelocity;

		// Token: 0x04002093 RID: 8339
		public float rollVelocity;

		// Token: 0x04002094 RID: 8340
		public float acceleration;

		// Token: 0x04002095 RID: 8341
		public float delayTimer;

		// Token: 0x04002096 RID: 8342
		public float giveupTimer = 8f;

		// Token: 0x04002097 RID: 8343
		public float deathTimer = 10f;

		// Token: 0x04002098 RID: 8344
		private float timer;

		// Token: 0x04002099 RID: 8345
		private QuaternionPID torquePID;

		// Token: 0x0400209A RID: 8346
		public float turbulence;

		// Token: 0x0400209B RID: 8347
		public float maxSeekDistance = 40f;

		// Token: 0x0400209C RID: 8348
		private BullseyeSearch search = new BullseyeSearch();
	}
}
