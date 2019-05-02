using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x02000264 RID: 612
	public class AwakeEvent : MonoBehaviour
	{
		// Token: 0x06000B5D RID: 2909 RVA: 0x00009123 File Offset: 0x00007323
		private void Awake()
		{
			this.action.Invoke();
		}

		// Token: 0x04000F5E RID: 3934
		public UnityEvent action;
	}
}
