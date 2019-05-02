using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000466 RID: 1126
	public struct MemoizedGetComponent<TComponent> where TComponent : Component
	{
		// Token: 0x0600195B RID: 6491 RVA: 0x00082A1C File Offset: 0x00080C1C
		public TComponent Get(GameObject gameObject)
		{
			if (this.cachedGameObject != gameObject)
			{
				this.cachedGameObject = gameObject;
				this.cachedValue = (gameObject ? gameObject.GetComponent<TComponent>() : default(TComponent));
			}
			return this.cachedValue;
		}

		// Token: 0x04001C92 RID: 7314
		private GameObject cachedGameObject;

		// Token: 0x04001C93 RID: 7315
		private TComponent cachedValue;
	}
}
