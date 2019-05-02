using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000225 RID: 549
	[CreateAssetMenu]
	public class DirectorCardCategorySelection : ScriptableObject
	{
		// Token: 0x06000AA9 RID: 2729 RVA: 0x00048C10 File Offset: 0x00046E10
		public float SumAllWeightsInCategory(DirectorCardCategorySelection.Category category)
		{
			float num = 0f;
			for (int i = 0; i < category.cards.Length; i++)
			{
				num += (float)category.cards[i].selectionWeight;
			}
			return num;
		}

		// Token: 0x04000E0D RID: 3597
		public DirectorCardCategorySelection.Category[] categories;

		// Token: 0x02000226 RID: 550
		[Serializable]
		public struct Category
		{
			// Token: 0x04000E0E RID: 3598
			[Tooltip("A name to help identify this category")]
			public string name;

			// Token: 0x04000E0F RID: 3599
			public DirectorCard[] cards;

			// Token: 0x04000E10 RID: 3600
			public float selectionWeight;
		}
	}
}
