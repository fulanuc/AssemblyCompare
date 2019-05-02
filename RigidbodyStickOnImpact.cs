using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A8 RID: 936
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public class RigidbodyStickOnImpact : MonoBehaviour
	{
		// Token: 0x060013E1 RID: 5089 RVA: 0x0000F1CE File Offset: 0x0000D3CE
		private void Start()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x060013E2 RID: 5090 RVA: 0x0006EF80 File Offset: 0x0006D180
		private void Update()
		{
			if (this.stuck)
			{
				this.stopwatchSinceStuck += Time.deltaTime;
				base.transform.position = this.transformPositionWhenContacted + this.embedDistanceCurve.Evaluate(this.stopwatchSinceStuck) * this.contactNormal;
			}
		}

		// Token: 0x060013E3 RID: 5091 RVA: 0x0006EFDC File Offset: 0x0006D1DC
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

		// Token: 0x04001794 RID: 6036
		private Rigidbody rb;

		// Token: 0x04001795 RID: 6037
		public string stickSoundString;

		// Token: 0x04001796 RID: 6038
		public GameObject stickEffectPrefab;

		// Token: 0x04001797 RID: 6039
		public float minimumRelativeVelocityMagnitude;

		// Token: 0x04001798 RID: 6040
		public AnimationCurve embedDistanceCurve;

		// Token: 0x04001799 RID: 6041
		private bool stuck;

		// Token: 0x0400179A RID: 6042
		private float stopwatchSinceStuck;

		// Token: 0x0400179B RID: 6043
		private Vector3 contactNormal;

		// Token: 0x0400179C RID: 6044
		private Vector3 contactPosition;

		// Token: 0x0400179D RID: 6045
		private Vector3 transformPositionWhenContacted;
	}
}
