using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000201 RID: 513
	public class ArtifactDef
	{
		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000A08 RID: 2568 RVA: 0x0000818B File Offset: 0x0000638B
		public Sprite smallIconSelectedSprite
		{
			get
			{
				if (!string.IsNullOrEmpty(this.smallIconSelectedPath))
				{
					return Resources.Load<Sprite>(this.smallIconSelectedPath);
				}
				return null;
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000A09 RID: 2569 RVA: 0x000081A7 File Offset: 0x000063A7
		public Sprite smallIconDeselectedSprite
		{
			get
			{
				if (!string.IsNullOrEmpty(this.smallIconDeselectedPath))
				{
					return Resources.Load<Sprite>(this.smallIconDeselectedPath);
				}
				return null;
			}
		}

		// Token: 0x04000D47 RID: 3399
		public string nameToken;

		// Token: 0x04000D48 RID: 3400
		public string unlockableName = "";

		// Token: 0x04000D49 RID: 3401
		public string smallIconSelectedPath;

		// Token: 0x04000D4A RID: 3402
		public string smallIconDeselectedPath;

		// Token: 0x04000D4B RID: 3403
		public string descriptionToken;
	}
}
