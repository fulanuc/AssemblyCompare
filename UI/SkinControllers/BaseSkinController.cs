using System;
using UnityEngine;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x02000658 RID: 1624
	[ExecuteAlways]
	public abstract class BaseSkinController : MonoBehaviour
	{
		// Token: 0x06002452 RID: 9298
		protected abstract void OnSkinUI();

		// Token: 0x06002453 RID: 9299 RVA: 0x0001A723 File Offset: 0x00018923
		protected void Awake()
		{
			if (this.skinData)
			{
				this.DoSkinUI();
			}
		}

		// Token: 0x06002454 RID: 9300 RVA: 0x0001A738 File Offset: 0x00018938
		private void DoSkinUI()
		{
			if (this.skinData)
			{
				this.OnSkinUI();
			}
		}

		// Token: 0x04002740 RID: 10048
		public UISkinData skinData;
	}
}
