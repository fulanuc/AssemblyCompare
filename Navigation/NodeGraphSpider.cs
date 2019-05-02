using System;
using System.Collections;
using System.Collections.Generic;

namespace RoR2.Navigation
{
	// Token: 0x0200053F RID: 1343
	public class NodeGraphSpider
	{
		// Token: 0x06001E49 RID: 7753 RVA: 0x00016222 File Offset: 0x00014422
		public NodeGraphSpider(NodeGraph nodeGraph, HullMask hullMask)
		{
			this.nodeGraph = nodeGraph;
			this.hullMask = hullMask;
			this.collectedSteps = new List<NodeGraphSpider.StepInfo>();
			this.uncheckedSteps = new List<NodeGraphSpider.StepInfo>();
			this.visitedNodes = new BitArray(nodeGraph.GetNodeCount());
		}

		// Token: 0x06001E4A RID: 7754 RVA: 0x00093F5C File Offset: 0x0009215C
		public bool PerformStep()
		{
			List<NodeGraphSpider.StepInfo> list = this.uncheckedSteps;
			this.uncheckedSteps = new List<NodeGraphSpider.StepInfo>();
			for (int i = 0; i < list.Count; i++)
			{
				NodeGraphSpider.StepInfo stepInfo = list[i];
				foreach (NodeGraph.LinkIndex linkIndex in this.nodeGraph.GetActiveNodeLinks(stepInfo.node))
				{
					if (this.nodeGraph.IsLinkSuitableForHull(linkIndex, this.hullMask))
					{
						NodeGraph.NodeIndex linkEndNode = this.nodeGraph.GetLinkEndNode(linkIndex);
						if (!this.visitedNodes[linkEndNode.nodeIndex])
						{
							this.uncheckedSteps.Add(new NodeGraphSpider.StepInfo
							{
								node = linkEndNode,
								previousStep = stepInfo
							});
							this.visitedNodes[linkEndNode.nodeIndex] = true;
						}
					}
				}
				this.collectedSteps.Add(stepInfo);
			}
			return list.Count > 0;
		}

		// Token: 0x06001E4B RID: 7755 RVA: 0x0009404C File Offset: 0x0009224C
		public void AddNodeForNextStep(NodeGraph.NodeIndex nodeIndex)
		{
			if (!this.visitedNodes[nodeIndex.nodeIndex])
			{
				this.uncheckedSteps.Add(new NodeGraphSpider.StepInfo
				{
					node = nodeIndex,
					previousStep = null
				});
				this.visitedNodes[nodeIndex.nodeIndex] = true;
			}
		}

		// Token: 0x0400204C RID: 8268
		private NodeGraph nodeGraph;

		// Token: 0x0400204D RID: 8269
		public List<NodeGraphSpider.StepInfo> collectedSteps;

		// Token: 0x0400204E RID: 8270
		private List<NodeGraphSpider.StepInfo> uncheckedSteps;

		// Token: 0x0400204F RID: 8271
		private BitArray visitedNodes;

		// Token: 0x04002050 RID: 8272
		public HullMask hullMask;

		// Token: 0x02000540 RID: 1344
		public class StepInfo
		{
			// Token: 0x04002051 RID: 8273
			public NodeGraph.NodeIndex node;

			// Token: 0x04002052 RID: 8274
			public NodeGraphSpider.StepInfo previousStep;
		}
	}
}
