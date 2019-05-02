using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x02000538 RID: 1336
	[CreateAssetMenu]
	public class NodeGraph : ScriptableObject
	{
		// Token: 0x06001E1B RID: 7707 RVA: 0x00016084 File Offset: 0x00014284
		public void Clear()
		{
			this.nodes = Array.Empty<NodeGraph.Node>();
			this.links = Array.Empty<NodeGraph.Link>();
			this.gateNames = new List<string>
			{
				""
			};
		}

		// Token: 0x06001E1C RID: 7708 RVA: 0x0009295C File Offset: 0x00090B5C
		public void SetNodes(ReadOnlyCollection<MapNode> mapNodes, ReadOnlyCollection<SerializableBitArray> lineOfSightMasks)
		{
			this.Clear();
			Dictionary<MapNode, NodeGraph.NodeIndex> dictionary = new Dictionary<MapNode, NodeGraph.NodeIndex>();
			List<NodeGraph.Node> list = new List<NodeGraph.Node>();
			List<NodeGraph.Link> list2 = new List<NodeGraph.Link>();
			for (int i = 0; i < mapNodes.Count; i++)
			{
				MapNode key = mapNodes[i];
				dictionary[key] = new NodeGraph.NodeIndex(i);
			}
			for (int j = 0; j < mapNodes.Count; j++)
			{
				MapNode mapNode = mapNodes[j];
				NodeGraph.NodeIndex nodeIndexA = dictionary[mapNode];
				int count = list2.Count;
				for (int k = 0; k < mapNode.links.Count; k++)
				{
					MapNode.Link link = mapNode.links[k];
					if (!dictionary.ContainsKey(link.nodeB))
					{
						Debug.LogErrorFormat(link.nodeB, "[{0}] Node {1} was not registered.", new object[]
						{
							k,
							link.nodeB
						});
					}
					list2.Add(new NodeGraph.Link
					{
						nodeIndexA = nodeIndexA,
						nodeIndexB = dictionary[link.nodeB],
						distanceScore = link.distanceScore,
						minJumpHeight = link.minJumpHeight,
						hullMask = link.hullMask,
						jumpHullMask = link.jumpHullMask,
						gateIndex = this.RegisterGateName(link.gateName)
					});
				}
				HullMask hullMask = mapNode.forbiddenHulls;
				for (HullClassification hullClassification = HullClassification.Human; hullClassification < HullClassification.Count; hullClassification++)
				{
					bool flag = false;
					int num = 1 << (int)hullClassification;
					List<MapNode.Link> list3 = mapNode.links;
					for (int l = 0; l < list3.Count; l++)
					{
						if ((list3[l].hullMask & num) != 0)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						hullMask |= (HullMask)num;
					}
				}
				list.Add(new NodeGraph.Node
				{
					position = mapNode.transform.position,
					linkListIndex = new NodeGraph.LinkListIndex
					{
						index = count,
						size = (uint)mapNode.links.Count
					},
					forbiddenHulls = hullMask,
					flags = mapNode.flags,
					lineOfSightMask = new SerializableBitArray(lineOfSightMasks[j]),
					gateIndex = this.RegisterGateName(mapNode.gateName)
				});
			}
			this.nodes = list.ToArray();
			this.links = list2.ToArray();
		}

		// Token: 0x06001E1D RID: 7709 RVA: 0x00092BD4 File Offset: 0x00090DD4
		public Vector3 GetQuadraticCoordinates(float t, Vector3 startPos, Vector3 apexPos, Vector3 endPos)
		{
			return Mathf.Pow(1f - t, 2f) * startPos + 2f * t * (1f - t) * apexPos + Mathf.Pow(t, 2f) * endPos;
		}

		// Token: 0x06001E1E RID: 7710 RVA: 0x00092C2C File Offset: 0x00090E2C
		public Mesh GenerateLinkDebugMesh(HullClassification hull)
		{
			Mesh result;
			using (WireMeshBuilder wireMeshBuilder = new WireMeshBuilder())
			{
				int num = 1 << (int)hull;
				foreach (NodeGraph.Link link in this.links)
				{
					if ((link.hullMask & num) != 0)
					{
						Vector3 position = this.nodes[link.nodeIndexA.nodeIndex].position;
						Vector3 position2 = this.nodes[link.nodeIndexB.nodeIndex].position;
						Vector3 vector = (position + position2) * 0.5f;
						bool flag = (link.jumpHullMask & num) != 0;
						Color color = flag ? Color.cyan : Color.green;
						if (flag)
						{
							Vector3 apexPos = vector;
							apexPos.y = position.y + link.minJumpHeight;
							int num2 = 8;
							Vector3 p = position;
							for (int j = 1; j <= num2; j++)
							{
								if (j > num2 / 2)
								{
									color.a = 0.1f;
								}
								Vector3 quadraticCoordinates = this.GetQuadraticCoordinates((float)j / (float)num2, position, apexPos, position2);
								wireMeshBuilder.AddLine(p, color, quadraticCoordinates, color);
								p = quadraticCoordinates;
							}
						}
						else
						{
							Color c = color;
							c.a = 0.1f;
							wireMeshBuilder.AddLine(position, color, position2, c);
						}
					}
				}
				result = wireMeshBuilder.GenerateMesh();
			}
			return result;
		}

		// Token: 0x06001E1F RID: 7711 RVA: 0x00092DA8 File Offset: 0x00090FA8
		public void DebugDrawLinks(HullClassification hull)
		{
			int num = 1 << (int)hull;
			foreach (NodeGraph.Link link in this.links)
			{
				if ((link.hullMask & num) != 0)
				{
					Vector3 position = this.nodes[link.nodeIndexA.nodeIndex].position;
					Vector3 position2 = this.nodes[link.nodeIndexB.nodeIndex].position;
					Vector3 vector = (position + position2) * 0.5f;
					bool flag = (link.jumpHullMask & num) != 0;
					Color color = flag ? Color.cyan : Color.green;
					if (flag)
					{
						Vector3 apexPos = vector;
						apexPos.y = position.y + link.minJumpHeight;
						int num2 = 8;
						Vector3 start = position;
						for (int j = 1; j <= num2; j++)
						{
							if (j > num2 / 2)
							{
								color.a = 0.1f;
							}
							Vector3 quadraticCoordinates = this.GetQuadraticCoordinates((float)j / (float)num2, position, apexPos, position2);
							Debug.DrawLine(start, quadraticCoordinates, color, 10f);
							start = quadraticCoordinates;
						}
					}
					else
					{
						Debug.DrawLine(position, vector, color, 10f, false);
						Color color2 = color;
						color2.a = 0.1f;
						Debug.DrawLine(vector, position2, color2, 10f, false);
					}
				}
			}
		}

		// Token: 0x06001E20 RID: 7712 RVA: 0x00092EFC File Offset: 0x000910FC
		public void DebugDrawPath(Vector3 startPos, Vector3 endPos)
		{
			Path path = new Path(this);
			this.ComputePath(new NodeGraph.PathRequest
			{
				startPos = startPos,
				endPos = endPos,
				path = path,
				hullClassification = HullClassification.Human
			}).Wait();
			if (path.status == PathStatus.Valid)
			{
				for (int i = 1; i < path.waypointsCount; i++)
				{
					Debug.DrawLine(this.nodes[path[i - 1].nodeIndex.nodeIndex].position, this.nodes[path[i].nodeIndex.nodeIndex].position, Color.red, 10f);
				}
			}
		}

		// Token: 0x06001E21 RID: 7713 RVA: 0x00092FA8 File Offset: 0x000911A8
		public void DebugHighlightNodesWithNoLinks()
		{
			foreach (NodeGraph.Node node in this.nodes)
			{
				if (node.linkListIndex.size <= 0u)
				{
					Debug.DrawRay(node.position, Vector3.up * 100f, Color.cyan, 60f);
				}
			}
		}

		// Token: 0x06001E22 RID: 7714 RVA: 0x000160B2 File Offset: 0x000142B2
		public int GetNodeCount()
		{
			return this.nodes.Length;
		}

		// Token: 0x06001E23 RID: 7715 RVA: 0x00093004 File Offset: 0x00091204
		public List<NodeGraph.NodeIndex> GetActiveNodesForHullMask(HullMask hullMask)
		{
			List<NodeGraph.NodeIndex> list = new List<NodeGraph.NodeIndex>(this.nodes.Length);
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if ((this.nodes[i].forbiddenHulls & hullMask) == HullMask.None && (this.nodes[i].gateIndex == 0 || this.openGates[(int)this.nodes[i].gateIndex]))
				{
					list.Add(new NodeGraph.NodeIndex(i));
				}
			}
			return list;
		}

		// Token: 0x06001E24 RID: 7716 RVA: 0x00093084 File Offset: 0x00091284
		public List<NodeGraph.NodeIndex> GetActiveNodesForHullMaskWithFlagConditions(HullMask hullMask, NodeFlags requiredFlags, NodeFlags forbiddenFlags)
		{
			List<NodeGraph.NodeIndex> list = new List<NodeGraph.NodeIndex>(this.nodes.Length);
			for (int i = 0; i < this.nodes.Length; i++)
			{
				NodeFlags flags = this.nodes[i].flags;
				if ((flags & forbiddenFlags) == NodeFlags.None && (flags & requiredFlags) == requiredFlags && (this.nodes[i].forbiddenHulls & hullMask) == HullMask.None && (this.nodes[i].gateIndex == 0 || this.openGates[(int)this.nodes[i].gateIndex]))
				{
					list.Add(new NodeGraph.NodeIndex(i));
				}
			}
			return list;
		}

		// Token: 0x06001E25 RID: 7717 RVA: 0x00093124 File Offset: 0x00091324
		public List<NodeGraph.NodeIndex> FindNodesInRange(Vector3 position, float minRange, float maxRange, HullMask hullMask)
		{
			float num = minRange * minRange;
			float num2 = maxRange * maxRange;
			List<NodeGraph.NodeIndex> list = new List<NodeGraph.NodeIndex>();
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if ((this.nodes[i].forbiddenHulls & hullMask) == HullMask.None && (this.nodes[i].gateIndex == 0 || this.openGates[(int)this.nodes[i].gateIndex]))
				{
					float sqrMagnitude = (this.nodes[i].position - position).sqrMagnitude;
					if (sqrMagnitude >= num && sqrMagnitude <= num2)
					{
						list.Add(new NodeGraph.NodeIndex(i));
					}
				}
			}
			return list;
		}

		// Token: 0x06001E26 RID: 7718 RVA: 0x000931D4 File Offset: 0x000913D4
		public List<NodeGraph.NodeIndex> FindNodesInRangeWithFlagConditions(Vector3 position, float minRange, float maxRange, HullMask hullMask, NodeFlags requiredFlags, NodeFlags forbiddenFlags, bool preventOverhead)
		{
			float num = minRange * minRange;
			float num2 = maxRange * maxRange;
			List<NodeGraph.NodeIndex> list = new List<NodeGraph.NodeIndex>();
			for (int i = 0; i < this.nodes.Length; i++)
			{
				NodeFlags flags = this.nodes[i].flags;
				if ((flags & forbiddenFlags) == NodeFlags.None && (flags & requiredFlags) == requiredFlags && (this.nodes[i].forbiddenHulls & hullMask) == HullMask.None && (this.nodes[i].gateIndex == 0 || this.openGates[(int)this.nodes[i].gateIndex]))
				{
					Vector3 a = this.nodes[i].position - position;
					float sqrMagnitude = a.sqrMagnitude;
					if (sqrMagnitude >= num && sqrMagnitude <= num2 && (!preventOverhead || Vector3.Dot(a / Mathf.Sqrt(sqrMagnitude), Vector3.up) <= 0.707106769f))
					{
						list.Add(new NodeGraph.NodeIndex(i));
					}
				}
			}
			return list;
		}

		// Token: 0x06001E27 RID: 7719 RVA: 0x000932D8 File Offset: 0x000914D8
		public bool GetNodePosition(NodeGraph.NodeIndex nodeIndex, out Vector3 position)
		{
			if (nodeIndex != NodeGraph.NodeIndex.invalid && nodeIndex.nodeIndex < this.nodes.Length)
			{
				position = this.nodes[nodeIndex.nodeIndex].position;
				return true;
			}
			position = Vector3.zero;
			return false;
		}

		// Token: 0x06001E28 RID: 7720 RVA: 0x000160BC File Offset: 0x000142BC
		public bool GetNodeFlags(NodeGraph.NodeIndex nodeIndex, out NodeFlags flags)
		{
			if (nodeIndex != NodeGraph.NodeIndex.invalid && nodeIndex.nodeIndex < this.nodes.Length)
			{
				flags = this.nodes[nodeIndex.nodeIndex].flags;
				return true;
			}
			flags = NodeFlags.None;
			return false;
		}

		// Token: 0x06001E29 RID: 7721 RVA: 0x0009332C File Offset: 0x0009152C
		public NodeGraph.LinkIndex[] GetActiveNodeLinks(NodeGraph.NodeIndex nodeIndex)
		{
			if (nodeIndex != NodeGraph.NodeIndex.invalid && nodeIndex.nodeIndex < this.nodes.Length)
			{
				NodeGraph.LinkListIndex linkListIndex = this.nodes[nodeIndex.nodeIndex].linkListIndex;
				NodeGraph.LinkIndex[] array = new NodeGraph.LinkIndex[linkListIndex.size];
				int index = linkListIndex.index;
				int num = 0;
				while ((long)num < (long)((ulong)linkListIndex.size))
				{
					array[num] = new NodeGraph.LinkIndex
					{
						linkIndex = index++
					};
					num++;
				}
				return array;
			}
			return null;
		}

		// Token: 0x06001E2A RID: 7722 RVA: 0x000933B4 File Offset: 0x000915B4
		public bool TestNodeLineOfSight(NodeGraph.NodeIndex nodeIndexA, NodeGraph.NodeIndex nodeIndexB)
		{
			return nodeIndexA != NodeGraph.NodeIndex.invalid && nodeIndexA.nodeIndex < this.nodes.Length && nodeIndexB != NodeGraph.NodeIndex.invalid && nodeIndexB.nodeIndex < this.nodes.Length && this.nodes[nodeIndexA.nodeIndex].lineOfSightMask[nodeIndexB.nodeIndex];
		}

		// Token: 0x06001E2B RID: 7723 RVA: 0x00093420 File Offset: 0x00091620
		public bool GetPositionAlongLink(NodeGraph.LinkIndex linkIndex, float t, out Vector3 position)
		{
			if (linkIndex != NodeGraph.LinkIndex.invalid && linkIndex.linkIndex < this.links.Length)
			{
				position = Vector3.LerpUnclamped(this.nodes[this.links[linkIndex.linkIndex].nodeIndexA.nodeIndex].position, this.nodes[this.links[linkIndex.linkIndex].nodeIndexB.nodeIndex].position, t);
				return true;
			}
			position = Vector3.zero;
			return false;
		}

		// Token: 0x06001E2C RID: 7724 RVA: 0x000934BC File Offset: 0x000916BC
		public bool IsLinkSuitableForHull(NodeGraph.LinkIndex linkIndex, HullClassification hullClassification)
		{
			return linkIndex != NodeGraph.LinkIndex.invalid && linkIndex.linkIndex < this.links.Length && (this.links[linkIndex.linkIndex].hullMask & 1 << (int)hullClassification) != 0 && (this.links[linkIndex.linkIndex].gateIndex == 0 || this.openGates[(int)this.links[linkIndex.linkIndex].gateIndex]);
		}

		// Token: 0x06001E2D RID: 7725 RVA: 0x00093540 File Offset: 0x00091740
		public bool IsLinkSuitableForHull(NodeGraph.LinkIndex linkIndex, HullMask hullMask)
		{
			return linkIndex != NodeGraph.LinkIndex.invalid && linkIndex.linkIndex < this.links.Length && (this.links[linkIndex.linkIndex].hullMask & (int)hullMask) != 0 && (this.links[linkIndex.linkIndex].gateIndex == 0 || this.openGates[(int)this.links[linkIndex.linkIndex].gateIndex]);
		}

		// Token: 0x06001E2E RID: 7726 RVA: 0x000160F9 File Offset: 0x000142F9
		public NodeGraph.NodeIndex GetLinkStartNode(NodeGraph.LinkIndex linkIndex)
		{
			if (linkIndex != NodeGraph.LinkIndex.invalid && linkIndex.linkIndex < this.links.Length)
			{
				return this.links[linkIndex.linkIndex].nodeIndexA;
			}
			return NodeGraph.NodeIndex.invalid;
		}

		// Token: 0x06001E2F RID: 7727 RVA: 0x00016134 File Offset: 0x00014334
		public NodeGraph.NodeIndex GetLinkEndNode(NodeGraph.LinkIndex linkIndex)
		{
			if (linkIndex != NodeGraph.LinkIndex.invalid && linkIndex.linkIndex < this.links.Length)
			{
				return this.links[linkIndex.linkIndex].nodeIndexB;
			}
			return NodeGraph.NodeIndex.invalid;
		}

		// Token: 0x06001E30 RID: 7728 RVA: 0x000935C0 File Offset: 0x000917C0
		public NodeGraph.NodeIndex FindClosestNode(Vector3 position, HullClassification hullClassification)
		{
			float num = float.PositiveInfinity;
			NodeGraph.NodeIndex invalid = NodeGraph.NodeIndex.invalid;
			int num2 = 1 << (int)hullClassification;
			for (int i = 0; i < this.nodes.Length; i++)
			{
				NodeGraph.Node node = this.nodes[i];
				if ((node.forbiddenHulls & (HullMask)num2) == HullMask.None && (node.gateIndex == 0 || this.openGates[(int)node.gateIndex]))
				{
					float sqrMagnitude = (node.position - position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						invalid = new NodeGraph.NodeIndex(i);
					}
				}
			}
			return invalid;
		}

		// Token: 0x06001E31 RID: 7729 RVA: 0x00093650 File Offset: 0x00091850
		public NodeGraph.NodeIndex FindClosestNodeWithFlagConditions(Vector3 position, HullClassification hullClassification, NodeFlags requiredFlags, NodeFlags forbiddenFlags, bool preventOverhead)
		{
			float num = float.PositiveInfinity;
			NodeGraph.NodeIndex invalid = NodeGraph.NodeIndex.invalid;
			int num2 = 1 << (int)hullClassification;
			for (int i = 0; i < this.nodes.Length; i++)
			{
				NodeFlags flags = this.nodes[i].flags;
				if ((flags & forbiddenFlags) == NodeFlags.None && (flags & requiredFlags) == requiredFlags && (this.nodes[i].forbiddenHulls & (HullMask)num2) == HullMask.None && (this.nodes[i].gateIndex == 0 || this.openGates[(int)this.nodes[i].gateIndex]))
				{
					Vector3 a = this.nodes[i].position - position;
					float sqrMagnitude = a.sqrMagnitude;
					if (sqrMagnitude < num && (!preventOverhead || Vector3.Dot(a / Mathf.Sqrt(sqrMagnitude), Vector3.up) <= 0.707106769f))
					{
						num = sqrMagnitude;
						invalid = new NodeGraph.NodeIndex(i);
					}
				}
			}
			return invalid;
		}

		// Token: 0x06001E32 RID: 7730 RVA: 0x0001616F File Offset: 0x0001436F
		private float HeuristicCostEstimate(Vector3 startPos, Vector3 endPos)
		{
			return Vector3.Distance(startPos, endPos);
		}

		// Token: 0x06001E33 RID: 7731 RVA: 0x00013098 File Offset: 0x00011298
		private static float DistanceXZ(Vector3 a, Vector3 b)
		{
			a.y = 0f;
			b.y = 0f;
			return Vector3.Distance(a, b);
		}

		// Token: 0x06001E34 RID: 7732 RVA: 0x00093750 File Offset: 0x00091950
		private static void ArrayRemoveNodeIndex(NodeGraph.NodeIndex[] array, NodeGraph.NodeIndex value, int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (array[i] == value)
				{
					array[i] = array[count - 1];
					return;
				}
			}
		}

		// Token: 0x06001E35 RID: 7733 RVA: 0x0009378C File Offset: 0x0009198C
		public PathTask ComputePath(NodeGraph.PathRequest pathRequest)
		{
			PathTask pathTask = new PathTask(pathRequest.path);
			pathTask.status = PathTask.TaskStatus.Running;
			NodeGraph.NodeIndex nodeIndex = this.FindClosestNode(pathRequest.startPos, pathRequest.hullClassification);
			NodeGraph.NodeIndex nodeIndex2 = this.FindClosestNode(pathRequest.endPos, pathRequest.hullClassification);
			if (nodeIndex.nodeIndex == NodeGraph.NodeIndex.invalid.nodeIndex || nodeIndex2.nodeIndex == NodeGraph.NodeIndex.invalid.nodeIndex)
			{
				pathRequest.path.Clear();
				pathTask.status = PathTask.TaskStatus.Complete;
				return pathTask;
			}
			int num = 1 << (int)pathRequest.hullClassification;
			bool[] array = new bool[this.nodes.Length];
			bool[] array2 = new bool[this.nodes.Length];
			array2[nodeIndex.nodeIndex] = true;
			int i = 1;
			NodeGraph.NodeIndex[] array3 = new NodeGraph.NodeIndex[this.nodes.Length];
			array3[0] = nodeIndex;
			NodeGraph.LinkIndex[] array4 = new NodeGraph.LinkIndex[this.nodes.Length];
			for (int j = 0; j < array4.Length; j++)
			{
				array4[j] = NodeGraph.LinkIndex.invalid;
			}
			float[] array5 = new float[this.nodes.Length];
			for (int k = 0; k < array5.Length; k++)
			{
				array5[k] = float.PositiveInfinity;
			}
			array5[nodeIndex.nodeIndex] = 0f;
			float[] array6 = new float[this.nodes.Length];
			for (int l = 0; l < array6.Length; l++)
			{
				array6[l] = float.PositiveInfinity;
			}
			array6[nodeIndex.nodeIndex] = this.HeuristicCostEstimate(pathRequest.startPos, pathRequest.endPos);
			while (i > 0)
			{
				NodeGraph.NodeIndex invalid = NodeGraph.NodeIndex.invalid;
				float num2 = float.PositiveInfinity;
				for (int m = 0; m < i; m++)
				{
					int nodeIndex3 = array3[m].nodeIndex;
					if (array6[nodeIndex3] <= num2)
					{
						num2 = array6[nodeIndex3];
						invalid = new NodeGraph.NodeIndex(nodeIndex3);
					}
				}
				if (invalid.nodeIndex == nodeIndex2.nodeIndex)
				{
					this.ReconstructPath(pathRequest.path, array4, array4[invalid.nodeIndex], pathRequest);
					pathTask.status = PathTask.TaskStatus.Complete;
					return pathTask;
				}
				array2[invalid.nodeIndex] = false;
				NodeGraph.ArrayRemoveNodeIndex(array3, invalid, i);
				i--;
				array[invalid.nodeIndex] = true;
				NodeGraph.LinkListIndex linkListIndex = this.nodes[invalid.nodeIndex].linkListIndex;
				NodeGraph.LinkIndex linkIndex = new NodeGraph.LinkIndex
				{
					linkIndex = linkListIndex.index
				};
				NodeGraph.LinkIndex linkIndex2 = new NodeGraph.LinkIndex
				{
					linkIndex = linkListIndex.index + (int)linkListIndex.size
				};
				while (linkIndex.linkIndex < linkIndex2.linkIndex)
				{
					NodeGraph.Link link = this.links[linkIndex.linkIndex];
					NodeGraph.NodeIndex nodeIndexB = link.nodeIndexB;
					if (!array[nodeIndexB.nodeIndex])
					{
						if ((num & link.jumpHullMask) != 0 && this.links[linkIndex.linkIndex].minJumpHeight > 0f)
						{
							Vector3 position = this.nodes[link.nodeIndexA.nodeIndex].position;
							Vector3 position2 = this.nodes[link.nodeIndexB.nodeIndex].position;
							if (Trajectory.CalculateApex(Trajectory.CalculateInitialYSpeed(Trajectory.CalculateGroundTravelTime(pathRequest.maxSpeed, NodeGraph.DistanceXZ(position, position2)), position2.y - position.y)) > pathRequest.maxJumpHeight)
							{
								goto IL_41A;
							}
						}
						if ((link.hullMask & num) != 0 && (link.gateIndex == 0 || this.openGates[(int)link.gateIndex]))
						{
							float num3 = array5[invalid.nodeIndex] + link.distanceScore;
							if (!array2[nodeIndexB.nodeIndex])
							{
								array2[nodeIndexB.nodeIndex] = true;
								array3[i] = nodeIndexB;
								i++;
							}
							else if (num3 >= array5[nodeIndexB.nodeIndex])
							{
								goto IL_41A;
							}
							array4[nodeIndexB.nodeIndex] = linkIndex;
							array5[nodeIndexB.nodeIndex] = num3;
							array6[nodeIndexB.nodeIndex] = array5[nodeIndexB.nodeIndex] + this.HeuristicCostEstimate(this.nodes[nodeIndexB.nodeIndex].position, this.nodes[nodeIndex2.nodeIndex].position);
						}
					}
					IL_41A:
					linkIndex.linkIndex++;
				}
			}
			pathRequest.path.Clear();
			pathTask.status = PathTask.TaskStatus.Complete;
			return pathTask;
		}

		// Token: 0x06001E36 RID: 7734 RVA: 0x00093BF0 File Offset: 0x00091DF0
		private NodeGraph.LinkIndex Resolve(NodeGraph.LinkIndex[] cameFrom, NodeGraph.LinkIndex current)
		{
			if (current.linkIndex < 0 || current.linkIndex > this.links.Length)
			{
				Debug.LogFormat("Link {0} is out of range [0,{1})", new object[]
				{
					current.linkIndex,
					this.links.Length
				});
			}
			NodeGraph.NodeIndex nodeIndexA = this.links[current.linkIndex].nodeIndexA;
			return cameFrom[nodeIndexA.nodeIndex];
		}

		// Token: 0x06001E37 RID: 7735 RVA: 0x00093C68 File Offset: 0x00091E68
		private void ReconstructPath(Path path, NodeGraph.LinkIndex[] cameFrom, NodeGraph.LinkIndex current, NodeGraph.PathRequest pathRequest)
		{
			int num = 1 << (int)pathRequest.hullClassification;
			path.Clear();
			if (current != NodeGraph.LinkIndex.invalid)
			{
				path.PushWaypointToFront(this.links[current.linkIndex].nodeIndexB, 0f);
			}
			while (current != NodeGraph.LinkIndex.invalid)
			{
				NodeGraph.NodeIndex nodeIndexB = this.links[current.linkIndex].nodeIndexB;
				float minJumpHeight = 0f;
				if ((num & this.links[current.linkIndex].jumpHullMask) != 0 && this.links[current.linkIndex].minJumpHeight > 0f)
				{
					Vector3 position = this.nodes[this.links[current.linkIndex].nodeIndexA.nodeIndex].position;
					Vector3 position2 = this.nodes[this.links[current.linkIndex].nodeIndexB.nodeIndex].position;
					minJumpHeight = Trajectory.CalculateApex(Trajectory.CalculateInitialYSpeed(Trajectory.CalculateGroundTravelTime(pathRequest.maxSpeed, NodeGraph.DistanceXZ(position, position2)), position2.y - position.y));
				}
				path.PushWaypointToFront(nodeIndexB, minJumpHeight);
				if (cameFrom[this.links[current.linkIndex].nodeIndexA.nodeIndex] == NodeGraph.LinkIndex.invalid)
				{
					path.PushWaypointToFront(this.links[current.linkIndex].nodeIndexA, 0f);
				}
				current = cameFrom[this.links[current.linkIndex].nodeIndexA.nodeIndex];
			}
			path.status = PathStatus.Valid;
		}

		// Token: 0x06001E38 RID: 7736 RVA: 0x00093E30 File Offset: 0x00092030
		private byte RegisterGateName(string gateName)
		{
			if (string.IsNullOrEmpty(gateName))
			{
				return 0;
			}
			int num = this.gateNames.IndexOf(gateName);
			if (num == -1)
			{
				num = this.gateNames.Count;
				if (num >= 256)
				{
					Debug.LogErrorFormat(this, "Nodegraph cannot have more than 255 gate names. Nodegraph={0} gateName={1}", new object[]
					{
						this,
						gateName
					});
					num = 0;
				}
				else
				{
					this.gateNames.Add(gateName);
				}
			}
			return (byte)num;
		}

		// Token: 0x06001E39 RID: 7737 RVA: 0x00093E98 File Offset: 0x00092098
		public bool IsGateOpen(string gateName)
		{
			int num = this.gateNames.IndexOf(gateName);
			return num != -1 && this.openGates[num];
		}

		// Token: 0x06001E3A RID: 7738 RVA: 0x00093EC0 File Offset: 0x000920C0
		public void SetGateState(string gateName, bool open)
		{
			int num = this.gateNames.IndexOf(gateName);
			if (num == -1)
			{
				return;
			}
			this.openGates[num] = open;
		}

		// Token: 0x0400202C RID: 8236
		[SerializeField]
		private NodeGraph.Node[] nodes = Array.Empty<NodeGraph.Node>();

		// Token: 0x0400202D RID: 8237
		[SerializeField]
		private NodeGraph.Link[] links = Array.Empty<NodeGraph.Link>();

		// Token: 0x0400202E RID: 8238
		[SerializeField]
		private List<string> gateNames = new List<string>
		{
			""
		};

		// Token: 0x0400202F RID: 8239
		private bool[] openGates = new bool[256];

		// Token: 0x04002030 RID: 8240
		private const float overheadDotLimit = 0.707106769f;

		// Token: 0x02000539 RID: 1337
		[Serializable]
		public struct NodeIndex : IEquatable<NodeGraph.NodeIndex>
		{
			// Token: 0x06001E3C RID: 7740 RVA: 0x00016178 File Offset: 0x00014378
			public NodeIndex(int nodeIndex)
			{
				this.nodeIndex = nodeIndex;
			}

			// Token: 0x06001E3D RID: 7741 RVA: 0x00016181 File Offset: 0x00014381
			public static bool operator ==(NodeGraph.NodeIndex lhs, NodeGraph.NodeIndex rhs)
			{
				return lhs.nodeIndex == rhs.nodeIndex;
			}

			// Token: 0x06001E3E RID: 7742 RVA: 0x00016191 File Offset: 0x00014391
			public static bool operator !=(NodeGraph.NodeIndex lhs, NodeGraph.NodeIndex rhs)
			{
				return lhs.nodeIndex != rhs.nodeIndex;
			}

			// Token: 0x06001E3F RID: 7743 RVA: 0x000161A4 File Offset: 0x000143A4
			public override bool Equals(object other)
			{
				return other is NodeGraph.NodeIndex && ((NodeGraph.NodeIndex)other).nodeIndex == this.nodeIndex;
			}

			// Token: 0x06001E40 RID: 7744 RVA: 0x000161C3 File Offset: 0x000143C3
			public override int GetHashCode()
			{
				return this.nodeIndex;
			}

			// Token: 0x06001E41 RID: 7745 RVA: 0x00016181 File Offset: 0x00014381
			public bool Equals(NodeGraph.NodeIndex other)
			{
				return this.nodeIndex == other.nodeIndex;
			}

			// Token: 0x04002031 RID: 8241
			public int nodeIndex;

			// Token: 0x04002032 RID: 8242
			public static readonly NodeGraph.NodeIndex invalid = new NodeGraph.NodeIndex(-1);
		}

		// Token: 0x0200053A RID: 1338
		[Serializable]
		public struct LinkIndex
		{
			// Token: 0x06001E43 RID: 7747 RVA: 0x000161D8 File Offset: 0x000143D8
			public static bool operator ==(NodeGraph.LinkIndex lhs, NodeGraph.LinkIndex rhs)
			{
				return lhs.linkIndex == rhs.linkIndex;
			}

			// Token: 0x06001E44 RID: 7748 RVA: 0x000161E8 File Offset: 0x000143E8
			public static bool operator !=(NodeGraph.LinkIndex lhs, NodeGraph.LinkIndex rhs)
			{
				return lhs.linkIndex != rhs.linkIndex;
			}

			// Token: 0x06001E45 RID: 7749 RVA: 0x000161FB File Offset: 0x000143FB
			public override bool Equals(object other)
			{
				return other is NodeGraph.LinkIndex && ((NodeGraph.LinkIndex)other).linkIndex == this.linkIndex;
			}

			// Token: 0x06001E46 RID: 7750 RVA: 0x0001621A File Offset: 0x0001441A
			public override int GetHashCode()
			{
				return this.linkIndex;
			}

			// Token: 0x04002033 RID: 8243
			public int linkIndex;

			// Token: 0x04002034 RID: 8244
			public static readonly NodeGraph.LinkIndex invalid = new NodeGraph.LinkIndex
			{
				linkIndex = -1
			};
		}

		// Token: 0x0200053B RID: 1339
		[Serializable]
		public struct LinkListIndex
		{
			// Token: 0x04002035 RID: 8245
			public int index;

			// Token: 0x04002036 RID: 8246
			public uint size;
		}

		// Token: 0x0200053C RID: 1340
		[Serializable]
		public struct Node
		{
			// Token: 0x04002037 RID: 8247
			public Vector3 position;

			// Token: 0x04002038 RID: 8248
			public NodeGraph.LinkListIndex linkListIndex;

			// Token: 0x04002039 RID: 8249
			public HullMask forbiddenHulls;

			// Token: 0x0400203A RID: 8250
			public SerializableBitArray lineOfSightMask;

			// Token: 0x0400203B RID: 8251
			public byte gateIndex;

			// Token: 0x0400203C RID: 8252
			public NodeFlags flags;
		}

		// Token: 0x0200053D RID: 1341
		[Serializable]
		public struct Link
		{
			// Token: 0x0400203D RID: 8253
			public NodeGraph.NodeIndex nodeIndexA;

			// Token: 0x0400203E RID: 8254
			public NodeGraph.NodeIndex nodeIndexB;

			// Token: 0x0400203F RID: 8255
			public float distanceScore;

			// Token: 0x04002040 RID: 8256
			public float maxSlope;

			// Token: 0x04002041 RID: 8257
			public float minJumpHeight;

			// Token: 0x04002042 RID: 8258
			public int hullMask;

			// Token: 0x04002043 RID: 8259
			public int jumpHullMask;

			// Token: 0x04002044 RID: 8260
			public byte gateIndex;
		}

		// Token: 0x0200053E RID: 1342
		public class PathRequest
		{
			// Token: 0x04002045 RID: 8261
			public Path path;

			// Token: 0x04002046 RID: 8262
			public Vector3 startPos;

			// Token: 0x04002047 RID: 8263
			public Vector3 endPos;

			// Token: 0x04002048 RID: 8264
			public HullClassification hullClassification;

			// Token: 0x04002049 RID: 8265
			public float maxSlope;

			// Token: 0x0400204A RID: 8266
			public float maxJumpHeight;

			// Token: 0x0400204B RID: 8267
			public float maxSpeed;
		}
	}
}
