using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200032E RID: 814
	internal interface IHologramContentProvider
	{
		// Token: 0x060010CA RID: 4298
		bool ShouldDisplayHologram(GameObject viewer);

		// Token: 0x060010CB RID: 4299
		GameObject GetHologramContentPrefab();

		// Token: 0x060010CC RID: 4300
		void UpdateHologramContent(GameObject hologramContentObject);
	}
}
