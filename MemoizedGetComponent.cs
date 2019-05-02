using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200045B RID: 1115
	public struct MemoizedGetComponent<TComponent> where TComponent : Component
	{
		// Token: 0x060018FF RID: 6399 RVA: 0x00082074 File Offset: 0x00080274
		public TComponent Get(GameObject gameObject)
		{
			if (this.cachedGameObject != gameObject)
			{
				this.cachedGameObject = gameObject;
				this.cachedValue = (gameObject ? gameObject.GetComponent<TComponent>() : default(TComponent));
			}
			return this.cachedValue;
		}

		// Token: 0x04001C5E RID: 7262
		private GameObject cachedGameObject;

		// Token: 0x04001C5F RID: 7263
		private TComponent cachedValue;
	}
}
