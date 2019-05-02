using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.LogBook
{
	// Token: 0x0200067A RID: 1658
	public class LogBookPage : MonoBehaviour
	{
		// Token: 0x060024FE RID: 9470 RVA: 0x000AF8C0 File Offset: 0x000ADAC0
		public void SetEntry(UserProfile userProfile, Entry entry)
		{
			PageBuilder pageBuilder = this.pageBuilder;
			if (pageBuilder != null)
			{
				pageBuilder.Destroy();
			}
			this.pageBuilder = new PageBuilder();
			this.pageBuilder.container = this.contentContainer;
			this.pageBuilder.entry = entry;
			this.pageBuilder.userProfile = userProfile;
			Action<PageBuilder> addEntries = entry.addEntries;
			if (addEntries != null)
			{
				addEntries(this.pageBuilder);
			}
			this.iconImage.texture = entry.iconTexture;
			this.titleText.text = Language.GetString(entry.nameToken);
			this.categoryText.text = Language.GetString(entry.categoryTypeToken);
			this.modelPanel.modelPrefab = entry.modelPrefab;
		}

		// Token: 0x04002819 RID: 10265
		private int currentEntryIndex;

		// Token: 0x0400281A RID: 10266
		public RawImage iconImage;

		// Token: 0x0400281B RID: 10267
		public ModelPanel modelPanel;

		// Token: 0x0400281C RID: 10268
		public TextMeshProUGUI titleText;

		// Token: 0x0400281D RID: 10269
		public TextMeshProUGUI categoryText;

		// Token: 0x0400281E RID: 10270
		public TextMeshProUGUI pageNumberText;

		// Token: 0x0400281F RID: 10271
		public RectTransform contentContainer;

		// Token: 0x04002820 RID: 10272
		private PageBuilder pageBuilder;
	}
}
