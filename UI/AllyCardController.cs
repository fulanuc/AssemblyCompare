using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B9 RID: 1465
	public class AllyCardController : MonoBehaviour
	{
		// Token: 0x06002110 RID: 8464 RVA: 0x000181F1 File Offset: 0x000163F1
		private void LateUpdate()
		{
			this.UpdateInfo();
		}

		// Token: 0x06002111 RID: 8465 RVA: 0x0009F664 File Offset: 0x0009D864
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

		// Token: 0x04002351 RID: 9041
		public HealthBar healthBar;

		// Token: 0x04002352 RID: 9042
		public TextMeshProUGUI nameLabel;

		// Token: 0x04002353 RID: 9043
		public RawImage portraitIconImage;

		// Token: 0x04002354 RID: 9044
		[HideInInspector]
		public GameObject sourceGameObject;
	}
}
