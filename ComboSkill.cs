using System;
using EntityStates;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002A8 RID: 680
	public class ComboSkill : GenericSkill
	{
		// Token: 0x06000DE1 RID: 3553 RVA: 0x0000ABF3 File Offset: 0x00008DF3
		protected new void Start()
		{
			base.Start();
		}

		// Token: 0x06000DE2 RID: 3554 RVA: 0x000564B4 File Offset: 0x000546B4
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

		// Token: 0x06000DE3 RID: 3555 RVA: 0x0000ABFB File Offset: 0x00008DFB
		private void Update()
		{
			this.icon = this.comboList[this.comboCounter].comboIcon;
		}

		// Token: 0x040011DE RID: 4574
		public ComboSkill.Combo[] comboList;

		// Token: 0x040011DF RID: 4575
		private int comboCounter;

		// Token: 0x020002A9 RID: 681
		[Serializable]
		public struct Combo
		{
			// Token: 0x040011E0 RID: 4576
			public SerializableEntityStateType comboActivationState;

			// Token: 0x040011E1 RID: 4577
			public Sprite comboIcon;
		}
	}
}
