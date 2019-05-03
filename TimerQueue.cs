using System;

namespace RoR2
{
	// Token: 0x020004C2 RID: 1218
	public class TimerQueue
	{
		// Token: 0x06001B77 RID: 7031 RVA: 0x00088B80 File Offset: 0x00086D80
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

		// Token: 0x06001B78 RID: 7032 RVA: 0x00088C14 File Offset: 0x00086E14
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

		// Token: 0x06001B79 RID: 7033 RVA: 0x0001456C File Offset: 0x0001276C
		private void RemoveTimerAt(int i)
		{
			this.indexAllocator.FreeIndex(this.timers[i].handle.uid);
			HGArrayUtilities.ArrayRemoveAt<TimerQueue.Timer>(ref this.timers, ref this.count, i, 1);
		}

		// Token: 0x06001B7A RID: 7034 RVA: 0x00088C54 File Offset: 0x00086E54
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

		// Token: 0x04001DFD RID: 7677
		private float internalTime;

		// Token: 0x04001DFE RID: 7678
		private int count;

		// Token: 0x04001DFF RID: 7679
		private TimerQueue.Timer[] timers = Array.Empty<TimerQueue.Timer>();

		// Token: 0x04001E00 RID: 7680
		private readonly IndexAllocator indexAllocator = new IndexAllocator();

		// Token: 0x04001E01 RID: 7681
		private Action[] actionsToCall = Array.Empty<Action>();

		// Token: 0x04001E02 RID: 7682
		private int actionsToCallCount;

		// Token: 0x020004C3 RID: 1219
		public struct TimerHandle : IEquatable<TimerQueue.TimerHandle>
		{
			// Token: 0x06001B7C RID: 7036 RVA: 0x000145CB File Offset: 0x000127CB
			public TimerHandle(int uid)
			{
				this.uid = uid;
			}

			// Token: 0x06001B7D RID: 7037 RVA: 0x000145D4 File Offset: 0x000127D4
			public bool Equals(TimerQueue.TimerHandle other)
			{
				return this.uid == other.uid;
			}

			// Token: 0x06001B7E RID: 7038 RVA: 0x000145E4 File Offset: 0x000127E4
			public override bool Equals(object obj)
			{
				return obj != null && obj is TimerQueue.TimerHandle && this.Equals((TimerQueue.TimerHandle)obj);
			}

			// Token: 0x06001B7F RID: 7039 RVA: 0x00014601 File Offset: 0x00012801
			public override int GetHashCode()
			{
				return this.uid;
			}

			// Token: 0x04001E03 RID: 7683
			public static readonly TimerQueue.TimerHandle invalid = new TimerQueue.TimerHandle(-1);

			// Token: 0x04001E04 RID: 7684
			public readonly int uid;
		}

		// Token: 0x020004C4 RID: 1220
		private struct Timer
		{
			// Token: 0x04001E05 RID: 7685
			public float time;

			// Token: 0x04001E06 RID: 7686
			public Action action;

			// Token: 0x04001E07 RID: 7687
			public TimerQueue.TimerHandle handle;
		}
	}
}
