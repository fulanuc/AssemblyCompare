using System;
using System.Collections.Generic;
using RoR2.Navigation;

namespace RoR2
{
	// Token: 0x02000464 RID: 1124
	public class Path : IDisposable
	{
		// Token: 0x06001918 RID: 6424 RVA: 0x00012AA2 File Offset: 0x00010CA2
		private static Path.Waypoint[] GetWaypointsBuffer()
		{
			if (Path.waypointsBufferPool.Count == 0)
			{
				return new Path.Waypoint[64];
			}
			return Path.waypointsBufferPool.Pop();
		}

		// Token: 0x06001919 RID: 6425 RVA: 0x00012AC2 File Offset: 0x00010CC2
		private static void FreeWaypointsBuffer(Path.Waypoint[] buffer)
		{
			if (buffer.Length != 64)
			{
				return;
			}
			Path.waypointsBufferPool.Push(buffer);
		}

		// Token: 0x0600191A RID: 6426 RVA: 0x00012AD7 File Offset: 0x00010CD7
		public Path(NodeGraph nodeGraph)
		{
			this.nodeGraph = nodeGraph;
			this.waypointsBuffer = Path.GetWaypointsBuffer();
			this.firstWaypointIndex = this.waypointsBuffer.Length;
		}

		// Token: 0x0600191B RID: 6427 RVA: 0x00012AFF File Offset: 0x00010CFF
		public void Dispose()
		{
			Path.FreeWaypointsBuffer(this.waypointsBuffer);
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x0600191C RID: 6428 RVA: 0x00012B0C File Offset: 0x00010D0C
		// (set) Token: 0x0600191D RID: 6429 RVA: 0x00012B14 File Offset: 0x00010D14
		public NodeGraph nodeGraph { get; private set; }

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x0600191E RID: 6430 RVA: 0x00012B1D File Offset: 0x00010D1D
		// (set) Token: 0x0600191F RID: 6431 RVA: 0x00012B25 File Offset: 0x00010D25
		public PathStatus status { get; set; }

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x06001920 RID: 6432 RVA: 0x00012B2E File Offset: 0x00010D2E
		// (set) Token: 0x06001921 RID: 6433 RVA: 0x00012B36 File Offset: 0x00010D36
		public int waypointsCount { get; private set; }

		// Token: 0x1700024F RID: 591
		public Path.Waypoint this[int i]
		{
			get
			{
				return this.waypointsBuffer[this.firstWaypointIndex + i];
			}
		}

		// Token: 0x06001923 RID: 6435 RVA: 0x00082B54 File Offset: 0x00080D54
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

		// Token: 0x06001924 RID: 6436 RVA: 0x00082BFC File Offset: 0x00080DFC
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

		// Token: 0x06001925 RID: 6437 RVA: 0x00012B54 File Offset: 0x00010D54
		public void Clear()
		{
			this.status = PathStatus.Invalid;
			this.waypointsCount = 0;
			this.firstWaypointIndex = this.waypointsBuffer.Length;
		}

		// Token: 0x04001C92 RID: 7314
		private static readonly Stack<Path.Waypoint[]> waypointsBufferPool = new Stack<Path.Waypoint[]>();

		// Token: 0x04001C93 RID: 7315
		private const int pooledBufferSize = 64;

		// Token: 0x04001C96 RID: 7318
		private Path.Waypoint[] waypointsBuffer;

		// Token: 0x04001C98 RID: 7320
		private int firstWaypointIndex;

		// Token: 0x02000465 RID: 1125
		public struct Waypoint
		{
			// Token: 0x04001C99 RID: 7321
			public NodeGraph.NodeIndex nodeIndex;

			// Token: 0x04001C9A RID: 7322
			public float minJumpHeight;
		}
	}
}
