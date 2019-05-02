using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200035B RID: 859
	public class MercDashSkill : GenericSkill
	{
		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060011C0 RID: 4544 RVA: 0x0000D820 File Offset: 0x0000BA20
		// (set) Token: 0x060011C1 RID: 4545 RVA: 0x0000D828 File Offset: 0x0000BA28
		public int currentDashIndex { get; private set; }

		// Token: 0x060011C2 RID: 4546 RVA: 0x00066E7C File Offset: 0x0006507C
		public void AddHit()
		{
			int num = this.currentDashIndex + 1;
			this.currentDashIndex = num;
			if (this.currentDashIndex < this.maxDashes)
			{
				num = base.stock + 1;
				base.stock = num;
				return;
			}
			this.currentDashIndex = 0;
		}

		// Token: 0x060011C3 RID: 4547 RVA: 0x00066EC0 File Offset: 0x000650C0
		protected new void FixedUpdate()
		{
			base.FixedUpdate();
			this.timeoutTimer -= Time.fixedDeltaTime;
			if (this.timeoutTimer <= 0f && this.currentDashIndex != 0)
			{
				base.stock = 0;
				this.currentDashIndex = 0;
			}
			int num = this.currentDashIndex;
			if (num >= this.icons.Length)
			{
				num = this.icons.Length - 1;
			}
			this.icon = this.icons[num];
		}

		// Token: 0x060011C4 RID: 4548 RVA: 0x0000D831 File Offset: 0x0000BA31
		protected override void OnExecute()
		{
			base.OnExecute();
			this.timeoutTimer = this.timeoutDuration;
		}

		// Token: 0x040015BB RID: 5563
		public int maxDashes;

		// Token: 0x040015BC RID: 5564
		private float timeoutTimer;

		// Token: 0x040015BD RID: 5565
		public float timeoutDuration;

		// Token: 0x040015BE RID: 5566
		public Sprite[] icons;
	}
}
