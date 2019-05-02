using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000358 RID: 856
	public class MercDashSkill : GenericSkill
	{
		// Token: 0x17000185 RID: 389
		// (get) Token: 0x060011A9 RID: 4521 RVA: 0x0000D737 File Offset: 0x0000B937
		// (set) Token: 0x060011AA RID: 4522 RVA: 0x0000D73F File Offset: 0x0000B93F
		public int currentDashIndex { get; private set; }

		// Token: 0x060011AB RID: 4523 RVA: 0x00066B44 File Offset: 0x00064D44
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

		// Token: 0x060011AC RID: 4524 RVA: 0x00066B88 File Offset: 0x00064D88
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

		// Token: 0x060011AD RID: 4525 RVA: 0x0000D748 File Offset: 0x0000B948
		protected override void OnExecute()
		{
			base.OnExecute();
			this.timeoutTimer = this.timeoutDuration;
		}

		// Token: 0x040015A2 RID: 5538
		public int maxDashes;

		// Token: 0x040015A3 RID: 5539
		private float timeoutTimer;

		// Token: 0x040015A4 RID: 5540
		public float timeoutDuration;

		// Token: 0x040015A5 RID: 5541
		public Sprite[] icons;
	}
}
