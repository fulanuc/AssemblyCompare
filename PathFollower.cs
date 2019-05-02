using System;
using System.Collections.Generic;
using RoR2.Navigation;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000471 RID: 1137
	public class PathFollower
	{
		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06001984 RID: 6532 RVA: 0x00003696 File Offset: 0x00001896
		public bool isOnJumpLink
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x06001985 RID: 6533 RVA: 0x00083604 File Offset: 0x00081804
		public bool nextWaypointNeedsJump
		{
			get
			{
				return this.waypoints.Count > 0 && this.currentWaypoint < this.waypoints.Count && this.waypoints[this.currentWaypoint].minJumpHeight > 0f;
			}
		}

		// Token: 0x06001986 RID: 6534 RVA: 0x00013098 File Offset: 0x00011298
		private static float DistanceXZ(Vector3 a, Vector3 b)
		{
			a.y = 0f;
			b.y = 0f;
			return Vector3.Distance(a, b);
		}

		// Token: 0x06001987 RID: 6535 RVA: 0x00083654 File Offset: 0x00081854
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

		// Token: 0x06001988 RID: 6536 RVA: 0x0008369C File Offset: 0x0008189C
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

		// Token: 0x06001989 RID: 6537 RVA: 0x00083758 File Offset: 0x00081958
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

		// Token: 0x0600198A RID: 6538 RVA: 0x000130B9 File Offset: 0x000112B9
		private void Reset()
		{
			this.nodeGraph = null;
			this.nextNode = NodeGraph.NodeIndex.invalid;
			this.previousNode = NodeGraph.NodeIndex.invalid;
			this.waypoints.Clear();
			this.currentWaypoint = 0;
		}

		// Token: 0x0600198B RID: 6539 RVA: 0x000837F0 File Offset: 0x000819F0
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

		// Token: 0x0600198C RID: 6540 RVA: 0x000838AC File Offset: 0x00081AAC
		private bool GetNextNodePosition(out Vector3 nextPosition)
		{
			if (this.nodeGraph != null && this.nextNode != NodeGraph.NodeIndex.invalid && this.nodeGraph.GetNodePosition(this.nextNode, out nextPosition))
			{
				return true;
			}
			nextPosition = Vector3.zero;
			return false;
		}

		// Token: 0x0600198D RID: 6541 RVA: 0x000838FC File Offset: 0x00081AFC
		public Vector3 GetNextPosition()
		{
			Vector3 result;
			if (this.GetNextNodePosition(out result))
			{
				return result;
			}
			return this.targetPosition;
		}

		// Token: 0x0600198E RID: 6542 RVA: 0x0008391C File Offset: 0x00081B1C
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

		// Token: 0x04001CCF RID: 7375
		private Vector3 currentPosition;

		// Token: 0x04001CD0 RID: 7376
		public Vector3 targetPosition;

		// Token: 0x04001CD1 RID: 7377
		private const float waypointPassDistance = 2f;

		// Token: 0x04001CD2 RID: 7378
		private const float waypointPassYTolerance = 2f;

		// Token: 0x04001CD3 RID: 7379
		private NodeGraph nodeGraph;

		// Token: 0x04001CD4 RID: 7380
		private List<Path.Waypoint> waypoints = new List<Path.Waypoint>();

		// Token: 0x04001CD5 RID: 7381
		private int currentWaypoint;

		// Token: 0x04001CD6 RID: 7382
		private NodeGraph.NodeIndex previousNode = NodeGraph.NodeIndex.invalid;

		// Token: 0x04001CD7 RID: 7383
		private NodeGraph.NodeIndex nextNode = NodeGraph.NodeIndex.invalid;
	}
}
