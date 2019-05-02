using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003C9 RID: 969
	public class RunConsoleStringOnEnable : MonoBehaviour
	{
		// Token: 0x06001524 RID: 5412 RVA: 0x000100D5 File Offset: 0x0000E2D5
		private void OnEnable()
		{
			Console.instance.SubmitCmd(null, this.consoleString, false);
		}

		// Token: 0x0400185F RID: 6239
		public string consoleString;
	}
}
