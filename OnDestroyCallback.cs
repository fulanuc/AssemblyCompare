using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200037F RID: 895
	public class OnDestroyCallback : MonoBehaviour
	{
		// Token: 0x060012CA RID: 4810 RVA: 0x0000E636 File Offset: 0x0000C836
		public void OnDestroy()
		{
			if (this.callback != null)
			{
				this.callback(this);
			}
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x0000E64C File Offset: 0x0000C84C
		public static OnDestroyCallback AddCallback(GameObject gameObject, Action<OnDestroyCallback> callback)
		{
			OnDestroyCallback onDestroyCallback = gameObject.AddComponent<OnDestroyCallback>();
			onDestroyCallback.callback = callback;
			return onDestroyCallback;
		}

		// Token: 0x060012CC RID: 4812 RVA: 0x0000E65B File Offset: 0x0000C85B
		public static void RemoveCallback(OnDestroyCallback callbackComponent)
		{
			callbackComponent.callback = null;
			UnityEngine.Object.Destroy(callbackComponent);
		}

		// Token: 0x04001661 RID: 5729
		private Action<OnDestroyCallback> callback;
	}
}
