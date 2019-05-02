using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000544 RID: 1348
	[RequireComponent(typeof(Rigidbody))]
	public class DaggerController : MonoBehaviour
	{
		// Token: 0x06001E5A RID: 7770 RVA: 0x00016286 File Offset: 0x00014486
		private void Awake()
		{
			this.transform = base.transform;
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.rigidbody.AddRelativeForce(UnityEngine.Random.insideUnitSphere * 50f);
		}

		// Token: 0x06001E5B RID: 7771 RVA: 0x000945C8 File Offset: 0x000927C8
		private void FixedUpdate()
		{
			this.timer += Time.fixedDeltaTime;
			if (this.timer < this.giveupTimer)
			{
				if (this.target)
				{
					Vector3 vector = this.target.transform.position - this.transform.position;
					if (vector != Vector3.zero)
					{
						this.transform.rotation = Util.QuaternionSafeLookRotation(vector);
					}
					if (this.timer >= this.delayTimer)
					{
						this.rigidbody.AddForce(this.transform.forward * this.acceleration);
						if (!this.hasPlayedSound)
						{
							Util.PlaySound("Play_item_proc_dagger_fly", base.gameObject);
							this.hasPlayedSound = true;
						}
					}
				}
			}
			else
			{
				this.rigidbody.useGravity = true;
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

		// Token: 0x06001E5C RID: 7772 RVA: 0x00094700 File Offset: 0x00092900
		private Transform FindTarget()
		{
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Monster);
			float num = 99999f;
			Transform result = null;
			for (int i = 0; i < teamMembers.Count; i++)
			{
				float num2 = Vector3.SqrMagnitude(teamMembers[i].transform.position - this.transform.position);
				if (num2 < num)
				{
					num = num2;
					result = teamMembers[i].transform;
				}
			}
			return result;
		}

		// Token: 0x04002064 RID: 8292
		private new Transform transform;

		// Token: 0x04002065 RID: 8293
		private Rigidbody rigidbody;

		// Token: 0x04002066 RID: 8294
		public Transform target;

		// Token: 0x04002067 RID: 8295
		public float acceleration;

		// Token: 0x04002068 RID: 8296
		public float delayTimer;

		// Token: 0x04002069 RID: 8297
		public float giveupTimer = 8f;

		// Token: 0x0400206A RID: 8298
		public float deathTimer = 10f;

		// Token: 0x0400206B RID: 8299
		private float timer;

		// Token: 0x0400206C RID: 8300
		public float turbulence;

		// Token: 0x0400206D RID: 8301
		private bool hasPlayedSound;
	}
}
