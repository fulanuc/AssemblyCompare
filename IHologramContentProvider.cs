using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000330 RID: 816
	internal interface IHologramContentProvider
	{
		// Token: 0x060010DE RID: 4318
		bool ShouldDisplayHologram(GameObject viewer);

		// Token: 0x060010DF RID: 4319
		GameObject GetHologramContentPrefab();

		// Token: 0x060010E0 RID: 4320
		void UpdateHologramContent(GameObject hologramContentObject);
	}
}
