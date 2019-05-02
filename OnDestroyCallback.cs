using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200037A RID: 890
	public class OnDestroyCallback : MonoBehaviour
	{
		// Token: 0x060012AA RID: 4778 RVA: 0x0000E4AB File Offset: 0x0000C6AB
		public void OnDestroy()
		{
			if (this.callback != null)
			{
				this.callback(this);
			}
		}

		// Token: 0x060012AB RID: 4779 RVA: 0x0000E4C1 File Offset: 0x0000C6C1
		public static OnDestroyCallback AddCallback(GameObject gameObject, Action<OnDestroyCallback> callback)
		{
			OnDestroyCallback onDestroyCallback = gameObject.AddComponent<OnDestroyCallback>();
			onDestroyCallback.callback = callback;
			return onDestroyCallback;
		}

		// Token: 0x060012AC RID: 4780 RVA: 0x0000E4D0 File Offset: 0x0000C6D0
		public static void RemoveCallback(OnDestroyCallback callbackComponent)
		{
			callbackComponent.callback = null;
			UnityEngine.Object.Destroy(callbackComponent);
		}

		// Token: 0x04001645 RID: 5701
		private Action<OnDestroyCallback> callback;
	}
}
