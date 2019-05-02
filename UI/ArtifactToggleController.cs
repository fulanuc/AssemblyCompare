using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005BC RID: 1468
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	public class ArtifactToggleController : MonoBehaviour
	{
		// Token: 0x0600211B RID: 8475 RVA: 0x00018248 File Offset: 0x00016448
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
		}

		// Token: 0x0600211C RID: 8476 RVA: 0x0009F940 File Offset: 0x0009DB40
		private void Start()
		{
			ArtifactDef artifactDef = ArtifactCatalog.GetArtifactDef(this.artifactIndex);
			if (artifactDef != null)
			{
				this.image.sprite = artifactDef.smallIconDeselectedSprite;
				this.image.SetNativeSize();
			}
		}

		// Token: 0x0600211D RID: 8477 RVA: 0x00018256 File Offset: 0x00016456
		private void LateUpdate()
		{
			ArtifactCatalog.GetArtifactDef(this.artifactIndex);
		}

		// Token: 0x0600211E RID: 8478 RVA: 0x00018264 File Offset: 0x00016464
		public void ToggleArtifact()
		{
			if (NetworkServer.active)
			{
				PreGameController.instance;
			}
		}

		// Token: 0x04002361 RID: 9057
		public ArtifactIndex artifactIndex;

		// Token: 0x04002362 RID: 9058
		private Image image;
	}
}
