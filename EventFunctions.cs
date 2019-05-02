using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002EA RID: 746
	public class EventFunctions : MonoBehaviour
	{
		// Token: 0x06000F1E RID: 3870 RVA: 0x0000BA14 File Offset: 0x00009C14
		public void DestroySelf()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x0000BA21 File Offset: 0x00009C21
		public void DestroyGameObject(GameObject obj)
		{
			UnityEngine.Object.Destroy(obj);
		}

		// Token: 0x06000F20 RID: 3872 RVA: 0x0000BA29 File Offset: 0x00009C29
		public void UnparentTransform(Transform transform)
		{
			if (transform)
			{
				transform.SetParent(null);
			}
		}

		// Token: 0x06000F21 RID: 3873 RVA: 0x0000BA3A File Offset: 0x00009C3A
		public void ToggleGameObjectActive(GameObject obj)
		{
			obj.SetActive(!obj.activeSelf);
		}

		// Token: 0x06000F22 RID: 3874 RVA: 0x0000BA4B File Offset: 0x00009C4B
		public void OpenURL(string url)
		{
			Application.OpenURL(url);
		}
	}
}
