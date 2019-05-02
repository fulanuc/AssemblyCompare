using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003F4 RID: 1012
	public class StriderLegController : MonoBehaviour
	{
		// Token: 0x06001641 RID: 5697 RVA: 0x0007609C File Offset: 0x0007429C
		public Vector3 GetCenterOfStance()
		{
			Vector3 a = Vector3.zero;
			for (int i = 0; i < this.feet.Length; i++)
			{
				a += this.feet[i].transform.position;
			}
			return a / (float)this.feet.Length;
		}

		// Token: 0x06001642 RID: 5698 RVA: 0x000760F0 File Offset: 0x000742F0
		private void Awake()
		{
			for (int i = 0; i < this.feet.Length; i++)
			{
				this.feet[i].footState = StriderLegController.FootState.Planted;
				this.feet[i].plantPosition = this.feet[i].referenceTransform.position;
				this.feet[i].trailingTargetPosition = this.feet[i].plantPosition;
			}
		}

		// Token: 0x06001643 RID: 5699 RVA: 0x0007616C File Offset: 0x0007436C
		private void Update()
		{
			int num = 0;
			this.footRaycastTimer -= Time.deltaTime;
			for (int i = 0; i < this.feet.Length; i++)
			{
				Transform transform = this.feet[i].transform;
				Transform referenceTransform = this.feet[i].referenceTransform;
				Vector3 position = transform.position;
				Vector3 vector = Vector3.zero;
				float num2 = 0f;
				StriderLegController.FootState footState = this.feet[i].footState;
				if (footState != StriderLegController.FootState.Planted)
				{
					if (footState == StriderLegController.FootState.Replanting)
					{
						StriderLegController.FootInfo[] array = this.feet;
						int num3 = i;
						array[num3].stopwatch = array[num3].stopwatch + Time.deltaTime;
						Vector3 plantPosition = this.feet[i].plantPosition;
						Vector3 vector2 = referenceTransform.position;
						vector2 += Vector3.ProjectOnPlane(vector2 - plantPosition, Vector3.up).normalized * this.overstepDistance;
						float num4 = this.lerpCurve.Evaluate(this.feet[i].stopwatch / this.replantDuration);
						vector = Vector3.Lerp(plantPosition, vector2, num4);
						num2 = Mathf.Sin(num4 * 3.14159274f) * this.replantHeight;
						if (this.feet[i].stopwatch >= this.replantDuration)
						{
							this.feet[i].plantPosition = vector2;
							this.feet[i].stopwatch = 0f;
							this.feet[i].footState = StriderLegController.FootState.Planted;
							Util.PlaySound(this.footPlantString, transform.gameObject);
						}
					}
				}
				else
				{
					num++;
					vector = this.feet[i].plantPosition;
					if ((referenceTransform.position - vector).sqrMagnitude > this.stabilityRadius * this.stabilityRadius)
					{
						this.feet[i].footState = StriderLegController.FootState.Replanting;
						Util.PlaySound(this.footMoveString, transform.gameObject);
					}
				}
				Ray ray = default(Ray);
				ray.direction = transform.TransformDirection(this.footRaycastDirection.normalized);
				ray.origin = vector - ray.direction * this.raycastVerticalOffset;
				RaycastHit raycastHit;
				if (this.footRaycastTimer <= 0f && Physics.Raycast(ray, out raycastHit, this.maxRaycastDistance + this.raycastVerticalOffset, LayerIndex.world.mask))
				{
					vector = raycastHit.point;
				}
				vector.y += num2;
				this.feet[i].trailingTargetPosition = Vector3.SmoothDamp(this.feet[i].trailingTargetPosition, vector, ref this.feet[i].velocity, this.footDampTime);
				transform.position = this.feet[i].trailingTargetPosition;
			}
			if (this.rootTransform)
			{
				Vector3 localPosition = this.rootTransform.localPosition;
				float num5 = (1f - (float)num / (float)this.feet.Length) * this.rootOffsetHeight;
				float target = localPosition.z - num5;
				float z = Mathf.SmoothDamp(localPosition.z, target, ref this.rootVelocity, this.rootSmoothDamp);
				this.rootTransform.localPosition = new Vector3(localPosition.x, localPosition.y, z);
			}
			if (this.footRaycastTimer <= 0f)
			{
				this.footRaycastTimer = 1f / this.footRaycastFrequency;
			}
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x0000CEF0 File Offset: 0x0000B0F0
		public Vector3 GetArcPosition(Vector3 start, Vector3 end, float arcHeight, float t)
		{
			return Vector3.Lerp(start, end, Mathf.Sin(t * 3.14159274f * 0.5f)) + new Vector3(0f, Mathf.Sin(t * 3.14159274f) * arcHeight, 0f);
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x000764FC File Offset: 0x000746FC
		public void OnDrawGizmos()
		{
			for (int i = 0; i < this.feet.Length; i++)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawRay(this.feet[i].transform.position, this.feet[i].transform.TransformVector(this.footRaycastDirection));
			}
		}

		// Token: 0x04001971 RID: 6513
		[Header("Foot Settings")]
		public Transform centerOfGravity;

		// Token: 0x04001972 RID: 6514
		public StriderLegController.FootInfo[] feet;

		// Token: 0x04001973 RID: 6515
		public Vector3 footRaycastDirection;

		// Token: 0x04001974 RID: 6516
		public float raycastVerticalOffset;

		// Token: 0x04001975 RID: 6517
		public float maxRaycastDistance;

		// Token: 0x04001976 RID: 6518
		public float footDampTime;

		// Token: 0x04001977 RID: 6519
		public float stabilityRadius;

		// Token: 0x04001978 RID: 6520
		public float replantDuration;

		// Token: 0x04001979 RID: 6521
		public float replantHeight;

		// Token: 0x0400197A RID: 6522
		public float overstepDistance;

		// Token: 0x0400197B RID: 6523
		public AnimationCurve lerpCurve;

		// Token: 0x0400197C RID: 6524
		public string footPlantString;

		// Token: 0x0400197D RID: 6525
		public string footMoveString;

		// Token: 0x0400197E RID: 6526
		public float footRaycastFrequency = 0.2f;

		// Token: 0x0400197F RID: 6527
		[Header("Root Settings")]
		public Transform rootTransform;

		// Token: 0x04001980 RID: 6528
		public float rootSpringConstant;

		// Token: 0x04001981 RID: 6529
		public float rootDampingConstant;

		// Token: 0x04001982 RID: 6530
		public float rootOffsetHeight;

		// Token: 0x04001983 RID: 6531
		public float rootSmoothDamp;

		// Token: 0x04001984 RID: 6532
		private float rootVelocity;

		// Token: 0x04001985 RID: 6533
		private float footRaycastTimer;

		// Token: 0x020003F5 RID: 1013
		[Serializable]
		public struct FootInfo
		{
			// Token: 0x04001986 RID: 6534
			public Transform transform;

			// Token: 0x04001987 RID: 6535
			public Transform referenceTransform;

			// Token: 0x04001988 RID: 6536
			[HideInInspector]
			public Vector3 velocity;

			// Token: 0x04001989 RID: 6537
			[HideInInspector]
			public StriderLegController.FootState footState;

			// Token: 0x0400198A RID: 6538
			[HideInInspector]
			public Vector3 plantPosition;

			// Token: 0x0400198B RID: 6539
			[HideInInspector]
			public Vector3 trailingTargetPosition;

			// Token: 0x0400198C RID: 6540
			[HideInInspector]
			public float stopwatch;
		}

		// Token: 0x020003F6 RID: 1014
		public enum FootState
		{
			// Token: 0x0400198E RID: 6542
			Planted,
			// Token: 0x0400198F RID: 6543
			Replanting
		}
	}
}
