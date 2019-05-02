using System;

namespace RoR2.Navigation
{
	// Token: 0x02000527 RID: 1319
	public class PathTask
	{
		// Token: 0x06001DAF RID: 7599 RVA: 0x00015B9B File Offset: 0x00013D9B
		public PathTask(Path path)
		{
			this.path = path;
		}

		// Token: 0x06001DB0 RID: 7600 RVA: 0x000025F6 File Offset: 0x000007F6
		public void Wait()
		{
		}

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06001DB1 RID: 7601 RVA: 0x00015BAA File Offset: 0x00013DAA
		// (set) Token: 0x06001DB2 RID: 7602 RVA: 0x00015BB2 File Offset: 0x00013DB2
		public Path path { get; private set; }

		// Token: 0x04001FE8 RID: 8168
		public PathTask.TaskStatus status;

		// Token: 0x02000528 RID: 1320
		public enum TaskStatus
		{
			// Token: 0x04001FEB RID: 8171
			NotStarted,
			// Token: 0x04001FEC RID: 8172
			Running,
			// Token: 0x04001FED RID: 8173
			Complete
		}
	}
}
