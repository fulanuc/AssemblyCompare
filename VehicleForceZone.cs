using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000412 RID: 1042
	[RequireComponent(typeof(Collider))]
	public class VehicleForceZone : MonoBehaviour
	{
		// Token: 0x0600174B RID: 5963 RVA: 0x00011684 File Offset: 0x0000F884
		private void Start()
		{
			this.collider = base.GetComponent<Collider>();
		}

		// Token: 0x0600174C RID: 5964 RVA: 0x00079DF8 File Offset: 0x00077FF8
		public void OnTriggerEnter(Collider other)
		{
			CharacterMotor component = other.GetComponent<CharacterMotor>();
			HealthComponent component2 = other.GetComponent<HealthComponent>();
			if (component && component2)
			{
				Vector3 position = base.transform.position;
				Vector3 normalized = this.vehicleRigidbody.velocity.normalized;
				Vector3 pointVelocity = this.vehicleRigidbody.GetPointVelocity(position);
				Vector3 vector = pointVelocity * this.vehicleRigidbody.mass * this.impactMultiplier;
				float mass = this.vehicleRigidbody.mass;
				Mathf.Pow(pointVelocity.magnitude, 2f);
				float num = component.mass / (component.mass + this.vehicleRigidbody.mass);
				this.vehicleRigidbody.AddForceAtPosition(-vector * num, position);
				Debug.LogFormat("Impulse: {0}, Ratio: {1}", new object[]
				{
					vector.magnitude,
					num
				});
				component2.TakeDamageForce(new DamageInfo
				{
					attacker = base.gameObject,
					force = vector,
					position = position
				}, true);
			}
		}

		// Token: 0x0600174D RID: 5965 RVA: 0x00079F20 File Offset: 0x00078120
		public void OnCollisionEnter(Collision collision)
		{
			Debug.LogFormat("Hit {0}", new object[]
			{
				collision.gameObject
			});
			Rigidbody component = collision.collider.GetComponent<Rigidbody>();
			if (component)
			{
				Debug.Log("Hit?");
				HealthComponent component2 = component.GetComponent<HealthComponent>();
				if (component2)
				{
					Vector3 point = collision.contacts[0].point;
					Vector3 normal = collision.contacts[0].normal;
					this.vehicleRigidbody.GetPointVelocity(point);
					Vector3 impulse = collision.impulse;
					float num = 0f;
					this.vehicleRigidbody.AddForceAtPosition(impulse * num, point);
					Debug.LogFormat("Impulse: {0}, Ratio: {1}", new object[]
					{
						impulse,
						num
					});
					component2.TakeDamageForce(new DamageInfo
					{
						attacker = base.gameObject,
						force = -impulse * (1f - num),
						position = point
					}, true);
				}
			}
		}

		// Token: 0x0600174E RID: 5966 RVA: 0x000025F6 File Offset: 0x000007F6
		private void Update()
		{
		}

		// Token: 0x04001A59 RID: 6745
		public Rigidbody vehicleRigidbody;

		// Token: 0x04001A5A RID: 6746
		public float impactMultiplier;

		// Token: 0x04001A5B RID: 6747
		private Collider collider;
	}
}
