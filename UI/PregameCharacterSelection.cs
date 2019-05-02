using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200061D RID: 1565
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(RectTransform))]
	public class PregameCharacterSelection : MonoBehaviour
	{
		// Token: 0x06002335 RID: 9013 RVA: 0x00019AE6 File Offset: 0x00017CE6
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
		}

		// Token: 0x06002336 RID: 9014 RVA: 0x000A8E88 File Offset: 0x000A7088
		private void LateUpdate()
		{
			this.image.sprite = this.disabledSprite;
			ReadOnlyCollection<NetworkUser> readOnlyLocalPlayersList = NetworkUser.readOnlyLocalPlayersList;
			for (int i = 0; i < readOnlyLocalPlayersList.Count; i++)
			{
				if (readOnlyLocalPlayersList[i].bodyIndexPreference == BodyCatalog.FindBodyIndex(this.characterBodyPrefab))
				{
					this.image.sprite = this.enabledSprite;
					return;
				}
			}
		}

		// Token: 0x0400261F RID: 9759
		private Image image;

		// Token: 0x04002620 RID: 9760
		public GameObject characterBodyPrefab;

		// Token: 0x04002621 RID: 9761
		public Sprite enabledSprite;

		// Token: 0x04002622 RID: 9762
		public Sprite disabledSprite;
	}
}
