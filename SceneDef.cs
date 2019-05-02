using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200022F RID: 559
	[CreateAssetMenu]
	public class SceneDef : ScriptableObject
	{
		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000AB9 RID: 2745 RVA: 0x00008A66 File Offset: 0x00006C66
		public SceneField sceneField
		{
			get
			{
				return new SceneField(this.sceneName);
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000ABA RID: 2746 RVA: 0x00008A73 File Offset: 0x00006C73
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

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000ABB RID: 2747 RVA: 0x00008A8F File Offset: 0x00006C8F
		[Obsolete("SceneDef.name should not be used due to unnecessary managed allocations. Use sceneName instead.")]
		public new string name
		{
			get
			{
				return base.name;
			}
		}

		// Token: 0x04000E42 RID: 3650
		public string nameToken;

		// Token: 0x04000E43 RID: 3651
		public string subtitleToken;

		// Token: 0x04000E44 RID: 3652
		public string loreToken;

		// Token: 0x04000E45 RID: 3653
		public int stageOrder;

		// Token: 0x04000E46 RID: 3654
		public Texture previewTexture;

		// Token: 0x04000E47 RID: 3655
		public GameObject dioramaPrefab;

		// Token: 0x04000E48 RID: 3656
		public SceneType sceneType;

		// Token: 0x04000E49 RID: 3657
		public string songName;

		// Token: 0x04000E4A RID: 3658
		public string bossSongName;

		// Token: 0x04000E4B RID: 3659
		public bool isOfflineScene;

		// Token: 0x04000E4C RID: 3660
		private string cachedSceneName;
	}
}
