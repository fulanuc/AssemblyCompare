using System;

namespace RoR2
{
	// Token: 0x0200031C RID: 796
	public class HuntressTrackingSkill : GenericSkill
	{
		// Token: 0x06001081 RID: 4225 RVA: 0x0000CA5B File Offset: 0x0000AC5B
		protected new void Start()
		{
			base.Start();
			this.huntressTracker = base.GetComponent<HuntressTracker>();
		}

		// Token: 0x06001082 RID: 4226 RVA: 0x0000CA6F File Offset: 0x0000AC6F
		public override bool CanExecute()
		{
			return (!this.huntressTracker || !(this.huntressTracker.GetTrackingTarget() == null)) && base.CanExecute();
		}

		// Token: 0x0400148F RID: 5263
		private HuntressTracker huntressTracker;
	}
}
