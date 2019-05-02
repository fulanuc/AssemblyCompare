using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200061E RID: 1566
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(RectTransform))]
	public class PregameDifficultySelection : MonoBehaviour
	{
		// Token: 0x06002338 RID: 9016 RVA: 0x00019AF4 File Offset: 0x00017CF4
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
		}

		// Token: 0x06002339 RID: 9017 RVA: 0x000025F6 File Offset: 0x000007F6
		private void LateUpdate()
		{
		}

		// Token: 0x0600233A RID: 9018 RVA: 0x00019B02 File Offset: 0x00017D02
		public void SetCharacterSelectControllerDifficulty()
		{
			bool active = NetworkServer.active;
		}

		// Token: 0x04002623 RID: 9763
		private Image image;

		// Token: 0x04002624 RID: 9764
		public DifficultyIndex difficulty;

		// Token: 0x04002625 RID: 9765
		public Sprite enabledSprite;

		// Token: 0x04002626 RID: 9766
		public Sprite disabledSprite;
	}
}
