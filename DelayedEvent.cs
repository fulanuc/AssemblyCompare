using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x020002C9 RID: 713
	public class DelayedEvent : MonoBehaviour
	{
		// Token: 0x06000E7B RID: 3707 RVA: 0x00059010 File Offset: 0x00057210
		public void CallDelayed(float timer)
		{
			TimerQueue timerQueue = null;
			switch (this.timeStepType)
			{
			case DelayedEvent.TimeStepType.Time:
				timerQueue = RoR2Application.timeTimers;
				break;
			case DelayedEvent.TimeStepType.UnscaledTime:
				timerQueue = RoR2Application.unscaledTimeTimers;
				break;
			case DelayedEvent.TimeStepType.FixedTime:
				timerQueue = RoR2Application.fixedTimeTimers;
				break;
			}
			if (timerQueue != null)
			{
				timerQueue.CreateTimer(timer, new Action(this.Call));
			}
		}

		// Token: 0x06000E7C RID: 3708 RVA: 0x0000B2C1 File Offset: 0x000094C1
		private void Call()
		{
			if (this)
			{
				this.action.Invoke();
			}
		}

		// Token: 0x04001269 RID: 4713
		public UnityEvent action;

		// Token: 0x0400126A RID: 4714
		public DelayedEvent.TimeStepType timeStepType;

		// Token: 0x020002CA RID: 714
		public enum TimeStepType
		{
			// Token: 0x0400126C RID: 4716
			Time,
			// Token: 0x0400126D RID: 4717
			UnscaledTime,
			// Token: 0x0400126E RID: 4718
			FixedTime
		}
	}
}
