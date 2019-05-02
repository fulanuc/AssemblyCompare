using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Navigation;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200037A RID: 890
	public class OccupyNearbyNodes : MonoBehaviour
	{
		// Token: 0x060012BC RID: 4796 RVA: 0x0000E590 File Offset: 0x0000C790
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(base.transform.position, this.radius);
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x0000E5B2 File Offset: 0x0000C7B2
		private void OnEnable()
		{
			OccupyNearbyNodes.instancesList.Add(this);
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x0000E5BF File Offset: 0x0000C7BF
		private void OnDisable()
		{
			OccupyNearbyNodes.instancesList.Remove(this);
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x0000E5CD File Offset: 0x0000C7CD
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			SceneDirector.onPrePopulateSceneServer += OccupyNearbyNodes.OnSceneDirectorPrePopulateSceneServer;
		}

		// Token: 0x060012C0 RID: 4800 RVA: 0x00069E9C File Offset: 0x0006809C
		private static void OnSceneDirectorPrePopulateSceneServer(SceneDirector sceneDirector)
		{
			DirectorCore instance = DirectorCore.instance;
			NodeGraph groundNodeGraph = SceneInfo.instance.GetNodeGraph(MapNodeGroup.GraphType.Ground);
			foreach (NodeGraph.NodeIndex nodeIndex in OccupyNearbyNodes.instancesList.SelectMany((OccupyNearbyNodes v) => groundNodeGraph.FindNodesInRange(v.transform.position, 0f, v.radius, HullMask.None)).Distinct<NodeGraph.NodeIndex>().ToArray<NodeGraph.NodeIndex>())
			{
				instance.AddOccupiedNode(groundNodeGraph, nodeIndex);
			}
		}

		// Token: 0x04001654 RID: 5716
		public float radius = 5f;

		// Token: 0x04001655 RID: 5717
		private static readonly List<OccupyNearbyNodes> instancesList = new List<OccupyNearbyNodes>();
	}
}
