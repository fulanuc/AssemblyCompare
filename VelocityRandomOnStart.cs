using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000419 RID: 1049
	public class VelocityRandomOnStart : MonoBehaviour
	{
		// Token: 0x06001793 RID: 6035 RVA: 0x0007A5F0 File Offset: 0x000787F0
		private void Start()
		{
			if (NetworkServer.active)
			{
				Rigidbody component = base.GetComponent<Rigidbody>();
				if (component)
				{
					float num = (this.minSpeed != this.maxSpeed) ? UnityEngine.Random.Range(this.minSpeed, this.maxSpeed) : this.minSpeed;
					if (num != 0f)
					{
						Vector3 vector = Vector3.zero;
						Vector3 vector2 = this.localDirection ? (base.transform.rotation * this.baseDirection) : this.baseDirection;
						switch (this.directionMode)
						{
						case VelocityRandomOnStart.DirectionMode.Sphere:
							vector = UnityEngine.Random.onUnitSphere;
							break;
						case VelocityRandomOnStart.DirectionMode.Hemisphere:
							vector = UnityEngine.Random.onUnitSphere;
							if (Vector3.Dot(vector, vector2) < 0f)
							{
								vector = -vector;
							}
							break;
						case VelocityRandomOnStart.DirectionMode.Cone:
							vector = Util.ApplySpread(vector2, 0f, this.coneAngle, 1f, 1f, 0f, 0f);
							break;
						}
						component.velocity = vector * num;
					}
					float num2 = (this.minAngularSpeed != this.maxAngularSpeed) ? UnityEngine.Random.Range(this.minAngularSpeed, this.maxAngularSpeed) : this.minAngularSpeed;
					if (num2 != 0f)
					{
						component.angularVelocity = UnityEngine.Random.onUnitSphere * (num2 * 0.0174532924f);
					}
				}
			}
		}

		// Token: 0x04001A85 RID: 6789
		public float minSpeed;

		// Token: 0x04001A86 RID: 6790
		public float maxSpeed;

		// Token: 0x04001A87 RID: 6791
		public Vector3 baseDirection = Vector3.up;

		// Token: 0x04001A88 RID: 6792
		public bool localDirection;

		// Token: 0x04001A89 RID: 6793
		public VelocityRandomOnStart.DirectionMode directionMode;

		// Token: 0x04001A8A RID: 6794
		public float coneAngle = 30f;

		// Token: 0x04001A8B RID: 6795
		[Tooltip("Minimum angular speed in degrees/second.")]
		public float minAngularSpeed;

		// Token: 0x04001A8C RID: 6796
		[Tooltip("Maximum angular speed in degrees/second.")]
		public float maxAngularSpeed;

		// Token: 0x0200041A RID: 1050
		public enum DirectionMode
		{
			// Token: 0x04001A8E RID: 6798
			Sphere,
			// Token: 0x04001A8F RID: 6799
			Hemisphere,
			// Token: 0x04001A90 RID: 6800
			Cone
		}
	}
}
