using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005AA RID: 1450
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	public class ArtifactToggleController : MonoBehaviour
	{
		// Token: 0x0600208A RID: 8330 RVA: 0x00017B4E File Offset: 0x00015D4E
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
		}

		// Token: 0x0600208B RID: 8331 RVA: 0x0009E36C File Offset: 0x0009C56C
		private void Start()
		{
			ArtifactDef artifactDef = ArtifactCatalog.GetArtifactDef(this.artifactIndex);
			if (artifactDef != null)
			{
				this.image.sprite = artifactDef.smallIconDeselectedSprite;
				this.image.SetNativeSize();
			}
		}

		// Token: 0x0600208C RID: 8332 RVA: 0x00017B5C File Offset: 0x00015D5C
		private void LateUpdate()
		{
			ArtifactCatalog.GetArtifactDef(this.artifactIndex);
		}

		// Token: 0x0600208D RID: 8333 RVA: 0x00017B6A File Offset: 0x00015D6A
		public void ToggleArtifact()
		{
			if (NetworkServer.active)
			{
				PreGameController.instance;
			}
		}

		// Token: 0x0400230D RID: 8973
		public ArtifactIndex artifactIndex;

		// Token: 0x0400230E RID: 8974
		private Image image;
	}
}
