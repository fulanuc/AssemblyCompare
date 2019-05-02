using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x02000380 RID: 896
	public class OnEnableEvent : MonoBehaviour
	{
		// Token: 0x060012CE RID: 4814 RVA: 0x0000E66A File Offset: 0x0000C86A
		private void OnEnable()
		{
			this.action.Invoke();
		}

		// Token: 0x04001662 RID: 5730
		public UnityEvent action;
	}
}
