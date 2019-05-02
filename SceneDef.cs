using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200022F RID: 559
	[CreateAssetMenu]
	public class SceneDef : ScriptableObject
	{
		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000AB5 RID: 2741 RVA: 0x00008A41 File Offset: 0x00006C41
		public SceneField sceneField
		{
			get
			{
				return new SceneField(this.sceneName);
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000AB6 RID: 2742 RVA: 0x00008A4E File Offset: 0x00006C4E
		public string sceneName
		{
			get
			{
				if (this.cachedSceneName == null)
				{
					this.cachedSceneName = base.name;
				}
				return this.cachedSceneName;
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000AB7 RID: 2743 RVA: 0x00008A6A File Offset: 0x00006C6A
		[Obsolete("SceneDef.name should not be used due to unnecessary managed allocations. Use sceneName instead.")]
		public new string name
		{
			get
			{
				return base.name;
			}
		}

		// Token: 0x04000E3E RID: 3646
		public string nameToken;

		// Token: 0x04000E3F RID: 3647
		public string subtitleToken;

		// Token: 0x04000E40 RID: 3648
		public string loreToken;

		// Token: 0x04000E41 RID: 3649
		public int stageOrder;

		// Token: 0x04000E42 RID: 3650
		public Texture previewTexture;

		// Token: 0x04000E43 RID: 3651
		public GameObject dioramaPrefab;

		// Token: 0x04000E44 RID: 3652
		public SceneType sceneType;

		// Token: 0x04000E45 RID: 3653
		public string songName;

		// Token: 0x04000E46 RID: 3654
		public string bossSongName;

		// Token: 0x04000E47 RID: 3655
		private string cachedSceneName;
	}
}
