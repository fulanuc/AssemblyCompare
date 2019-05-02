using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200023D RID: 573
	public class DifficultyDef
	{
		// Token: 0x06000AD0 RID: 2768 RVA: 0x00008B65 File Offset: 0x00006D65
		public DifficultyDef(float scalingValue, string nameToken, string iconPath, string descriptionToken, Color color)
		{
			this.scalingValue = scalingValue;
			this.descriptionToken = descriptionToken;
			this.nameToken = nameToken;
			this.iconPath = iconPath;
			this.color = color;
		}

		// Token: 0x06000AD1 RID: 2769 RVA: 0x00008B92 File Offset: 0x00006D92
		public Sprite GetIconSprite()
		{
			if (!this.foundIconSprite)
			{
				this.iconSprite = Resources.Load<Sprite>(this.iconPath);
				this.foundIconSprite = this.iconSprite;
			}
			return this.iconSprite;
		}

		// Token: 0x04000E8E RID: 3726
		public readonly float scalingValue;

		// Token: 0x04000E8F RID: 3727
		public readonly string descriptionToken;

		// Token: 0x04000E90 RID: 3728
		public readonly string nameToken;

		// Token: 0x04000E91 RID: 3729
		public readonly string iconPath;

		// Token: 0x04000E92 RID: 3730
		public readonly Color color;

		// Token: 0x04000E93 RID: 3731
		private Sprite iconSprite;

		// Token: 0x04000E94 RID: 3732
		private bool foundIconSprite;
	}
}
