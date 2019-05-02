using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200062F RID: 1583
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(RectTransform))]
	public class PregameCharacterSelection : MonoBehaviour
	{
		// Token: 0x060023C5 RID: 9157 RVA: 0x0001A1B4 File Offset: 0x000183B4
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
		}

		// Token: 0x060023C6 RID: 9158 RVA: 0x000AA504 File Offset: 0x000A8704
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

		// Token: 0x0400267A RID: 9850
		private Image image;

		// Token: 0x0400267B RID: 9851
		public GameObject characterBodyPrefab;

		// Token: 0x0400267C RID: 9852
		public Sprite enabledSprite;

		// Token: 0x0400267D RID: 9853
		public Sprite disabledSprite;
	}
}
