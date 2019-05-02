using System;

namespace RoR2
{
	// Token: 0x0200031A RID: 794
	public class HuntressTrackingSkill : GenericSkill
	{
		// Token: 0x0600106D RID: 4205 RVA: 0x0000C977 File Offset: 0x0000AB77
		protected new void Start()
		{
			base.Start();
			this.huntressTracker = base.GetComponent<HuntressTracker>();
		}

		// Token: 0x0600106E RID: 4206 RVA: 0x0000C98B File Offset: 0x0000AB8B
		public override bool CanExecute()
		{
			return (!this.huntressTracker || !(this.huntressTracker.GetTrackingTarget() == null)) && base.CanExecute();
		}

		// Token: 0x0400147B RID: 5243
		private HuntressTracker huntressTracker;
	}
}
