using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005B2 RID: 1458
	public class ButtonSelectionController : MonoBehaviour
	{
		// Token: 0x060020B7 RID: 8375 RVA: 0x0009E8FC File Offset: 0x0009CAFC
		public void SelectThisButton(MPButton selectedButton)
		{
			for (int i = 0; i < this.buttons.Length; i++)
			{
				this.buttons[i] == selectedButton;
			}
		}

		// Token: 0x04002333 RID: 9011
		public MPButton[] buttons;
	}
}
