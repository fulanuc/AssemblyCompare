using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x02000264 RID: 612
	public class AwakeEvent : MonoBehaviour
	{
		// Token: 0x06000B60 RID: 2912 RVA: 0x00009148 File Offset: 0x00007348
		private void Awake()
		{
			this.action.Invoke();
		}

		// Token: 0x04000F64 RID: 3940
		public UnityEvent action;
	}
}
