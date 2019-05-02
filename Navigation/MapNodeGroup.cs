using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x02000532 RID: 1330
	public class MapNodeGroup : MonoBehaviour
	{
		// Token: 0x06001E0D RID: 7693 RVA: 0x00092614 File Offset: 0x00090814
		public void Clear()
		{
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				UnityEngine.Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
			}
		}

		// Token: 0x06001E0E RID: 7694 RVA: 0x0001601C File Offset: 0x0001421C
		public void AddNode(Vector3 position)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.position = position;
			gameObject.transform.parent = base.transform;
			gameObject.AddComponent<MapNode>();
			gameObject.name = "MapNode";
		}

		// Token: 0x06001E0F RID: 7695 RVA: 0x00092650 File Offset: 0x00090850
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

		// Token: 0x06001E10 RID: 7696 RVA: 0x000926A0 File Offset: 0x000908A0
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

		// Token: 0x06001E11 RID: 7697 RVA: 0x00092734 File Offset: 0x00090934
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

		// Token: 0x06001E12 RID: 7698 RVA: 0x000927C8 File Offset: 0x000909C8
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

		// Token: 0x04002011 RID: 8209
		public NodeGraph nodeGraph;

		// Token: 0x04002012 RID: 8210
		public Transform testPointA;

		// Token: 0x04002013 RID: 8211
		public Transform testPointB;

		// Token: 0x04002014 RID: 8212
		public HullClassification debugHullDef;

		// Token: 0x04002015 RID: 8213
		public MapNodeGroup.GraphType graphType;

		// Token: 0x02000533 RID: 1331
		public enum GraphType
		{
			// Token: 0x04002017 RID: 8215
			Ground,
			// Token: 0x04002018 RID: 8216
			Air,
			// Token: 0x04002019 RID: 8217
			Rail
		}
	}
}
