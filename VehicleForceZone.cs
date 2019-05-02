using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000418 RID: 1048
	[RequireComponent(typeof(Collider))]
	public class VehicleForceZone : MonoBehaviour
	{
		// Token: 0x0600178E RID: 6030 RVA: 0x00011AB0 File Offset: 0x0000FCB0
		private void Start()
		{
			this.collider = base.GetComponent<Collider>();
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x0007A3B8 File Offset: 0x000785B8
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

		// Token: 0x06001790 RID: 6032 RVA: 0x0007A4E0 File Offset: 0x000786E0
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

		// Token: 0x06001791 RID: 6033 RVA: 0x000025DA File Offset: 0x000007DA
		private void Update()
		{
		}

		// Token: 0x04001A82 RID: 6786
		public Rigidbody vehicleRigidbody;

		// Token: 0x04001A83 RID: 6787
		public float impactMultiplier;

		// Token: 0x04001A84 RID: 6788
		private Collider collider;
	}
}
