using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000406 RID: 1030
	public class ConsoleFunctions : MonoBehaviour
	{
		// Token: 0x060016FE RID: 5886 RVA: 0x0001137B File Offset: 0x0000F57B
		public void SubmitCmd(string cmd)
		{
			Console.instance.SubmitCmd(null, cmd, false);
		}
	}
}
