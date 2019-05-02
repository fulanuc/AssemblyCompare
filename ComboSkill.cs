using System;
using EntityStates;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002A6 RID: 678
	public class ComboSkill : GenericSkill
	{
		// Token: 0x06000DDA RID: 3546 RVA: 0x0000ABA1 File Offset: 0x00008DA1
		protected new void Start()
		{
			base.Start();
		}

		// Token: 0x06000DDB RID: 3547 RVA: 0x00056570 File Offset: 0x00054770
		protected override void OnExecute()
		{
			this.activationState = this.comboList[this.comboCounter].comboActivationState;
			base.OnExecute();
			if (this.hasExecutedSuccessfully)
			{
				this.comboCounter++;
				if (this.comboCounter >= this.comboList.Length)
				{
					this.comboCounter = 0;
				}
			}
		}

		// Token: 0x06000DDC RID: 3548 RVA: 0x0000ABA9 File Offset: 0x00008DA9
		private void Update()
		{
			this.icon = this.comboList[this.comboCounter].comboIcon;
		}

		// Token: 0x040011CC RID: 4556
		public ComboSkill.Combo[] comboList;

		// Token: 0x040011CD RID: 4557
		private int comboCounter;

		// Token: 0x020002A7 RID: 679
		[Serializable]
		public struct Combo
		{
			// Token: 0x040011CE RID: 4558
			public SerializableEntityStateType comboActivationState;

			// Token: 0x040011CF RID: 4559
			public Sprite comboIcon;
		}
	}
}
