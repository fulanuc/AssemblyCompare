using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x02000523 RID: 1315
	public class MapNodeGroup : MonoBehaviour
	{
		// Token: 0x06001DA5 RID: 7589 RVA: 0x00091898 File Offset: 0x0008FA98
		public void Clear()
		{
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				UnityEngine.Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
			}
		}

		// Token: 0x06001DA6 RID: 7590 RVA: 0x00015B53 File Offset: 0x00013D53
		public void AddNode(Vector3 position)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.position = position;
			gameObject.transform.parent = base.transform;
			gameObject.AddComponent<MapNode>();
			gameObject.name = "MapNode";
		}

		// Token: 0x06001DA7 RID: 7591 RVA: 0x000918D4 File Offset: 0x0008FAD4
		public List<MapNode> GetNodes()
		{
			List<MapNode> list = new List<MapNode>();
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				MapNode component = base.transform.GetChild(i).GetComponent<MapNode>();
				if (component)
				{
					list.Add(component);
				}
			}
			return list;
		}

		// Token: 0x06001DA8 RID: 7592 RVA: 0x00091924 File Offset: 0x0008FB24
		public void UpdateNoCeilingMasks()
		{
			int num = 0;
			foreach (MapNode mapNode in this.GetNodes())
			{
				mapNode.flags &= ~NodeFlags.NoCeiling;
				if (mapNode.TestNoCeiling())
				{
					num++;
					mapNode.flags |= NodeFlags.NoCeiling;
				}
			}
			Debug.LogFormat("{0} successful ceiling masks baked.", new object[]
			{
				num
			});
		}

		// Token: 0x06001DA9 RID: 7593 RVA: 0x000919B8 File Offset: 0x0008FBB8
		public void UpdateTeleporterMasks()
		{
			int num = 0;
			foreach (MapNode mapNode in this.GetNodes())
			{
				mapNode.flags &= ~NodeFlags.TeleporterOK;
				if (mapNode.TestTeleporterOK())
				{
					num++;
					mapNode.flags |= NodeFlags.TeleporterOK;
				}
			}
			Debug.LogFormat("{0} successful teleporter masks baked.", new object[]
			{
				num
			});
		}

		// Token: 0x06001DAA RID: 7594 RVA: 0x00091A4C File Offset: 0x0008FC4C
		public void Bake(NodeGraph nodeGraph)
		{
			List<MapNode> nodes = this.GetNodes();
			ReadOnlyCollection<MapNode> readOnlyCollection = nodes.AsReadOnly();
			for (int i = 0; i < nodes.Count; i++)
			{
				nodes[i].BuildLinks(readOnlyCollection, this.graphType);
			}
			List<SerializableBitArray> list = new List<SerializableBitArray>();
			for (int j = 0; j < nodes.Count; j++)
			{
				MapNode mapNode = nodes[j];
				SerializableBitArray serializableBitArray = new SerializableBitArray(nodes.Count);
				for (int k = 0; k < nodes.Count; k++)
				{
					MapNode other = nodes[k];
					serializableBitArray[k] = mapNode.TestLineOfSight(other);
				}
				list.Add(serializableBitArray);
			}
			nodeGraph.SetNodes(readOnlyCollection, list.AsReadOnly());
		}

		// Token: 0x04001FD3 RID: 8147
		public NodeGraph nodeGraph;

		// Token: 0x04001FD4 RID: 8148
		public Transform testPointA;

		// Token: 0x04001FD5 RID: 8149
		public Transform testPointB;

		// Token: 0x04001FD6 RID: 8150
		public HullClassification debugHullDef;

		// Token: 0x04001FD7 RID: 8151
		public MapNodeGroup.GraphType graphType;

		// Token: 0x02000524 RID: 1316
		public enum GraphType
		{
			// Token: 0x04001FD9 RID: 8153
			Ground,
			// Token: 0x04001FDA RID: 8154
			Air,
			// Token: 0x04001FDB RID: 8155
			Rail
		}
	}
}
