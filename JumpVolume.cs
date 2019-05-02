using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200034A RID: 842
	public class JumpVolume : MonoBehaviour
	{
		// Token: 0x0600118F RID: 4495 RVA: 0x00066534 File Offset: 0x00064734
		public void OnTriggerStay(Collider other)
		{
			CharacterMotor component = other.GetComponent<CharacterMotor>();
			if (component && component.hasEffectiveAuthority)
			{
				if (!component.disableAirControlUntilCollision)
				{
					Util.PlaySound(this.jumpSoundString, base.gameObject);
				}
				component.velocity = this.jumpVelocity;
				component.disableAirControlUntilCollision = true;
				component.Motor.ForceUnground();
			}
		}

		// Token: 0x06001190 RID: 4496 RVA: 0x00066590 File Offset: 0x00064790
		private void OnDrawGizmos()
		{
			int num = 20;
			float d = this.time / (float)num;
			Vector3 vector = base.transform.position;
			Vector3 position = base.transform.position;
			Vector3 a = this.jumpVelocity;
			Gizmos.color = Color.yellow;
			for (int i = 0; i <= num; i++)
			{
				Vector3 vector2 = vector + a * d;
				a += Physics.gravity * d;
				Gizmos.DrawLine(vector2, vector);
				vector = vector2;
			}
		}

		// Token: 0x04001587 RID: 5511
		public Transform targetElevationTransform;

		// Token: 0x04001588 RID: 5512
		public Vector3 jumpVelocity;

		// Token: 0x04001589 RID: 5513
		public float time;

		// Token: 0x0400158A RID: 5514
		public string jumpSoundString;
	}
}
