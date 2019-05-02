using System;
using System.Collections;
using System.Collections.Generic;

namespace RoR2.Navigation
{
	// Token: 0x02000530 RID: 1328
	public class NodeGraphSpider
	{
		// Token: 0x06001DDF RID: 7647 RVA: 0x00015D43 File Offset: 0x00013F43
		public NodeGraphSpider(NodeGraph nodeGraph, HullMask hullMask)
		{
			this.nodeGraph = nodeGraph;
			this.hullMask = hullMask;
			this.collectedSteps = new List<NodeGraphSpider.StepInfo>();
			this.uncheckedSteps = new List<NodeGraphSpider.StepInfo>();
			this.visitedNodes = new BitArray(nodeGraph.GetNodeCount());
		}

		// Token: 0x06001DE0 RID: 7648 RVA: 0x00093240 File Offset: 0x00091440
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

		// Token: 0x06001DE1 RID: 7649 RVA: 0x00093330 File Offset: 0x00091530
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

		// Token: 0x0400200E RID: 8206
		private NodeGraph nodeGraph;

		// Token: 0x0400200F RID: 8207
		public List<NodeGraphSpider.StepInfo> collectedSteps;

		// Token: 0x04002010 RID: 8208
		private List<NodeGraphSpider.StepInfo> uncheckedSteps;

		// Token: 0x04002011 RID: 8209
		private BitArray visitedNodes;

		// Token: 0x04002012 RID: 8210
		public HullMask hullMask;

		// Token: 0x02000531 RID: 1329
		public class StepInfo
		{
			// Token: 0x04002013 RID: 8211
			public NodeGraph.NodeIndex node;

			// Token: 0x04002014 RID: 8212
			public NodeGraphSpider.StepInfo previousStep;
		}
	}
}
