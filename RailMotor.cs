using System;
using RoR2.Navigation;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A8 RID: 936
	[RequireComponent(typeof(CharacterBody))]
	public class RailMotor : MonoBehaviour
	{
		// Token: 0x060013E6 RID: 5094 RVA: 0x0006E598 File Offset: 0x0006C798
		private void Start()
		{
			this.characterDirection = base.GetComponent<CharacterDirection>();
			this.inputBank = base.GetComponent<InputBankTest>();
			this.characterBody = base.GetComponent<CharacterBody>();
			this.railGraph = SceneInfo.instance.railNodes;
			ModelLocator component = base.GetComponent<ModelLocator>();
			if (component)
			{
				this.modelAnimator = component.modelTransform.GetComponent<Animator>();
			}
			this.nodeA = this.railGraph.FindClosestNode(base.transform.position, this.characterBody.hullClassification);
			NodeGraph.LinkIndex[] activeNodeLinks = this.railGraph.GetActiveNodeLinks(this.nodeA);
			this.currentLink = activeNodeLinks[0];
			this.UpdateNodeAndLinkInfo();
			this.useRootMotion = this.characterBody.rootMotionInMainState;
		}

		// Token: 0x060013E7 RID: 5095 RVA: 0x0006E658 File Offset: 0x0006C858
		private void UpdateNodeAndLinkInfo()
		{
			this.nodeA = this.railGraph.GetLinkStartNode(this.currentLink);
			this.nodeB = this.railGraph.GetLinkEndNode(this.currentLink);
			this.railGraph.GetNodePosition(this.nodeA, out this.nodeAPosition);
			this.railGraph.GetNodePosition(this.nodeB, out this.nodeBPosition);
			this.linkVector = this.nodeBPosition - this.nodeAPosition;
			this.linkLength = this.linkVector.magnitude;
		}

		// Token: 0x060013E8 RID: 5096 RVA: 0x0006E6EC File Offset: 0x0006C8EC
		private void FixedUpdate()
		{
			this.UpdateNodeAndLinkInfo();
			if (this.inputBank)
			{
				bool value = false;
				if (this.inputMoveVector.sqrMagnitude > 0f)
				{
					value = true;
					this.characterDirection.moveVector = this.linkVector;
					if (this.linkLerp == 0f || this.linkLerp == 1f)
					{
						NodeGraph.NodeIndex nodeIndex;
						if (this.linkLerp == 0f)
						{
							nodeIndex = this.nodeA;
						}
						else
						{
							nodeIndex = this.nodeB;
						}
						NodeGraph.LinkIndex[] activeNodeLinks = this.railGraph.GetActiveNodeLinks(nodeIndex);
						float num = -1f;
						NodeGraph.LinkIndex lhs = this.currentLink;
						Debug.DrawRay(base.transform.position, this.inputMoveVector, Color.green);
						foreach (NodeGraph.LinkIndex linkIndex in activeNodeLinks)
						{
							NodeGraph.NodeIndex linkStartNode = this.railGraph.GetLinkStartNode(linkIndex);
							NodeGraph.NodeIndex linkEndNode = this.railGraph.GetLinkEndNode(linkIndex);
							if (!(linkStartNode != nodeIndex))
							{
								Vector3 vector;
								this.railGraph.GetNodePosition(linkStartNode, out vector);
								Vector3 a;
								this.railGraph.GetNodePosition(linkEndNode, out a);
								Vector3 vector2 = a - vector;
								Vector3 rhs = new Vector3(vector2.x, 0f, vector2.z);
								Debug.DrawRay(vector, vector2, Color.red);
								float num2 = Vector3.Dot(this.inputMoveVector, rhs);
								if (num2 > num)
								{
									num = num2;
									lhs = linkIndex;
								}
							}
						}
						if (lhs != this.currentLink)
						{
							this.currentLink = lhs;
							this.UpdateNodeAndLinkInfo();
							this.linkLerp = 0f;
						}
					}
				}
				this.modelAnimator.SetBool("isMoving", value);
				if (this.useRootMotion)
				{
					this.TravelLink();
				}
				else
				{
					this.TravelLink();
				}
			}
			base.transform.position = Vector3.Lerp(this.nodeAPosition, this.nodeBPosition, this.linkLerp);
		}

		// Token: 0x060013E9 RID: 5097 RVA: 0x0006E8D4 File Offset: 0x0006CAD4
		private void TravelLink()
		{
			this.projectedMoveVector = Vector3.Project(this.inputMoveVector, this.linkVector);
			this.projectedMoveVector = this.projectedMoveVector.normalized;
			if (this.characterBody.rootMotionInMainState)
			{
				this.currentMoveSpeed = this.rootMotion.magnitude / Time.fixedDeltaTime;
				this.rootMotion = Vector3.zero;
			}
			else
			{
				float target;
				if (this.projectedMoveVector.sqrMagnitude > 0f)
				{
					target = this.characterBody.moveSpeed * this.inputMoveVector.magnitude;
				}
				else
				{
					target = 0f;
				}
				this.currentMoveSpeed = Mathf.MoveTowards(this.currentMoveSpeed, target, this.characterBody.acceleration * Time.fixedDeltaTime);
			}
			if (this.currentMoveSpeed > 0f)
			{
				Vector3 lhs = this.projectedMoveVector * this.currentMoveSpeed;
				float num = this.currentMoveSpeed / this.linkLength * Mathf.Sign(Vector3.Dot(lhs, this.linkVector)) * Time.fixedDeltaTime;
				this.linkLerp = Mathf.Clamp01(this.linkLerp + num);
			}
		}

		// Token: 0x060013EA RID: 5098 RVA: 0x0006E9E8 File Offset: 0x0006CBE8
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(this.nodeAPosition, 0.5f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(this.nodeBPosition, 0.5f);
			Gizmos.DrawLine(this.nodeAPosition, this.nodeBPosition);
		}

		// Token: 0x04001773 RID: 6003
		public Vector3 inputMoveVector;

		// Token: 0x04001774 RID: 6004
		public Vector3 rootMotion;

		// Token: 0x04001775 RID: 6005
		private Animator modelAnimator;

		// Token: 0x04001776 RID: 6006
		private InputBankTest inputBank;

		// Token: 0x04001777 RID: 6007
		private NodeGraph railGraph;

		// Token: 0x04001778 RID: 6008
		private NodeGraph.NodeIndex nodeA;

		// Token: 0x04001779 RID: 6009
		private NodeGraph.NodeIndex nodeB;

		// Token: 0x0400177A RID: 6010
		private NodeGraph.LinkIndex currentLink;

		// Token: 0x0400177B RID: 6011
		private CharacterBody characterBody;

		// Token: 0x0400177C RID: 6012
		private CharacterDirection characterDirection;

		// Token: 0x0400177D RID: 6013
		private float linkLerp;

		// Token: 0x0400177E RID: 6014
		private Vector3 projectedMoveVector;

		// Token: 0x0400177F RID: 6015
		private Vector3 nodeAPosition;

		// Token: 0x04001780 RID: 6016
		private Vector3 nodeBPosition;

		// Token: 0x04001781 RID: 6017
		private Vector3 linkVector;

		// Token: 0x04001782 RID: 6018
		private float linkLength;

		// Token: 0x04001783 RID: 6019
		private float currentMoveSpeed;

		// Token: 0x04001784 RID: 6020
		private bool useRootMotion;
	}
}
