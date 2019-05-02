using System;

namespace RoR2.Navigation
{
	// Token: 0x02000536 RID: 1334
	public class PathTask
	{
		// Token: 0x06001E17 RID: 7703 RVA: 0x00016064 File Offset: 0x00014264
		public PathTask(Path path)
		{
			this.path = path;
		}

		// Token: 0x06001E18 RID: 7704 RVA: 0x000025DA File Offset: 0x000007DA
		public void Wait()
		{
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x06001E19 RID: 7705 RVA: 0x00016073 File Offset: 0x00014273
		// (set) Token: 0x06001E1A RID: 7706 RVA: 0x0001607B File Offset: 0x0001427B
		public Path path { get; private set; }

		// Token: 0x04002026 RID: 8230
		public PathTask.TaskStatus status;

		// Token: 0x02000537 RID: 1335
		public enum TaskStatus
		{
			// Token: 0x04002029 RID: 8233
			NotStarted,
			// Token: 0x0400202A RID: 8234
			Running,
			// Token: 0x0400202B RID: 8235
			Complete
		}
	}
}
