using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x0200037B RID: 891
	public class OnEnableEvent : MonoBehaviour
	{
		// Token: 0x060012AE RID: 4782 RVA: 0x0000E4DF File Offset: 0x0000C6DF
		private void OnEnable()
		{
			this.action.Invoke();
		}

		// Token: 0x04001646 RID: 5702
		public UnityEvent action;
	}
}
