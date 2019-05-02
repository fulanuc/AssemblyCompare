using System;
using System.Collections.Generic;
using RoR2.Navigation;

namespace RoR2
{
	// Token: 0x0200046F RID: 1135
	public class Path : IDisposable
	{
		// Token: 0x06001975 RID: 6517 RVA: 0x00012FBC File Offset: 0x000111BC
		private static Path.Waypoint[] GetWaypointsBuffer()
		{
			if (Path.waypointsBufferPool.Count == 0)
			{
				return new Path.Waypoint[64];
			}
			return Path.waypointsBufferPool.Pop();
		}

		// Token: 0x06001976 RID: 6518 RVA: 0x00012FDC File Offset: 0x000111DC
		private static void FreeWaypointsBuffer(Path.Waypoint[] buffer)
		{
			if (buffer.Length != 64)
			{
				return;
			}
			Path.waypointsBufferPool.Push(buffer);
		}

		// Token: 0x06001977 RID: 6519 RVA: 0x00012FF1 File Offset: 0x000111F1
		public Path(NodeGraph nodeGraph)
		{
			this.nodeGraph = nodeGraph;
			this.waypointsBuffer = Path.GetWaypointsBuffer();
			this.firstWaypointIndex = this.waypointsBuffer.Length;
		}

		// Token: 0x06001978 RID: 6520 RVA: 0x00013019 File Offset: 0x00011219
		public void Dispose()
		{
			Path.FreeWaypointsBuffer(this.waypointsBuffer);
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06001979 RID: 6521 RVA: 0x00013026 File Offset: 0x00011226
		// (set) Token: 0x0600197A RID: 6522 RVA: 0x0001302E File Offset: 0x0001122E
		public NodeGraph nodeGraph { get; private set; }

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x0600197B RID: 6523 RVA: 0x00013037 File Offset: 0x00011237
		// (set) Token: 0x0600197C RID: 6524 RVA: 0x0001303F File Offset: 0x0001123F
		public PathStatus status { get; set; }

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x0600197D RID: 6525 RVA: 0x00013048 File Offset: 0x00011248
		// (set) Token: 0x0600197E RID: 6526 RVA: 0x00013050 File Offset: 0x00011250
		public int waypointsCount { get; private set; }

		// Token: 0x1700025B RID: 603
		public Path.Waypoint this[int i]
		{
			get
			{
				return this.waypointsBuffer[this.firstWaypointIndex + i];
			}
		}

		// Token: 0x06001980 RID: 6528 RVA: 0x000834FC File Offset: 0x000816FC
		public void PushWaypointToFront(NodeGraph.NodeIndex nodeIndex, float minJumpHeight)
		{
			if (this.waypointsCount + 1 >= this.waypointsBuffer.Length)
			{
				Path.Waypoint[] array = this.waypointsBuffer;
				this.waypointsBuffer = new Path.Waypoint[this.waypointsCount + 32];
				Array.Copy(array, 0, this.waypointsBuffer, this.waypointsBuffer.Length - array.Length, array.Length);
				Path.FreeWaypointsBuffer(array);
			}
			int num = this.waypointsBuffer.Length;
			int num2 = this.waypointsCount + 1;
			this.waypointsCount = num2;
			this.firstWaypointIndex = num - num2;
			this.waypointsBuffer[this.firstWaypointIndex] = new Path.Waypoint
			{
				nodeIndex = nodeIndex,
				minJumpHeight = minJumpHeight
			};
		}

		// Token: 0x06001981 RID: 6529 RVA: 0x000835A4 File Offset: 0x000817A4
		public void WriteWaypointsToList(List<Path.Waypoint> waypointsList)
		{
			if (waypointsList.Capacity < waypointsList.Count + this.waypointsCount)
			{
				waypointsList.Capacity = waypointsList.Count + this.waypointsCount;
			}
			for (int i = this.firstWaypointIndex; i < this.waypointsBuffer.Length; i++)
			{
				waypointsList.Add(this.waypointsBuffer[i]);
			}
		}

		// Token: 0x06001982 RID: 6530 RVA: 0x0001306E File Offset: 0x0001126E
		public void Clear()
		{
			this.status = PathStatus.Invalid;
			this.waypointsCount = 0;
			this.firstWaypointIndex = this.waypointsBuffer.Length;
		}

		// Token: 0x04001CC6 RID: 7366
		private static readonly Stack<Path.Waypoint[]> waypointsBufferPool = new Stack<Path.Waypoint[]>();

		// Token: 0x04001CC7 RID: 7367
		private const int pooledBufferSize = 64;

		// Token: 0x04001CCA RID: 7370
		private Path.Waypoint[] waypointsBuffer;

		// Token: 0x04001CCC RID: 7372
		private int firstWaypointIndex;

		// Token: 0x02000470 RID: 1136
		public struct Waypoint
		{
			// Token: 0x04001CCD RID: 7373
			public NodeGraph.NodeIndex nodeIndex;

			// Token: 0x04001CCE RID: 7374
			public float minJumpHeight;
		}
	}
}
