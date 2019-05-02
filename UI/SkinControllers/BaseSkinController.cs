using System;
using UnityEngine;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x0200066A RID: 1642
	[ExecuteAlways]
	public abstract class BaseSkinController : MonoBehaviour
	{
		// Token: 0x060024E3 RID: 9443
		protected abstract void OnSkinUI();

		// Token: 0x060024E4 RID: 9444 RVA: 0x0001AE1C File Offset: 0x0001901C
		protected void Awake()
		{
			if (this.skinData)
			{
				this.DoSkinUI();
			}
		}

		// Token: 0x060024E5 RID: 9445 RVA: 0x0001AE31 File Offset: 0x00019031
		private void DoSkinUI()
		{
			if (this.skinData)
			{
				this.OnSkinUI();
			}
		}

		// Token: 0x0400279B RID: 10139
		public UISkinData skinData;
	}
}
