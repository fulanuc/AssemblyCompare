using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005A7 RID: 1447
	public class AllyCardController : MonoBehaviour
	{
		// Token: 0x0600207F RID: 8319 RVA: 0x00017AF7 File Offset: 0x00015CF7
		private void LateUpdate()
		{
			this.UpdateInfo();
		}

		// Token: 0x06002080 RID: 8320 RVA: 0x0009E090 File Offset: 0x0009C290
		private void UpdateInfo()
		{
			HealthComponent source = null;
			string text = "";
			Texture texture = null;
			if (this.sourceGameObject)
			{
				source = this.sourceGameObject.GetComponent<HealthComponent>();
				text = Util.GetBestBodyName(this.sourceGameObject);
				CharacterBody component = this.sourceGameObject.GetComponent<CharacterBody>();
				if (component)
				{
					texture = component.portraitIcon;
				}
			}
			this.healthBar.source = source;
			this.nameLabel.text = text;
			this.portraitIconImage.texture = texture;
		}

		// Token: 0x040022FD RID: 8957
		public HealthBar healthBar;

		// Token: 0x040022FE RID: 8958
		public TextMeshProUGUI nameLabel;

		// Token: 0x040022FF RID: 8959
		public RawImage portraitIconImage;

		// Token: 0x04002300 RID: 8960
		[HideInInspector]
		public GameObject sourceGameObject;
	}
}
