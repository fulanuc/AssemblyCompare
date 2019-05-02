﻿using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003EE RID: 1006
	public class StriderLegController : MonoBehaviour
	{
		// Token: 0x06001604 RID: 5636 RVA: 0x00075A64 File Offset: 0x00073C64
		public Vector3 GetCenterOfStance()
		{
			Vector3 a = Vector3.zero;
			for (int i = 0; i < this.feet.Length; i++)
			{
				a += this.feet[i].transform.position;
			}
			return a / (float)this.feet.Length;
		}

		// Token: 0x06001605 RID: 5637 RVA: 0x00075AB8 File Offset: 0x00073CB8
		private void Awake()
		{
			for (int i = 0; i < this.feet.Length; i++)
			{
				this.feet[i].footState = StriderLegController.FootState.Planted;
				this.feet[i].plantPosition = this.feet[i].referenceTransform.position;
				this.feet[i].trailingTargetPosition = this.feet[i].plantPosition;
			}
		}

		// Token: 0x06001606 RID: 5638 RVA: 0x00075B34 File Offset: 0x00073D34
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

		// Token: 0x06001607 RID: 5639 RVA: 0x0000CE07 File Offset: 0x0000B007
		public Vector3 GetArcPosition(Vector3 start, Vector3 end, float arcHeight, float t)
		{
			return Vector3.Lerp(start, end, Mathf.Sin(t * 3.14159274f * 0.5f)) + new Vector3(0f, Mathf.Sin(t * 3.14159274f) * arcHeight, 0f);
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x00075EC4 File Offset: 0x000740C4
		public void OnDrawGizmos()
		{
			for (int i = 0; i < this.feet.Length; i++)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawRay(this.feet[i].transform.position, this.feet[i].transform.TransformVector(this.footRaycastDirection));
			}
		}

		// Token: 0x04001948 RID: 6472
		[Header("Foot Settings")]
		public Transform centerOfGravity;

		// Token: 0x04001949 RID: 6473
		public StriderLegController.FootInfo[] feet;

		// Token: 0x0400194A RID: 6474
		public Vector3 footRaycastDirection;

		// Token: 0x0400194B RID: 6475
		public float raycastVerticalOffset;

		// Token: 0x0400194C RID: 6476
		public float maxRaycastDistance;

		// Token: 0x0400194D RID: 6477
		public float footDampTime;

		// Token: 0x0400194E RID: 6478
		public float stabilityRadius;

		// Token: 0x0400194F RID: 6479
		public float replantDuration;

		// Token: 0x04001950 RID: 6480
		public float replantHeight;

		// Token: 0x04001951 RID: 6481
		public float overstepDistance;

		// Token: 0x04001952 RID: 6482
		public AnimationCurve lerpCurve;

		// Token: 0x04001953 RID: 6483
		public string footPlantString;

		// Token: 0x04001954 RID: 6484
		public string footMoveString;

		// Token: 0x04001955 RID: 6485
		public float footRaycastFrequency = 0.2f;

		// Token: 0x04001956 RID: 6486
		[Header("Root Settings")]
		public Transform rootTransform;

		// Token: 0x04001957 RID: 6487
		public float rootSpringConstant;

		// Token: 0x04001958 RID: 6488
		public float rootDampingConstant;

		// Token: 0x04001959 RID: 6489
		public float rootOffsetHeight;

		// Token: 0x0400195A RID: 6490
		public float rootSmoothDamp;

		// Token: 0x0400195B RID: 6491
		private float rootVelocity;

		// Token: 0x0400195C RID: 6492
		private float footRaycastTimer;

		// Token: 0x020003EF RID: 1007
		[Serializable]
		public struct FootInfo
		{
			// Token: 0x0400195D RID: 6493
			public Transform transform;

			// Token: 0x0400195E RID: 6494
			public Transform referenceTransform;

			// Token: 0x0400195F RID: 6495
			[HideInInspector]
			public Vector3 velocity;

			// Token: 0x04001960 RID: 6496
			[HideInInspector]
			public StriderLegController.FootState footState;

			// Token: 0x04001961 RID: 6497
			[HideInInspector]
			public Vector3 plantPosition;

			// Token: 0x04001962 RID: 6498
			[HideInInspector]
			public Vector3 trailingTargetPosition;

			// Token: 0x04001963 RID: 6499
			[HideInInspector]
			public float stopwatch;
		}

		// Token: 0x020003F0 RID: 1008
		public enum FootState
		{
			// Token: 0x04001965 RID: 6501
			Planted,
			// Token: 0x04001966 RID: 6502
			Replanting
		}
	}
}
