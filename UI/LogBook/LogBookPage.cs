using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.LogBook
{
	// Token: 0x0200068C RID: 1676
	public class LogBookPage : MonoBehaviour
	{
		// Token: 0x06002595 RID: 9621 RVA: 0x000B0FB0 File Offset: 0x000AF1B0
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

		// Token: 0x04002875 RID: 10357
		private int currentEntryIndex;

		// Token: 0x04002876 RID: 10358
		public RawImage iconImage;

		// Token: 0x04002877 RID: 10359
		public ModelPanel modelPanel;

		// Token: 0x04002878 RID: 10360
		public TextMeshProUGUI titleText;

		// Token: 0x04002879 RID: 10361
		public TextMeshProUGUI categoryText;

		// Token: 0x0400287A RID: 10362
		public TextMeshProUGUI pageNumberText;

		// Token: 0x0400287B RID: 10363
		public RectTransform contentContainer;

		// Token: 0x0400287C RID: 10364
		private PageBuilder pageBuilder;
	}
}
