using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003C3 RID: 963
	public class RunConsoleStringOnEnable : MonoBehaviour
	{
		// Token: 0x060014FD RID: 5373 RVA: 0x0000FE65 File Offset: 0x0000E065
		private void OnEnable()
		{
			Console.instance.SubmitCmd(null, this.consoleString, false);
		}

		// Token: 0x04001840 RID: 6208
		public string consoleString;
	}
}
