using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x0200052E RID: 1326
	[ExecuteInEditMode]
	public class MapNode : MonoBehaviour
	{
		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06001DF4 RID: 7668 RVA: 0x00015F7F File Offset: 0x0001417F
		public static ReadOnlyCollection<MapNode> instances
		{
			get
			{
				return MapNode.instancesReadOnly;
			}
		}

		// Token: 0x06001DF5 RID: 7669 RVA: 0x00015F86 File Offset: 0x00014186
		public void OnEnable()
		{
			MapNode._instances.Add(this);
		}

		// Token: 0x06001DF6 RID: 7670 RVA: 0x00015F93 File Offset: 0x00014193
		public void OnDisable()
		{
			MapNode._instances.Remove(this);
		}

		// Token: 0x06001DF7 RID: 7671 RVA: 0x000918F0 File Offset: 0x0008FAF0
		private void AddLink(MapNode nodeB, float distanceScore, float minJumpHeight, HullClassification hullClassification)
		{
			int num = this.links.FindIndex((MapNode.Link item) => item.nodeB == nodeB);
			if (num == -1)
			{
				this.links.Add(new MapNode.Link
				{
					nodeB = nodeB
				});
				num = this.links.Count - 1;
			}
			MapNode.Link link = this.links[num];
			link.distanceScore = Mathf.Max(link.distanceScore, distanceScore);
			link.minJumpHeight = Mathf.Max(link.minJumpHeight, minJumpHeight);
			link.hullMask |= 1 << (int)hullClassification;
			if (minJumpHeight > 0f)
			{
				link.jumpHullMask |= 1 << (int)hullClassification;
			}
			if (string.IsNullOrEmpty(link.gateName))
			{
				link.gateName = nodeB.gateName;
			}
			this.links[num] = link;
		}

		// Token: 0x06001DF8 RID: 7672 RVA: 0x000919E0 File Offset: 0x0008FBE0
		private void BuildGroundLinks(ReadOnlyCollection<MapNode> nodes, MapNode.MoveProbe moveProbe)
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < nodes.Count; i++)
			{
				MapNode mapNode = nodes[i];
				if (!(mapNode == this))
				{
					Vector3 position2 = mapNode.transform.position;
					float num = MapNode.maxConnectionDistance;
					float num2 = num * num;
					float sqrMagnitude = (position2 - position).sqrMagnitude;
					if (sqrMagnitude < num2)
					{
						float distanceScore = Mathf.Sqrt(sqrMagnitude);
						for (int j = 0; j < 3; j++)
						{
							moveProbe.SetHull((HullClassification)j);
							if ((this.forbiddenHulls & (HullMask)(1 << j)) == HullMask.None && (mapNode.forbiddenHulls & (HullMask)(1 << j)) == HullMask.None)
							{
								Vector3 b = Vector3.up * (moveProbe.testCharacterController.height * 0.5f);
								Vector3 b2 = Vector3.up * 0.01f;
								Vector3 a = moveProbe.GetGroundPosition(position) + b2;
								Vector3 a2 = moveProbe.GetGroundPosition(position2) + b2;
								Vector3 vector = a + b;
								Vector3 vector2 = a2 + b;
								if (moveProbe.CapsuleOverlapTest(vector) && moveProbe.CapsuleOverlapTest(vector2))
								{
									bool flag = moveProbe.GroundTest(vector, vector2, 6f);
									float num3 = (!flag) ? moveProbe.JumpTest(vector, vector2, 7.5f) : 0f;
									if (flag || (num3 > 0f && num3 < 10f))
									{
										this.AddLink(mapNode, distanceScore, num3, (HullClassification)j);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001DF9 RID: 7673 RVA: 0x00091B60 File Offset: 0x0008FD60
		private void BuildAirLinks(ReadOnlyCollection<MapNode> nodes, MapNode.MoveProbe moveProbe)
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < nodes.Count; i++)
			{
				MapNode mapNode = nodes[i];
				if (!(mapNode == this))
				{
					Vector3 position2 = mapNode.transform.position;
					float num = MapNode.maxConnectionDistance * 2f;
					float num2 = num * num;
					float sqrMagnitude = (position2 - position).sqrMagnitude;
					if (sqrMagnitude < num2)
					{
						float distanceScore = Mathf.Sqrt(sqrMagnitude);
						for (int j = 0; j < 3; j++)
						{
							if ((this.forbiddenHulls & (HullMask)(1 << j)) == HullMask.None && (mapNode.forbiddenHulls & (HullMask)(1 << j)) == HullMask.None)
							{
								moveProbe.SetHull((HullClassification)j);
								Vector3 vector = position;
								Vector3 vector2 = position2;
								if (moveProbe.CapsuleOverlapTest(vector) && moveProbe.CapsuleOverlapTest(vector2) && moveProbe.FlyTest(vector, vector2, 6f))
								{
									this.AddLink(mapNode, distanceScore, 0f, (HullClassification)j);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001DFA RID: 7674 RVA: 0x00091C54 File Offset: 0x0008FE54
		private void BuildRailLinks(ReadOnlyCollection<MapNode> nodes, MapNode.MoveProbe moveProbe)
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < nodes.Count; i++)
			{
				MapNode mapNode = nodes[i];
				if (!(mapNode == this))
				{
					Vector3 position2 = mapNode.transform.position;
					float num = MapNode.maxConnectionDistance * 2f;
					float num2 = num * num;
					float sqrMagnitude = (position2 - position).sqrMagnitude;
					if (sqrMagnitude < num2)
					{
						float distanceScore = Mathf.Sqrt(sqrMagnitude);
						for (int j = 0; j < 3; j++)
						{
							HullDef hullDef = HullDef.Find((HullClassification)j);
							if ((this.forbiddenHulls & (HullMask)(1 << j)) == HullMask.None && (mapNode.forbiddenHulls & (HullMask)(1 << j)) == HullMask.None)
							{
								moveProbe.SetHull((HullClassification)j);
								Vector3 vector = position;
								Vector3 vector2 = position2;
								if (Vector3.Angle(Vector3.up, vector2 - vector) > 50f)
								{
									vector.y += hullDef.height;
									vector2.y += hullDef.height;
									if (moveProbe.CapsuleOverlapTest(vector) && moveProbe.CapsuleOverlapTest(vector2) && moveProbe.FlyTest(vector, vector2, 6f))
									{
										this.AddLink(mapNode, distanceScore, 0f, (HullClassification)j);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001DFB RID: 7675 RVA: 0x00091DA0 File Offset: 0x0008FFA0
		public void BuildLinks(ReadOnlyCollection<MapNode> nodes, MapNodeGroup.GraphType graphType)
		{
			this.links.Clear();
			Vector3 position = base.transform.position;
			MapNode.MoveProbe moveProbe = new MapNode.MoveProbe();
			moveProbe.Init();
			switch (graphType)
			{
			case MapNodeGroup.GraphType.Ground:
				this.BuildGroundLinks(nodes, moveProbe);
				break;
			case MapNodeGroup.GraphType.Air:
				this.BuildAirLinks(nodes, moveProbe);
				break;
			case MapNodeGroup.GraphType.Rail:
				this.BuildRailLinks(nodes, moveProbe);
				break;
			}
			foreach (MapNodeLink mapNodeLink in base.GetComponents<MapNodeLink>())
			{
				if (mapNodeLink.other)
				{
					MapNode.Link link = new MapNode.Link
					{
						nodeB = mapNodeLink.other,
						distanceScore = Vector3.Distance(position, mapNodeLink.other.transform.position),
						minJumpHeight = mapNodeLink.minJumpHeight,
						gateName = mapNodeLink.gateName,
						hullMask = -1
					};
					bool flag = false;
					for (int j = 0; j < this.links.Count; j++)
					{
						if (this.links[j].nodeB == mapNodeLink.other)
						{
							this.links[j] = link;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						this.links.Add(link);
					}
				}
			}
			moveProbe.Destroy();
		}

		// Token: 0x06001DFC RID: 7676 RVA: 0x00091EF8 File Offset: 0x000900F8
		public bool TestLineOfSight(MapNode other)
		{
			return !Physics.Linecast(base.transform.position + Vector3.up, other.transform.position + Vector3.up, LayerIndex.world.mask);
		}

		// Token: 0x06001DFD RID: 7677 RVA: 0x00091F4C File Offset: 0x0009014C
		public bool TestNoCeiling()
		{
			return !Physics.Raycast(new Ray(base.transform.position, Vector3.up), float.PositiveInfinity, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
		}

		// Token: 0x06001DFE RID: 7678 RVA: 0x00091F90 File Offset: 0x00090190
		public bool TestTeleporterOK()
		{
			float d = 15f;
			int num = 20;
			float num2 = 7f;
			float num3 = 3f;
			float num4 = 360f / (float)num;
			for (int i = 0; i < num; i++)
			{
				Vector3 b = Quaternion.AngleAxis(num4 * (float)i, Vector3.up) * (Vector3.forward * d);
				Vector3 origin = base.transform.position + b + Vector3.up * num2;
				RaycastHit raycastHit;
				if (!Physics.Raycast(new Ray(origin, Vector3.down), out raycastHit, num3 + num2, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
				{
					return false;
				}
			}
			Debug.DrawRay(base.transform.position, base.transform.up * 20f, Color.green, 15f);
			return true;
		}

		// Token: 0x04002001 RID: 8193
		private static List<MapNode> _instances = new List<MapNode>();

		// Token: 0x04002002 RID: 8194
		private static ReadOnlyCollection<MapNode> instancesReadOnly = MapNode._instances.AsReadOnly();

		// Token: 0x04002003 RID: 8195
		public static readonly float maxConnectionDistance = 15f;

		// Token: 0x04002004 RID: 8196
		public List<MapNode.Link> links = new List<MapNode.Link>();

		// Token: 0x04002005 RID: 8197
		public HullMask forbiddenHulls;

		// Token: 0x04002006 RID: 8198
		[EnumMask(typeof(NodeFlags))]
		public NodeFlags flags;

		// Token: 0x04002007 RID: 8199
		[Tooltip("The name of the nodegraph gate associated with this node. If the named gate is closed this node will be treated as though it does not exist.")]
		public string gateName = "";

		// Token: 0x0200052F RID: 1327
		public struct Link
		{
			// Token: 0x04002008 RID: 8200
			public MapNode nodeB;

			// Token: 0x04002009 RID: 8201
			public float distanceScore;

			// Token: 0x0400200A RID: 8202
			public float minJumpHeight;

			// Token: 0x0400200B RID: 8203
			public int hullMask;

			// Token: 0x0400200C RID: 8204
			public int jumpHullMask;

			// Token: 0x0400200D RID: 8205
			public string gateName;
		}

		// Token: 0x02000530 RID: 1328
		private class MoveProbe
		{
			// Token: 0x06001E01 RID: 7681 RVA: 0x00092070 File Offset: 0x00090270
			public void Init()
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "NodeGraphProbe";
				Transform transform = gameObject.transform;
				this.testCharacterController = gameObject.AddComponent<CharacterController>();
				this.testCharacterController.stepOffset = 0.5f;
				this.testCharacterController.slopeLimit = 60f;
			}

			// Token: 0x06001E02 RID: 7682 RVA: 0x000920C4 File Offset: 0x000902C4
			public void SetHull(HullClassification hullClassification)
			{
				HullDef hullDef = HullDef.Find(hullClassification);
				this.testCharacterController.radius = hullDef.radius;
				this.testCharacterController.height = hullDef.height;
			}

			// Token: 0x06001E03 RID: 7683 RVA: 0x00015FE4 File Offset: 0x000141E4
			public void Destroy()
			{
				UnityEngine.Object.DestroyImmediate(this.testCharacterController.gameObject);
			}

			// Token: 0x06001E04 RID: 7684 RVA: 0x00013098 File Offset: 0x00011298
			private static float DistanceXZ(Vector3 a, Vector3 b)
			{
				a.y = 0f;
				b.y = 0f;
				return Vector3.Distance(a, b);
			}

			// Token: 0x06001E05 RID: 7685 RVA: 0x000920FC File Offset: 0x000902FC
			public Vector3 GetGroundPosition(Vector3 footPosition)
			{
				Vector3 b = Vector3.up * (this.testCharacterController.height * 0.5f - this.testCharacterController.radius);
				Vector3 b2 = Vector3.up * (this.testCharacterController.height * 0.5f);
				Vector3 a = footPosition + b2;
				float num = this.testCharacterController.radius * 0.5f + 0.005f;
				Vector3 vector = footPosition + Vector3.up * num;
				Vector3 a2 = a + Vector3.up * num;
				RaycastHit raycastHit;
				if (Physics.CapsuleCast(a2 + b, a2 - b, this.testCharacterController.radius, Vector3.down, out raycastHit, num * 2f + 0.005f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
				{
					Vector3 b3 = raycastHit.distance * Vector3.down;
					return vector + b3;
				}
				Debug.DrawLine(vector, vector + Vector3.up * 100f, Color.red, 30f);
				return footPosition;
			}

			// Token: 0x06001E06 RID: 7686 RVA: 0x0009221C File Offset: 0x0009041C
			public bool CapsuleOverlapTest(Vector3 centerOfCapsule)
			{
				Vector3 b = Vector3.up * (this.testCharacterController.height * 0.5f - this.testCharacterController.radius);
				Vector3.up * (this.testCharacterController.height * 0.5f);
				return Physics.OverlapCapsule(centerOfCapsule + b, centerOfCapsule - b, this.testCharacterController.radius, LayerIndex.world.mask | LayerIndex.defaultLayer.mask, QueryTriggerInteraction.Ignore).Length == 0;
			}

			// Token: 0x06001E07 RID: 7687 RVA: 0x000922B8 File Offset: 0x000904B8
			public bool FlyTest(Vector3 startPos, Vector3 endPos, float flySpeed)
			{
				Vector3 b = Vector3.up * (this.testCharacterController.height * 0.5f - this.testCharacterController.radius);
				return !Physics.CapsuleCast(startPos + b, startPos - b, this.testCharacterController.radius, (endPos - startPos).normalized, (endPos - startPos).magnitude, LayerIndex.world.mask);
			}

			// Token: 0x06001E08 RID: 7688 RVA: 0x00092340 File Offset: 0x00090540
			public bool GroundTest(Vector3 startCenterOfCapsulePos, Vector3 endCenterOfCapsulePos, float hSpeed)
			{
				this.testCharacterController.Move(Vector3.zero);
				Vector3 a = Vector3.zero;
				float num = MapNode.MoveProbe.DistanceXZ(startCenterOfCapsulePos, endCenterOfCapsulePos);
				this.testCharacterController.transform.position = startCenterOfCapsulePos + Vector3.up;
				int num2 = Mathf.CeilToInt(num * 1.5f / hSpeed / this.testTimeStep);
				Vector3 rhs = this.testCharacterController.transform.position;
				for (int i = 0; i < num2; i++)
				{
					Vector3 vector = endCenterOfCapsulePos - this.testCharacterController.transform.position;
					if (vector.sqrMagnitude <= 0.25f)
					{
						return true;
					}
					Vector3 vector2 = vector;
					vector2.y = 0f;
					vector2.Normalize();
					a.x = vector2.x * hSpeed;
					a.z = vector2.z * hSpeed;
					a += Physics.gravity * this.testTimeStep;
					this.testCharacterController.Move(a * this.testTimeStep);
					Vector3 position = this.testCharacterController.transform.position;
					if (position == rhs)
					{
						return false;
					}
					rhs = position;
				}
				return false;
			}

			// Token: 0x06001E09 RID: 7689 RVA: 0x00092470 File Offset: 0x00090670
			public float JumpTest(Vector3 startCenterOfCapsulePos, Vector3 endCenterOfCapsulePos, float hSpeed)
			{
				float y = Trajectory.CalculateInitialYSpeed(Trajectory.CalculateGroundTravelTime(hSpeed, MapNode.MoveProbe.DistanceXZ(startCenterOfCapsulePos, endCenterOfCapsulePos)), endCenterOfCapsulePos.y - startCenterOfCapsulePos.y);
				this.testCharacterController.Move(Vector3.zero);
				Vector3 a = endCenterOfCapsulePos - startCenterOfCapsulePos;
				a.y = 0f;
				a.Normalize();
				a *= hSpeed;
				a.y = y;
				float num = MapNode.MoveProbe.DistanceXZ(startCenterOfCapsulePos, endCenterOfCapsulePos);
				this.testCharacterController.transform.position = startCenterOfCapsulePos;
				int num2 = Mathf.CeilToInt(num * 1.5f / hSpeed / this.testTimeStep);
				float num3 = float.NegativeInfinity;
				Vector3 rhs = this.testCharacterController.transform.position;
				for (int i = 0; i < num2; i++)
				{
					Vector3 vector = endCenterOfCapsulePos - this.testCharacterController.transform.position;
					if (vector.sqrMagnitude <= 4f)
					{
						return num3 - startCenterOfCapsulePos.y;
					}
					num3 = Mathf.Max(this.testCharacterController.transform.position.y, num3);
					Vector3 vector2 = vector;
					vector2.y = 0f;
					vector2.Normalize();
					a.x = vector2.x * hSpeed;
					a.z = vector2.z * hSpeed;
					a += Physics.gravity * this.testTimeStep;
					this.testCharacterController.Move(a * this.testTimeStep);
					Vector3 position = this.testCharacterController.transform.position;
					if (position == rhs)
					{
						return 0f;
					}
					rhs = position;
				}
				return 0f;
			}

			// Token: 0x0400200E RID: 8206
			public CharacterController testCharacterController;

			// Token: 0x0400200F RID: 8207
			private float testTimeStep = 0.06666667f;
		}
	}
}
