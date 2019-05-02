using System;
using System.Collections.Generic;
using RoR2.Navigation;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000466 RID: 1126
	public class PathFollower
	{
		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06001927 RID: 6439 RVA: 0x00003696 File Offset: 0x00001896
		public bool isOnJumpLink
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06001928 RID: 6440 RVA: 0x00082C5C File Offset: 0x00080E5C
		public bool nextWaypointNeedsJump
		{
			get
			{
				return this.waypoints.Count > 0 && this.currentWaypoint < this.waypoints.Count && this.waypoints[this.currentWaypoint].minJumpHeight > 0f;
			}
		}

		// Token: 0x06001929 RID: 6441 RVA: 0x00012B7E File Offset: 0x00010D7E
		private static float DistanceXZ(Vector3 a, Vector3 b)
		{
			a.y = 0f;
			b.y = 0f;
			return Vector3.Distance(a, b);
		}

		// Token: 0x0600192A RID: 6442 RVA: 0x00082CAC File Offset: 0x00080EAC
		public float CalculateJumpVelocityNeededToReachNextWaypoint(float moveSpeed)
		{
			if (!this.nextWaypointNeedsJump)
			{
				return 0f;
			}
			Vector3 vector = this.currentPosition;
			Vector3 nextPosition = this.GetNextPosition();
			return Trajectory.CalculateInitialYSpeed(Trajectory.CalculateGroundTravelTime(moveSpeed, PathFollower.DistanceXZ(vector, nextPosition)), nextPosition.y - vector.y);
		}

		// Token: 0x0600192B RID: 6443 RVA: 0x00082CF4 File Offset: 0x00080EF4
		public void UpdatePosition(Vector3 newPosition)
		{
			this.currentPosition = newPosition;
			Vector3 a;
			if (this.GetNextNodePosition(out a))
			{
				Vector3 vector = a - this.currentPosition;
				Vector3 vector2 = vector;
				vector2.y = 0f;
				float num = 2f;
				if (this.waypoints.Count > this.currentWaypoint + 1 && this.waypoints[this.currentWaypoint + 1].minJumpHeight > 0f)
				{
					num = 0.5f;
				}
				if (num * num >= vector2.sqrMagnitude && Mathf.Abs(vector.y) <= 2f)
				{
					this.SetWaypoint(this.currentWaypoint + 1);
				}
			}
			this.nextNode != NodeGraph.NodeIndex.invalid;
		}

		// Token: 0x0600192C RID: 6444 RVA: 0x00082DB0 File Offset: 0x00080FB0
		private void SetWaypoint(int newWaypoint)
		{
			this.currentWaypoint = Math.Min(newWaypoint, this.waypoints.Count);
			if (this.currentWaypoint == this.waypoints.Count)
			{
				this.nextNode = NodeGraph.NodeIndex.invalid;
				this.previousNode = NodeGraph.NodeIndex.invalid;
				return;
			}
			this.nextNode = this.waypoints[this.currentWaypoint].nodeIndex;
			this.previousNode = ((this.currentWaypoint > 0) ? this.waypoints[this.currentWaypoint - 1].nodeIndex : NodeGraph.NodeIndex.invalid);
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x00012B9F File Offset: 0x00010D9F
		private void Reset()
		{
			this.nodeGraph = null;
			this.nextNode = NodeGraph.NodeIndex.invalid;
			this.previousNode = NodeGraph.NodeIndex.invalid;
			this.waypoints.Clear();
			this.currentWaypoint = 0;
		}

		// Token: 0x0600192E RID: 6446 RVA: 0x00082E48 File Offset: 0x00081048
		public void SetPath(Path newPath)
		{
			if (this.nodeGraph != newPath.nodeGraph)
			{
				this.Reset();
				this.nodeGraph = newPath.nodeGraph;
			}
			this.waypoints.Clear();
			newPath.WriteWaypointsToList(this.waypoints);
			this.currentWaypoint = 0;
			for (int i = 1; i < this.waypoints.Count; i++)
			{
				if (this.waypoints[i].nodeIndex == this.nextNode && this.waypoints[i - 1].nodeIndex == this.previousNode)
				{
					this.currentWaypoint = i;
					break;
				}
			}
			this.SetWaypoint(this.currentWaypoint);
		}

		// Token: 0x0600192F RID: 6447 RVA: 0x00082F04 File Offset: 0x00081104
		private bool GetNextNodePosition(out Vector3 nextPosition)
		{
			if (this.nodeGraph != null && this.nextNode != NodeGraph.NodeIndex.invalid && this.nodeGraph.GetNodePosition(this.nextNode, out nextPosition))
			{
				return true;
			}
			nextPosition = Vector3.zero;
			return false;
		}

		// Token: 0x06001930 RID: 6448 RVA: 0x00082F54 File Offset: 0x00081154
		public Vector3 GetNextPosition()
		{
			Vector3 result;
			if (this.GetNextNodePosition(out result))
			{
				return result;
			}
			return this.targetPosition;
		}

		// Token: 0x06001931 RID: 6449 RVA: 0x00082F74 File Offset: 0x00081174
		public void DebugDrawPath(Color color, float duration)
		{
			for (int i = 1; i < this.waypoints.Count; i++)
			{
				Vector3 start;
				this.nodeGraph.GetNodePosition(this.waypoints[i].nodeIndex, out start);
				Vector3 end;
				this.nodeGraph.GetNodePosition(this.waypoints[i - 1].nodeIndex, out end);
				Debug.DrawLine(start, end, color, duration);
			}
			for (int j = 1; j < this.waypoints.Count; j++)
			{
				Vector3 a;
				this.nodeGraph.GetNodePosition(this.waypoints[j].nodeIndex, out a);
				Debug.DrawLine(a + Vector3.up, a - Vector3.up, color, duration);
			}
		}

		// Token: 0x04001C9B RID: 7323
		private Vector3 currentPosition;

		// Token: 0x04001C9C RID: 7324
		public Vector3 targetPosition;

		// Token: 0x04001C9D RID: 7325
		private const float waypointPassDistance = 2f;

		// Token: 0x04001C9E RID: 7326
		private const float waypointPassYTolerance = 2f;

		// Token: 0x04001C9F RID: 7327
		private NodeGraph nodeGraph;

		// Token: 0x04001CA0 RID: 7328
		private List<Path.Waypoint> waypoints = new List<Path.Waypoint>();

		// Token: 0x04001CA1 RID: 7329
		private int currentWaypoint;

		// Token: 0x04001CA2 RID: 7330
		private NodeGraph.NodeIndex previousNode = NodeGraph.NodeIndex.invalid;

		// Token: 0x04001CA3 RID: 7331
		private NodeGraph.NodeIndex nextNode = NodeGraph.NodeIndex.invalid;
	}
}
