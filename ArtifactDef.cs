using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000201 RID: 513
	public class ArtifactDef
	{
		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000A05 RID: 2565 RVA: 0x0000817C File Offset: 0x0000637C
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
		// (get) Token: 0x06000A06 RID: 2566 RVA: 0x00008198 File Offset: 0x00006398
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

		// Token: 0x04000D43 RID: 3395
		public string nameToken;

		// Token: 0x04000D44 RID: 3396
		public string unlockableName = "";

		// Token: 0x04000D45 RID: 3397
		public string smallIconSelectedPath;

		// Token: 0x04000D46 RID: 3398
		public string smallIconDeselectedPath;

		// Token: 0x04000D47 RID: 3399
		public string descriptionToken;
	}
}
