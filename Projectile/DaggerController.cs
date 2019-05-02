using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000535 RID: 1333
	[RequireComponent(typeof(Rigidbody))]
	public class DaggerController : MonoBehaviour
	{
		// Token: 0x06001DF0 RID: 7664 RVA: 0x00015DA7 File Offset: 0x00013FA7
		private void Awake()
		{
			this.transform = base.transform;
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.rigidbody.AddRelativeForce(UnityEngine.Random.insideUnitSphere * 50f);
		}

		// Token: 0x06001DF1 RID: 7665 RVA: 0x000938AC File Offset: 0x00091AAC
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

		// Token: 0x06001DF2 RID: 7666 RVA: 0x000939E4 File Offset: 0x00091BE4
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

		// Token: 0x04002026 RID: 8230
		private new Transform transform;

		// Token: 0x04002027 RID: 8231
		private Rigidbody rigidbody;

		// Token: 0x04002028 RID: 8232
		public Transform target;

		// Token: 0x04002029 RID: 8233
		public float acceleration;

		// Token: 0x0400202A RID: 8234
		public float delayTimer;

		// Token: 0x0400202B RID: 8235
		public float giveupTimer = 8f;

		// Token: 0x0400202C RID: 8236
		public float deathTimer = 10f;

		// Token: 0x0400202D RID: 8237
		private float timer;

		// Token: 0x0400202E RID: 8238
		public float turbulence;

		// Token: 0x0400202F RID: 8239
		private bool hasPlayedSound;
	}
}
