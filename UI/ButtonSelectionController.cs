using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005C4 RID: 1476
	public class ButtonSelectionController : MonoBehaviour
	{
		// Token: 0x06002148 RID: 8520 RVA: 0x0009FED0 File Offset: 0x0009E0D0
		public void SelectThisButton(MPButton selectedButton)
		{
			for (int i = 0; i < this.buttons.Length; i++)
			{
				this.buttons[i] == selectedButton;
			}
		}

		// Token: 0x04002387 RID: 9095
		public MPButton[] buttons;
	}
}
