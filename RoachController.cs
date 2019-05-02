using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A9 RID: 937
	public class RoachController : MonoBehaviour
	{
		// Token: 0x060013E5 RID: 5093 RVA: 0x0006F0D8 File Offset: 0x0006D2D8
		private void Awake()
		{
			this.roachTransforms = new Transform[this.roaches.Length];
			for (int i = 0; i < this.roachTransforms.Length; i++)
			{
				this.roachTransforms[i] = UnityEngine.Object.Instantiate<GameObject>(this.roachParams.roachPrefab, this.roaches[i].keyFrames[0].position, this.roaches[i].keyFrames[0].rotation).transform;
			}
		}

		// Token: 0x060013E6 RID: 5094 RVA: 0x0006F160 File Offset: 0x0006D360
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

		// Token: 0x060013E7 RID: 5095 RVA: 0x0006F1A4 File Offset: 0x0006D3A4
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

		// Token: 0x060013E8 RID: 5096 RVA: 0x0000F1DC File Offset: 0x0000D3DC
		public void BakeRoaches()
		{
			this.BakeRoaches2();
		}

		// Token: 0x060013E9 RID: 5097 RVA: 0x0006F364 File Offset: 0x0006D564
		private void ClearRoachPathEditors()
		{
			for (int i = base.transform.childCount - 1; i > 0; i--)
			{
				UnityEngine.Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
			}
		}

		// Token: 0x060013EA RID: 5098 RVA: 0x0006F3A0 File Offset: 0x0006D5A0
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

		// Token: 0x060013EB RID: 5099 RVA: 0x0000F1E4 File Offset: 0x0000D3E4
		public RoachController.RoachPathEditorComponent AddPathEditorObject()
		{
			GameObject gameObject = new GameObject("Roach Path (Temporary)");
			gameObject.hideFlags = HideFlags.DontSave;
			gameObject.transform.SetParent(base.transform, false);
			RoachController.RoachPathEditorComponent roachPathEditorComponent = gameObject.AddComponent<RoachController.RoachPathEditorComponent>();
			roachPathEditorComponent.roachController = this;
			return roachPathEditorComponent;
		}

		// Token: 0x060013EC RID: 5100 RVA: 0x0006F424 File Offset: 0x0006D624
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

		// Token: 0x060013ED RID: 5101 RVA: 0x0000F216 File Offset: 0x0000D416
		private void SetRoachPosition(int i, Vector3 position, Quaternion rotation)
		{
			this.roachTransforms[i].SetPositionAndRotation(position, rotation);
		}

		// Token: 0x060013EE RID: 5102 RVA: 0x0006F4DC File Offset: 0x0006D6DC
		private void Update()
		{
			for (int i = 0; i < this.roaches.Length; i++)
			{
				this.UpdateRoach(i);
			}
		}

		// Token: 0x060013EF RID: 5103 RVA: 0x0000F227 File Offset: 0x0000D427
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

		// Token: 0x060013F0 RID: 5104 RVA: 0x0000F255 File Offset: 0x0000D455
		private void OnTriggerEnter(Collider other)
		{
			CharacterBody component = other.GetComponent<CharacterBody>();
			if (component != null && component.isPlayerControlled)
			{
				this.Scatter();
			}
		}

		// Token: 0x060013F1 RID: 5105 RVA: 0x0006F504 File Offset: 0x0006D704
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
			Gizmos.DrawFrustum(Vector3.zero, this.placementSpreadMax * 0.5f, this.placementMaxDistance, 0f, 1f);
		}

		// Token: 0x0400179E RID: 6046
		public RoachParams roachParams;

		// Token: 0x0400179F RID: 6047
		public int roachCount;

		// Token: 0x040017A0 RID: 6048
		public float placementSpreadMin = 1f;

		// Token: 0x040017A1 RID: 6049
		public float placementSpreadMax = 25f;

		// Token: 0x040017A2 RID: 6050
		public float placementMaxDistance = 10f;

		// Token: 0x040017A3 RID: 6051
		public RoachController.Roach[] roaches;

		// Token: 0x040017A4 RID: 6052
		private Transform[] roachTransforms;

		// Token: 0x040017A5 RID: 6053
		private bool scattered;

		// Token: 0x040017A6 RID: 6054
		private Run.TimeStamp scatterStartTime = Run.TimeStamp.positiveInfinity;

		// Token: 0x040017A7 RID: 6055
		private const string roachScatterSoundString = "Play_env_roach_scatter";

		// Token: 0x020003AA RID: 938
		[Serializable]
		public struct KeyFrame
		{
			// Token: 0x040017A8 RID: 6056
			public float time;

			// Token: 0x040017A9 RID: 6057
			public Vector3 position;

			// Token: 0x040017AA RID: 6058
			public Quaternion rotation;
		}

		// Token: 0x020003AB RID: 939
		[Serializable]
		public struct Roach
		{
			// Token: 0x040017AB RID: 6059
			public RoachController.KeyFrame[] keyFrames;
		}

		// Token: 0x020003AC RID: 940
		private class SimulatedRoach : IDisposable
		{
			// Token: 0x170001BA RID: 442
			// (get) Token: 0x060013F3 RID: 5107 RVA: 0x0000F2A5 File Offset: 0x0000D4A5
			// (set) Token: 0x060013F4 RID: 5108 RVA: 0x0000F2AD File Offset: 0x0000D4AD
			public Transform transform { get; private set; }

			// Token: 0x170001BB RID: 443
			// (get) Token: 0x060013F5 RID: 5109 RVA: 0x0000F2B6 File Offset: 0x0000D4B6
			// (set) Token: 0x060013F6 RID: 5110 RVA: 0x0000F2BE File Offset: 0x0000D4BE
			public float age { get; private set; }

			// Token: 0x170001BC RID: 444
			// (get) Token: 0x060013F7 RID: 5111 RVA: 0x0000F2C7 File Offset: 0x0000D4C7
			// (set) Token: 0x060013F8 RID: 5112 RVA: 0x0000F2CF File Offset: 0x0000D4CF
			public bool finished { get; private set; }

			// Token: 0x170001BD RID: 445
			// (get) Token: 0x060013F9 RID: 5113 RVA: 0x0000F2D8 File Offset: 0x0000D4D8
			private bool onGround
			{
				get
				{
					return this.groundNormal != Vector3.zero;
				}
			}

			// Token: 0x060013FA RID: 5114 RVA: 0x0006F568 File Offset: 0x0006D768
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

			// Token: 0x060013FB RID: 5115 RVA: 0x0006F644 File Offset: 0x0006D844
			private void SetUpVector(Vector3 desiredUp)
			{
				Vector3 right = this.transform.right;
				Vector3 up = this.transform.up;
				this.transform.Rotate(right, Vector3.SignedAngle(up, desiredUp, right), Space.World);
			}

			// Token: 0x060013FC RID: 5116 RVA: 0x0006F680 File Offset: 0x0006D880
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

			// Token: 0x060013FD RID: 5117 RVA: 0x0006F7FC File Offset: 0x0006D9FC
			private void OnBump()
			{
				this.TurnDesiredMovement(UnityEngine.Random.Range(-90f, 90f));
				this.currentSpeed *= -0.5f;
				if (this.roachParams.chanceToFinishOnBump < UnityEngine.Random.value)
				{
					this.finished = true;
				}
			}

			// Token: 0x060013FE RID: 5118 RVA: 0x0006F84C File Offset: 0x0006DA4C
			private void TurnDesiredMovement(float degrees)
			{
				Quaternion rotation = Quaternion.AngleAxis(degrees, this.transform.up);
				this.desiredMovement = rotation * this.desiredMovement;
			}

			// Token: 0x060013FF RID: 5119 RVA: 0x0000F2EA File Offset: 0x0000D4EA
			private void TurnBody(float degrees)
			{
				this.transform.Rotate(Vector3.up, degrees, Space.Self);
			}

			// Token: 0x06001400 RID: 5120 RVA: 0x0006F880 File Offset: 0x0006DA80
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

			// Token: 0x06001401 RID: 5121 RVA: 0x0006F8FC File Offset: 0x0006DAFC
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

			// Token: 0x06001402 RID: 5122 RVA: 0x0006FA6C File Offset: 0x0006DC6C
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

			// Token: 0x06001403 RID: 5123 RVA: 0x0000F2FE File Offset: 0x0000D4FE
			public void Dispose()
			{
				UnityEngine.Object.DestroyImmediate(this.transform.gameObject);
				this.transform = null;
			}

			// Token: 0x040017AC RID: 6060
			private Vector3 initialFleeNormal;

			// Token: 0x040017AD RID: 6061
			private Vector3 desiredMovement;

			// Token: 0x040017AE RID: 6062
			private RoachParams roachParams;

			// Token: 0x040017B2 RID: 6066
			private float reorientTimer;

			// Token: 0x040017B3 RID: 6067
			private float backupTimer;

			// Token: 0x040017B4 RID: 6068
			private Vector3 velocity = Vector3.zero;

			// Token: 0x040017B5 RID: 6069
			private float currentSpeed;

			// Token: 0x040017B6 RID: 6070
			private float desiredSpeed;

			// Token: 0x040017B7 RID: 6071
			private float turnVelocity;

			// Token: 0x040017B8 RID: 6072
			private Vector3 groundNormal;

			// Token: 0x040017B9 RID: 6073
			private float simulationDuration;

			// Token: 0x020003AD RID: 941
			private struct RaycastResult
			{
				// Token: 0x040017BA RID: 6074
				public bool didHit;

				// Token: 0x040017BB RID: 6075
				public Vector3 point;

				// Token: 0x040017BC RID: 6076
				public Vector3 normal;

				// Token: 0x040017BD RID: 6077
				public float distance;
			}
		}

		// Token: 0x020003AE RID: 942
		public class RoachPathEditorComponent : MonoBehaviour
		{
			// Token: 0x170001BE RID: 446
			// (get) Token: 0x06001404 RID: 5124 RVA: 0x0000F317 File Offset: 0x0000D517
			public int nodeCount
			{
				get
				{
					return base.transform.childCount;
				}
			}

			// Token: 0x06001405 RID: 5125 RVA: 0x0000F324 File Offset: 0x0000D524
			public RoachController.RoachNodeEditorComponent AddNode()
			{
				GameObject gameObject = new GameObject("Roach Path Node (Temporary)");
				gameObject.hideFlags = HideFlags.DontSave;
				gameObject.transform.SetParent(base.transform);
				RoachController.RoachNodeEditorComponent roachNodeEditorComponent = gameObject.AddComponent<RoachController.RoachNodeEditorComponent>();
				roachNodeEditorComponent.path = this;
				return roachNodeEditorComponent;
			}

			// Token: 0x06001406 RID: 5126 RVA: 0x0006FAF0 File Offset: 0x0006DCF0
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

			// Token: 0x040017BE RID: 6078
			public RoachController roachController;
		}

		// Token: 0x020003AF RID: 943
		public class RoachNodeEditorComponent : MonoBehaviour
		{
			// Token: 0x06001408 RID: 5128 RVA: 0x0006FB50 File Offset: 0x0006DD50
			public void FacePosition(Vector3 position)
			{
				Vector3 position2 = base.transform.position;
				Vector3 up = base.transform.up;
				Quaternion rotation = Quaternion.LookRotation(position - position2, up);
				base.transform.rotation = rotation;
				base.transform.up = up;
			}

			// Token: 0x040017BF RID: 6079
			public RoachController.RoachPathEditorComponent path;
		}
	}
}
