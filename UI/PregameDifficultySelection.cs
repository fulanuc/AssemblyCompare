using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000630 RID: 1584
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(RectTransform))]
	public class PregameDifficultySelection : MonoBehaviour
	{
		// Token: 0x060023C8 RID: 9160 RVA: 0x0001A1C2 File Offset: 0x000183C2
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
		}

		// Token: 0x060023C9 RID: 9161 RVA: 0x000025DA File Offset: 0x000007DA
		private void LateUpdate()
		{
		}

		// Token: 0x060023CA RID: 9162 RVA: 0x0001A1D0 File Offset: 0x000183D0
		public void SetCharacterSelectControllerDifficulty()
		{
			bool active = NetworkServer.active;
		}

		// Token: 0x0400267E RID: 9854
		private Image image;

		// Token: 0x0400267F RID: 9855
		public DifficultyIndex difficulty;

		// Token: 0x04002680 RID: 9856
		public Sprite enabledSprite;

		// Token: 0x04002681 RID: 9857
		public Sprite disabledSprite;
	}
}
