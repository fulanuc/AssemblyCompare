using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003AD RID: 941
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public class RigidbodyStickOnImpact : MonoBehaviour
	{
		// Token: 0x060013FE RID: 5118 RVA: 0x0000F372 File Offset: 0x0000D572
		private void Start()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x060013FF RID: 5119 RVA: 0x0006F188 File Offset: 0x0006D388
		private void Update()
		{
			if (this.stuck)
			{
				this.stopwatchSinceStuck += Time.deltaTime;
				base.transform.position = this.transformPositionWhenContacted + this.embedDistanceCurve.Evaluate(this.stopwatchSinceStuck) * this.contactNormal;
			}
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x0006F1E4 File Offset: 0x0006D3E4
		private void OnCollisionEnter(Collision collision)
		{
			if (this.stuck || this.rb.isKinematic)
			{
				return;
			}
			if (collision.transform.gameObject.layer != LayerIndex.world.intVal)
			{
				return;
			}
			if (collision.relativeVelocity.sqrMagnitude > this.minimumRelativeVelocityMagnitude * this.minimumRelativeVelocityMagnitude)
			{
				this.stuck = true;
				ContactPoint contact = collision.GetContact(0);
				this.contactNormal = contact.normal;
				this.contactPosition = contact.point;
				this.transformPositionWhenContacted = base.transform.position;
				EffectManager.instance.SpawnEffect(this.stickEffectPrefab, new EffectData
				{
					origin = this.contactPosition,
					rotation = Util.QuaternionSafeLookRotation(this.contactNormal)
				}, false);
				Util.PlaySound(this.stickSoundString, base.gameObject);
				this.rb.isKinematic = true;
				this.rb.velocity = Vector3.zero;
			}
		}

		// Token: 0x040017AE RID: 6062
		private Rigidbody rb;

		// Token: 0x040017AF RID: 6063
		public string stickSoundString;

		// Token: 0x040017B0 RID: 6064
		public GameObject stickEffectPrefab;

		// Token: 0x040017B1 RID: 6065
		public float minimumRelativeVelocityMagnitude;

		// Token: 0x040017B2 RID: 6066
		public AnimationCurve embedDistanceCurve;

		// Token: 0x040017B3 RID: 6067
		private bool stuck;

		// Token: 0x040017B4 RID: 6068
		private float stopwatchSinceStuck;

		// Token: 0x040017B5 RID: 6069
		private Vector3 contactNormal;

		// Token: 0x040017B6 RID: 6070
		private Vector3 contactPosition;

		// Token: 0x040017B7 RID: 6071
		private Vector3 transformPositionWhenContacted;
	}
}
