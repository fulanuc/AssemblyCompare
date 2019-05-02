using System;

namespace RoR2
{
	// Token: 0x020004D0 RID: 1232
	public class TimerQueue
	{
		// Token: 0x06001BDB RID: 7131 RVA: 0x000896F8 File Offset: 0x000878F8
		public TimerQueue.TimerHandle CreateTimer(float time, Action action)
		{
			time += this.internalTime;
			int position = this.count;
			for (int i = 0; i < this.count; i++)
			{
				if (time < this.timers[i].time)
				{
					position = i;
					break;
				}
			}
			TimerQueue.TimerHandle timerHandle = new TimerQueue.TimerHandle(this.indexAllocator.RequestIndex());
			TimerQueue.Timer timer = new TimerQueue.Timer
			{
				time = time,
				action = action,
				handle = timerHandle
			};
			HGArrayUtilities.ArrayInsert<TimerQueue.Timer>(ref this.timers, ref this.count, position, ref timer);
			return timerHandle;
		}

		// Token: 0x06001BDC RID: 7132 RVA: 0x0008978C File Offset: 0x0008798C
		public void RemoveTimer(TimerQueue.TimerHandle timerHandle)
		{
			for (int i = 0; i < this.count; i++)
			{
				if (this.timers[i].handle.Equals(timerHandle))
				{
					this.RemoveTimerAt(i);
					return;
				}
			}
		}

		// Token: 0x06001BDD RID: 7133 RVA: 0x00014A39 File Offset: 0x00012C39
		private void RemoveTimerAt(int i)
		{
			this.indexAllocator.FreeIndex(this.timers[i].handle.uid);
			HGArrayUtilities.ArrayRemoveAt<TimerQueue.Timer>(ref this.timers, ref this.count, i, 1);
		}

		// Token: 0x06001BDE RID: 7134 RVA: 0x000897CC File Offset: 0x000879CC
		public void Update(float deltaTime)
		{
			this.internalTime += deltaTime;
			int num = 0;
			while (num < this.count && this.timers[num].time <= this.internalTime)
			{
				HGArrayUtilities.ArrayInsert<Action>(ref this.actionsToCall, ref this.actionsToCallCount, this.actionsToCallCount, ref this.timers[num].action);
				num++;
			}
			for (int i = this.actionsToCallCount - 1; i >= 0; i--)
			{
				this.RemoveTimerAt(i);
			}
			for (int j = 0; j < this.actionsToCallCount; j++)
			{
				this.actionsToCall[j]();
				this.actionsToCall[j] = null;
			}
			this.actionsToCallCount = 0;
		}

		// Token: 0x04001E37 RID: 7735
		private float internalTime;

		// Token: 0x04001E38 RID: 7736
		private int count;

		// Token: 0x04001E39 RID: 7737
		private TimerQueue.Timer[] timers = Array.Empty<TimerQueue.Timer>();

		// Token: 0x04001E3A RID: 7738
		private readonly IndexAllocator indexAllocator = new IndexAllocator();

		// Token: 0x04001E3B RID: 7739
		private Action[] actionsToCall = Array.Empty<Action>();

		// Token: 0x04001E3C RID: 7740
		private int actionsToCallCount;

		// Token: 0x020004D1 RID: 1233
		public struct TimerHandle : IEquatable<TimerQueue.TimerHandle>
		{
			// Token: 0x06001BE0 RID: 7136 RVA: 0x00014A98 File Offset: 0x00012C98
			public TimerHandle(int uid)
			{
				this.uid = uid;
			}

			// Token: 0x06001BE1 RID: 7137 RVA: 0x00014AA1 File Offset: 0x00012CA1
			public bool Equals(TimerQueue.TimerHandle other)
			{
				return this.uid == other.uid;
			}

			// Token: 0x06001BE2 RID: 7138 RVA: 0x00014AB1 File Offset: 0x00012CB1
			public override bool Equals(object obj)
			{
				return obj != null && obj is TimerQueue.TimerHandle && this.Equals((TimerQueue.TimerHandle)obj);
			}

			// Token: 0x06001BE3 RID: 7139 RVA: 0x00014ACE File Offset: 0x00012CCE
			public override int GetHashCode()
			{
				return this.uid;
			}

			// Token: 0x04001E3D RID: 7741
			public static readonly TimerQueue.TimerHandle invalid = new TimerQueue.TimerHandle(-1);

			// Token: 0x04001E3E RID: 7742
			public readonly int uid;
		}

		// Token: 0x020004D2 RID: 1234
		private struct Timer
		{
			// Token: 0x04001E3F RID: 7743
			public float time;

			// Token: 0x04001E40 RID: 7744
			public Action action;

			// Token: 0x04001E41 RID: 7745
			public TimerQueue.TimerHandle handle;
		}
	}
}
