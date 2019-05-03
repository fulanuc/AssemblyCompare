using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000413 RID: 1043
	public class VelocityRandomOnStart : MonoBehaviour
	{
		// Token: 0x06001750 RID: 5968 RVA: 0x0007A030 File Offset: 0x00078230
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

		// Token: 0x04001A5C RID: 6748
		public float minSpeed;

		// Token: 0x04001A5D RID: 6749
		public float maxSpeed;

		// Token: 0x04001A5E RID: 6750
		public Vector3 baseDirection = Vector3.up;

		// Token: 0x04001A5F RID: 6751
		public bool localDirection;

		// Token: 0x04001A60 RID: 6752
		public VelocityRandomOnStart.DirectionMode directionMode;

		// Token: 0x04001A61 RID: 6753
		public float coneAngle = 30f;

		// Token: 0x04001A62 RID: 6754
		[Tooltip("Minimum angular speed in degrees/second.")]
		public float minAngularSpeed;

		// Token: 0x04001A63 RID: 6755
		[Tooltip("Maximum angular speed in degrees/second.")]
		public float maxAngularSpeed;

		// Token: 0x02000414 RID: 1044
		public enum DirectionMode
		{
			// Token: 0x04001A65 RID: 6757
			Sphere,
			// Token: 0x04001A66 RID: 6758
			Hemisphere,
			// Token: 0x04001A67 RID: 6759
			Cone
		}
	}
}
