using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003AE RID: 942
	public class RoachController : MonoBehaviour
	{
		// Token: 0x06001402 RID: 5122 RVA: 0x0006F2E0 File Offset: 0x0006D4E0
		private void Awake()
		{
			this.roachTransforms = new Transform[this.roaches.Length];
			for (int i = 0; i < this.roachTransforms.Length; i++)
			{
				this.roachTransforms[i] = UnityEngine.Object.Instantiate<GameObject>(this.roachParams.roachPrefab, this.roaches[i].keyFrames[0].position, this.roaches[i].keyFrames[0].rotation).transform;
			}
		}

		// Token: 0x06001403 RID: 5123 RVA: 0x0006F368 File Offset: 0x0006D568
		private void OnDestroy()
		{
			for (int i = 0; i < this.roachTransforms.Length; i++)
			{
				if (this.roachTransforms[i])
				{
					UnityEngine.Object.Destroy(this.roachTransforms[i].gameObject);
				}
			}
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x0006F3AC File Offset: 0x0006D5AC
		public void BakeRoaches2()
		{
			List<RoachController.Roach> list = new List<RoachController.Roach>();
			for (int i = 0; i < this.roachCount; i++)
			{
				Ray ray = new Ray(base.transform.position, Util.ApplySpread(base.transform.forward, this.placementSpreadMin, this.placementSpreadMax, 1f, 1f, 0f, 0f));
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, this.placementMaxDistance, LayerIndex.world.mask))
				{
					RoachController.SimulatedRoach simulatedRoach = new RoachController.SimulatedRoach(raycastHit.point + raycastHit.normal * 0.01f, raycastHit.normal, ray.direction, this.roachParams);
					float keyframeInterval = this.roachParams.keyframeInterval;
					List<RoachController.KeyFrame> list2 = new List<RoachController.KeyFrame>();
					while (!simulatedRoach.finished)
					{
						simulatedRoach.Simulate(keyframeInterval);
						list2.Add(new RoachController.KeyFrame
						{
							position = simulatedRoach.transform.position,
							rotation = simulatedRoach.transform.rotation,
							time = simulatedRoach.age
						});
					}
					RoachController.KeyFrame keyFrame = list2[list2.Count - 1];
					keyFrame.position += keyFrame.rotation * (Vector3.down * 0.25f);
					list2[list2.Count - 1] = keyFrame;
					simulatedRoach.Dispose();
					list.Add(new RoachController.Roach
					{
						keyFrames = list2.ToArray()
					});
				}
			}
			this.roaches = list.ToArray();
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x0000F380 File Offset: 0x0000D580
		public void BakeRoaches()
		{
			this.BakeRoaches2();
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x0006F56C File Offset: 0x0006D76C
		private void ClearRoachPathEditors()
		{
			for (int i = base.transform.childCount - 1; i > 0; i--)
			{
				UnityEngine.Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
			}
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x0006F5A8 File Offset: 0x0006D7A8
		public void DebakeRoaches()
		{
			this.ClearRoachPathEditors();
			for (int i = 0; i < this.roaches.Length; i++)
			{
				RoachController.Roach roach = this.roaches[i];
				RoachController.RoachPathEditorComponent roachPathEditorComponent = this.AddPathEditorObject();
				for (int j = 0; j < roach.keyFrames.Length; j++)
				{
					RoachController.KeyFrame keyFrame = roach.keyFrames[j];
					RoachController.RoachNodeEditorComponent roachNodeEditorComponent = roachPathEditorComponent.AddNode();
					roachNodeEditorComponent.transform.position = keyFrame.position;
					roachNodeEditorComponent.transform.rotation = keyFrame.rotation;
				}
			}
		}

		// Token: 0x06001408 RID: 5128 RVA: 0x0000F388 File Offset: 0x0000D588
		public RoachController.RoachPathEditorComponent AddPathEditorObject()
		{
			GameObject gameObject = new GameObject("Roach Path (Temporary)");
			gameObject.hideFlags = HideFlags.DontSave;
			gameObject.transform.SetParent(base.transform, false);
			RoachController.RoachPathEditorComponent roachPathEditorComponent = gameObject.AddComponent<RoachController.RoachPathEditorComponent>();
			roachPathEditorComponent.roachController = this;
			return roachPathEditorComponent;
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x0006F62C File Offset: 0x0006D82C
		private void UpdateRoach(int i)
		{
			RoachController.KeyFrame[] keyFrames = this.roaches[i].keyFrames;
			float num = Mathf.Min(this.scatterStartTime.timeSince, keyFrames[keyFrames.Length - 1].time);
			for (int j = 1; j < keyFrames.Length; j++)
			{
				if (num <= keyFrames[j].time)
				{
					RoachController.KeyFrame keyFrame = keyFrames[j - 1];
					RoachController.KeyFrame keyFrame2 = keyFrames[j];
					float t = Mathf.InverseLerp(keyFrame.time, keyFrame2.time, num);
					this.SetRoachPosition(i, Vector3.Lerp(keyFrame.position, keyFrame2.position, t), Quaternion.Slerp(keyFrame.rotation, keyFrame2.rotation, t));
					return;
				}
			}
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x0000F3BA File Offset: 0x0000D5BA
		private void SetRoachPosition(int i, Vector3 position, Quaternion rotation)
		{
			this.roachTransforms[i].SetPositionAndRotation(position, rotation);
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x0006F6E4 File Offset: 0x0006D8E4
		private void Update()
		{
			for (int i = 0; i < this.roaches.Length; i++)
			{
				this.UpdateRoach(i);
			}
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x0000F3CB File Offset: 0x0000D5CB
		private void Scatter()
		{
			if (this.scattered)
			{
				return;
			}
			Util.PlaySound("Play_env_roach_scatter", base.gameObject);
			this.scattered = true;
			this.scatterStartTime = Run.TimeStamp.now;
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x0000F3F9 File Offset: 0x0000D5F9
		private void OnTriggerEnter(Collider other)
		{
			CharacterBody component = other.GetComponent<CharacterBody>();
			if (component != null && component.isPlayerControlled)
			{
				this.Scatter();
			}
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x0006F70C File Offset: 0x0006D90C
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
			Gizmos.DrawFrustum(Vector3.zero, this.placementSpreadMax * 0.5f, this.placementMaxDistance, 0f, 1f);
		}

		// Token: 0x040017B8 RID: 6072
		public RoachParams roachParams;

		// Token: 0x040017B9 RID: 6073
		public int roachCount;

		// Token: 0x040017BA RID: 6074
		public float placementSpreadMin = 1f;

		// Token: 0x040017BB RID: 6075
		public float placementSpreadMax = 25f;

		// Token: 0x040017BC RID: 6076
		public float placementMaxDistance = 10f;

		// Token: 0x040017BD RID: 6077
		public RoachController.Roach[] roaches;

		// Token: 0x040017BE RID: 6078
		private Transform[] roachTransforms;

		// Token: 0x040017BF RID: 6079
		private bool scattered;

		// Token: 0x040017C0 RID: 6080
		private Run.TimeStamp scatterStartTime = Run.TimeStamp.positiveInfinity;

		// Token: 0x040017C1 RID: 6081
		private const string roachScatterSoundString = "Play_env_roach_scatter";

		// Token: 0x020003AF RID: 943
		[Serializable]
		public struct KeyFrame
		{
			// Token: 0x040017C2 RID: 6082
			public float time;

			// Token: 0x040017C3 RID: 6083
			public Vector3 position;

			// Token: 0x040017C4 RID: 6084
			public Quaternion rotation;
		}

		// Token: 0x020003B0 RID: 944
		[Serializable]
		public struct Roach
		{
			// Token: 0x040017C5 RID: 6085
			public RoachController.KeyFrame[] keyFrames;
		}

		// Token: 0x020003B1 RID: 945
		private class SimulatedRoach : IDisposable
		{
			// Token: 0x170001BF RID: 447
			// (get) Token: 0x06001410 RID: 5136 RVA: 0x0000F449 File Offset: 0x0000D649
			// (set) Token: 0x06001411 RID: 5137 RVA: 0x0000F451 File Offset: 0x0000D651
			public Transform transform { get; private set; }

			// Token: 0x170001C0 RID: 448
			// (get) Token: 0x06001412 RID: 5138 RVA: 0x0000F45A File Offset: 0x0000D65A
			// (set) Token: 0x06001413 RID: 5139 RVA: 0x0000F462 File Offset: 0x0000D662
			public float age { get; private set; }

			// Token: 0x170001C1 RID: 449
			// (get) Token: 0x06001414 RID: 5140 RVA: 0x0000F46B File Offset: 0x0000D66B
			// (set) Token: 0x06001415 RID: 5141 RVA: 0x0000F473 File Offset: 0x0000D673
			public bool finished { get; private set; }

			// Token: 0x170001C2 RID: 450
			// (get) Token: 0x06001416 RID: 5142 RVA: 0x0000F47C File Offset: 0x0000D67C
			private bool onGround
			{
				get
				{
					return this.groundNormal != Vector3.zero;
				}
			}

			// Token: 0x06001417 RID: 5143 RVA: 0x0006F770 File Offset: 0x0006D970
			public SimulatedRoach(Vector3 position, Vector3 groundNormal, Vector3 initialFleeNormal, RoachParams roachParams)
			{
				this.roachParams = roachParams;
				GameObject gameObject = new GameObject("SimulatedRoach");
				this.transform = gameObject.transform;
				this.transform.position = position;
				this.transform.up = groundNormal;
				this.transform.Rotate(this.transform.up, UnityEngine.Random.Range(0f, 360f));
				this.transform.forward = UnityEngine.Random.onUnitSphere;
				this.groundNormal = groundNormal;
				this.initialFleeNormal = initialFleeNormal;
				this.desiredMovement = UnityEngine.Random.onUnitSphere;
				this.age = UnityEngine.Random.Range(roachParams.minReactionTime, roachParams.maxReactionTime);
				this.simulationDuration = this.age + UnityEngine.Random.Range(roachParams.minSimulationDuration, roachParams.maxSimulationDuration);
			}

			// Token: 0x06001418 RID: 5144 RVA: 0x0006F84C File Offset: 0x0006DA4C
			private void SetUpVector(Vector3 desiredUp)
			{
				Vector3 right = this.transform.right;
				Vector3 up = this.transform.up;
				this.transform.Rotate(right, Vector3.SignedAngle(up, desiredUp, right), Space.World);
			}

			// Token: 0x06001419 RID: 5145 RVA: 0x0006F888 File Offset: 0x0006DA88
			public void Simulate(float deltaTime)
			{
				this.age += deltaTime;
				if (this.onGround)
				{
					this.SetUpVector(this.groundNormal);
					this.turnVelocity += UnityEngine.Random.Range(-this.roachParams.wiggle, this.roachParams.wiggle) * deltaTime;
					this.TurnDesiredMovement(this.turnVelocity * deltaTime);
					Vector3 up = this.transform.up;
					Vector3 normalized = Vector3.ProjectOnPlane(this.desiredMovement, up).normalized;
					float value = Vector3.SignedAngle(this.transform.forward, normalized, up);
					this.TurnBody(Mathf.Clamp(value, -this.turnVelocity * deltaTime, this.turnVelocity * deltaTime));
					this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, this.roachParams.maxSpeed, deltaTime * this.roachParams.acceleration);
					this.StepGround(this.currentSpeed * deltaTime);
				}
				else
				{
					this.velocity += Physics.gravity * deltaTime;
					this.StepAir(this.velocity);
				}
				this.reorientTimer -= deltaTime;
				if (this.reorientTimer <= 0f)
				{
					this.desiredMovement = this.initialFleeNormal;
					this.reorientTimer = UnityEngine.Random.Range(this.roachParams.reorientTimerMin, this.roachParams.reorientTimerMax);
				}
				if (this.age >= this.simulationDuration)
				{
					this.finished = true;
				}
			}

			// Token: 0x0600141A RID: 5146 RVA: 0x0006FA04 File Offset: 0x0006DC04
			private void OnBump()
			{
				this.TurnDesiredMovement(UnityEngine.Random.Range(-90f, 90f));
				this.currentSpeed *= -0.5f;
				if (this.roachParams.chanceToFinishOnBump < UnityEngine.Random.value)
				{
					this.finished = true;
				}
			}

			// Token: 0x0600141B RID: 5147 RVA: 0x0006FA54 File Offset: 0x0006DC54
			private void TurnDesiredMovement(float degrees)
			{
				Quaternion rotation = Quaternion.AngleAxis(degrees, this.transform.up);
				this.desiredMovement = rotation * this.desiredMovement;
			}

			// Token: 0x0600141C RID: 5148 RVA: 0x0000F48E File Offset: 0x0000D68E
			private void TurnBody(float degrees)
			{
				this.transform.Rotate(Vector3.up, degrees, Space.Self);
			}

			// Token: 0x0600141D RID: 5149 RVA: 0x0006FA88 File Offset: 0x0006DC88
			private void StepAir(Vector3 movement)
			{
				RoachController.SimulatedRoach.RaycastResult raycastResult = RoachController.SimulatedRoach.SimpleRaycast(new Ray(this.transform.position, movement), movement.magnitude);
				Debug.DrawLine(this.transform.position, raycastResult.point, Color.magenta, 10f, false);
				if (raycastResult.didHit)
				{
					this.groundNormal = raycastResult.normal;
					this.velocity = Vector3.zero;
				}
				this.transform.position = raycastResult.point;
			}

			// Token: 0x0600141E RID: 5150 RVA: 0x0006FB04 File Offset: 0x0006DD04
			private void StepGround(float distance)
			{
				this.groundNormal = Vector3.zero;
				Vector3 up = this.transform.up;
				Vector3 forward = this.transform.forward;
				float stepSize = this.roachParams.stepSize;
				Vector3 vector = up * stepSize;
				Vector3 vector2 = this.transform.position;
				vector2 += vector;
				Debug.DrawLine(this.transform.position, vector2, Color.red, 10f, false);
				RoachController.SimulatedRoach.RaycastResult raycastResult = RoachController.SimulatedRoach.SimpleRaycast(new Ray(vector2, forward), distance);
				Debug.DrawLine(vector2, raycastResult.point, Color.green, 10f, false);
				vector2 = raycastResult.point;
				if (raycastResult.didHit)
				{
					if (Vector3.Dot(raycastResult.normal, forward) < -0.5f)
					{
						this.OnBump();
					}
					this.groundNormal = raycastResult.normal;
				}
				else
				{
					RoachController.SimulatedRoach.RaycastResult raycastResult2 = RoachController.SimulatedRoach.SimpleRaycast(new Ray(vector2, -vector), stepSize * 2f);
					if (raycastResult2.didHit)
					{
						Debug.DrawLine(vector2, raycastResult2.point, Color.blue, 10f, false);
						vector2 = raycastResult2.point;
						this.groundNormal = raycastResult2.normal;
					}
					else
					{
						Debug.DrawLine(vector2, vector2 - vector, Color.white, 10f);
						vector2 -= vector;
					}
				}
				if (this.groundNormal == Vector3.zero)
				{
					this.currentSpeed = 0f;
				}
				this.transform.position = vector2;
			}

			// Token: 0x0600141F RID: 5151 RVA: 0x0006FC74 File Offset: 0x0006DE74
			private static RoachController.SimulatedRoach.RaycastResult SimpleRaycast(Ray ray, float maxDistance)
			{
				RaycastHit raycastHit;
				bool flag = Physics.Raycast(ray, out raycastHit, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
				return new RoachController.SimulatedRoach.RaycastResult
				{
					didHit = flag,
					point = (flag ? raycastHit.point : ray.GetPoint(maxDistance)),
					normal = (flag ? raycastHit.normal : Vector3.zero),
					distance = (flag ? raycastHit.distance : maxDistance)
				};
			}

			// Token: 0x06001420 RID: 5152 RVA: 0x0000F4A2 File Offset: 0x0000D6A2
			public void Dispose()
			{
				UnityEngine.Object.DestroyImmediate(this.transform.gameObject);
				this.transform = null;
			}

			// Token: 0x040017C6 RID: 6086
			private Vector3 initialFleeNormal;

			// Token: 0x040017C7 RID: 6087
			private Vector3 desiredMovement;

			// Token: 0x040017C8 RID: 6088
			private RoachParams roachParams;

			// Token: 0x040017CC RID: 6092
			private float reorientTimer;

			// Token: 0x040017CD RID: 6093
			private float backupTimer;

			// Token: 0x040017CE RID: 6094
			private Vector3 velocity = Vector3.zero;

			// Token: 0x040017CF RID: 6095
			private float currentSpeed;

			// Token: 0x040017D0 RID: 6096
			private float desiredSpeed;

			// Token: 0x040017D1 RID: 6097
			private float turnVelocity;

			// Token: 0x040017D2 RID: 6098
			private Vector3 groundNormal;

			// Token: 0x040017D3 RID: 6099
			private float simulationDuration;

			// Token: 0x020003B2 RID: 946
			private struct RaycastResult
			{
				// Token: 0x040017D4 RID: 6100
				public bool didHit;

				// Token: 0x040017D5 RID: 6101
				public Vector3 point;

				// Token: 0x040017D6 RID: 6102
				public Vector3 normal;

				// Token: 0x040017D7 RID: 6103
				public float distance;
			}
		}

		// Token: 0x020003B3 RID: 947
		public class RoachPathEditorComponent : MonoBehaviour
		{
			// Token: 0x170001C3 RID: 451
			// (get) Token: 0x06001421 RID: 5153 RVA: 0x0000F4BB File Offset: 0x0000D6BB
			public int nodeCount
			{
				get
				{
					return base.transform.childCount;
				}
			}

			// Token: 0x06001422 RID: 5154 RVA: 0x0000F4C8 File Offset: 0x0000D6C8
			public RoachController.RoachNodeEditorComponent AddNode()
			{
				GameObject gameObject = new GameObject("Roach Path Node (Temporary)");
				gameObject.hideFlags = HideFlags.DontSave;
				gameObject.transform.SetParent(base.transform);
				RoachController.RoachNodeEditorComponent roachNodeEditorComponent = gameObject.AddComponent<RoachController.RoachNodeEditorComponent>();
				roachNodeEditorComponent.path = this;
				return roachNodeEditorComponent;
			}

			// Token: 0x06001423 RID: 5155 RVA: 0x0006FCF8 File Offset: 0x0006DEF8
			private void OnDrawGizmosSelected()
			{
				Gizmos.color = Color.white;
				int num = 0;
				while (num + 1 < this.nodeCount)
				{
					Vector3 position = base.transform.GetChild(num).transform.position;
					Vector3 position2 = base.transform.GetChild(num + 1).transform.position;
					Gizmos.DrawLine(position, position2);
					num++;
				}
			}

			// Token: 0x040017D8 RID: 6104
			public RoachController roachController;
		}

		// Token: 0x020003B4 RID: 948
		public class RoachNodeEditorComponent : MonoBehaviour
		{
			// Token: 0x06001425 RID: 5157 RVA: 0x0006FD58 File Offset: 0x0006DF58
			public void FacePosition(Vector3 position)
			{
				Vector3 position2 = base.transform.position;
				Vector3 up = base.transform.up;
				Quaternion rotation = Quaternion.LookRotation(position - position2, up);
				base.transform.rotation = rotation;
				base.transform.up = up;
			}

			// Token: 0x040017D9 RID: 6105
			public RoachController.RoachPathEditorComponent path;
		}
	}
}
