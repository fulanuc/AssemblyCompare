using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200045D RID: 1117
	public class LocalNavigator
	{
		// Token: 0x060018FF RID: 6399 RVA: 0x00081690 File Offset: 0x0007F890
		public void SetBody(CharacterBody newBody)
		{
			this.transform = null;
			this.characterCollider = null;
			this.body = newBody;
			if (this.body)
			{
				this.transform = this.body.GetComponent<Transform>();
				this.motor = this.body.GetComponent<CharacterMotor>();
				this.currentPosition = this.transform.position;
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06001900 RID: 6400 RVA: 0x00012C3F File Offset: 0x00010E3F
		// (set) Token: 0x06001901 RID: 6401 RVA: 0x00012C47 File Offset: 0x00010E47
		public Vector3 moveVector { get; private set; }

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06001902 RID: 6402 RVA: 0x00012C50 File Offset: 0x00010E50
		// (set) Token: 0x06001903 RID: 6403 RVA: 0x00012C58 File Offset: 0x00010E58
		public float jumpSpeed { get; private set; }

		// Token: 0x06001904 RID: 6404 RVA: 0x000816F4 File Offset: 0x0007F8F4
		public void Update(float deltaTime)
		{
			if (this.transform)
			{
				this.currentPosition = this.transform.position;
			}
			Vector3 vector = this.targetPosition - this.currentPosition;
			float magnitude = vector.magnitude;
			if (magnitude == 0f)
			{
				this.moveVector = Vector3.zero;
			}
			else
			{
				float num = this.body ? this.body.moveSpeed : 0f;
				float b = this.body ? this.body.maxJumpHeight : 0f;
				float a = 0f;
				float num2 = 0f;
				if (this.characterCollider)
				{
					Bounds bounds = this.characterCollider.bounds;
					a = (bounds.max.y - this.currentPosition.y) * 0.5f;
					num2 = bounds.min.y;
				}
				Vector3 vector2 = this.currentPosition;
				vector2.y += Mathf.Min(a, b);
				LayerMask mask = LayerIndex.world.mask | LayerIndex.defaultLayer.mask;
				float num3 = this.lookAheadDistance;
				bool flag = !Physics.Raycast(vector2, vector, num3, mask);
				Debug.DrawRay(vector2, vector.normalized * num3, flag ? Color.yellow : Color.red, deltaTime);
				float num4 = 0f;
				if (this.backupRestrictionTimer <= 0f)
				{
					if (!flag)
					{
						float num5 = 45f;
						bool flag2 = !Physics.Raycast(vector2, Quaternion.Euler(0f, num5, 0f) * vector, num3, mask);
						bool flag3 = !Physics.Raycast(vector2, Quaternion.Euler(0f, -num5, 0f) * vector, num3, mask);
						Debug.DrawRay(vector2, (Quaternion.Euler(0f, num5, 0f) * vector).normalized * num3, flag2 ? Color.yellow : Color.red, deltaTime);
						Debug.DrawRay(vector2, (Quaternion.Euler(0f, -num5, 0f) * vector).normalized * num3, flag3 ? Color.yellow : Color.red, deltaTime);
						int num6 = (flag2 ? -1 : 0) + (flag3 ? 1 : 0);
						if (num6 == 0)
						{
							num6 = ((UnityEngine.Random.Range(0, 1) == 1) ? -1 : 1);
						}
						if (this.walkFrustration > 0.5f)
						{
							num4 = vector.y + 1f;
						}
						else
						{
							this.moveVector = Quaternion.Euler(0f, -num5 * (float)num6, 0f) * (vector / magnitude);
						}
					}
					else
					{
						this.moveVector = vector / magnitude;
					}
				}
				if (num != 0f)
				{
					float num7 = 4f;
					float num8 = num * 0.5f;
					float magnitude2 = ((this.currentPosition - this.previousPosition) / deltaTime).magnitude;
					float num9 = num8 - magnitude2;
					this.walkFrustration = Mathf.Clamp01(this.walkFrustration + num7 * num9 / num8 * deltaTime);
				}
				if (this.motor)
				{
					bool isGrounded = this.motor.isGrounded;
				}
				if (this.walkFrustration >= 0.5f && flag)
				{
					float num10 = this.motor ? this.motor.capsuleRadius : 1f;
					Vector3 a2 = vector;
					a2.y = 0f;
					a2.Normalize();
					float num11 = vector2.y - num2;
					RaycastHit raycastHit;
					if (Physics.Raycast(vector2 + a2 * (num10 + 0.25f), Vector3.down, out raycastHit, num11, mask))
					{
						num4 = num11 - raycastHit.distance;
					}
				}
				if (num4 >= (this.motor ? this.motor.stepOffset : 0f))
				{
					this.jumpSpeed = Trajectory.CalculateInitialYSpeed(0.5f, num4 + 0.1f);
				}
				else
				{
					this.jumpSpeed = 0f;
				}
			}
			this.avoidanceTimer -= deltaTime;
			this.walkFrustration -= deltaTime * 0.25f;
			this.backupRestrictionTimer -= deltaTime;
			this.previousPosition = this.currentPosition;
		}

		// Token: 0x04001C5E RID: 7262
		private Vector3 currentPosition;

		// Token: 0x04001C5F RID: 7263
		private Vector3 previousPosition;

		// Token: 0x04001C60 RID: 7264
		public Vector3 targetPosition;

		// Token: 0x04001C61 RID: 7265
		public float lookAheadDistance = 3.5f;

		// Token: 0x04001C62 RID: 7266
		public float avoidanceDuration = 0.5f;

		// Token: 0x04001C63 RID: 7267
		private float avoidanceTimer;

		// Token: 0x04001C64 RID: 7268
		private CharacterBody body;

		// Token: 0x04001C65 RID: 7269
		private Transform transform;

		// Token: 0x04001C66 RID: 7270
		private CharacterMotor motor;

		// Token: 0x04001C67 RID: 7271
		private Collider characterCollider;

		// Token: 0x04001C6A RID: 7274
		private float walkFrustration;

		// Token: 0x04001C6B RID: 7275
		private float backupRestrictionTimer;
	}
}
