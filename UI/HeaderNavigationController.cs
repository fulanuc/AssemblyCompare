using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005ED RID: 1517
	public class HeaderNavigationController : MonoBehaviour
	{
		// Token: 0x06002231 RID: 8753 RVA: 0x00018E39 File Offset: 0x00017039
		private void Start()
		{
			this.RebuildHeaders();
		}

		// Token: 0x06002232 RID: 8754 RVA: 0x000A42E0 File Offset: 0x000A24E0
		private void LateUpdate()
		{
			for (int i = 0; i < this.headers.Length; i++)
			{
				HeaderNavigationController.Header header = this.headers[i];
				if (i == this.currentHeaderIndex)
				{
					header.tmpHeaderText.color = Color.white;
				}
				else
				{
					header.tmpHeaderText.color = Color.gray;
				}
			}
		}

		// Token: 0x06002233 RID: 8755 RVA: 0x000A4338 File Offset: 0x000A2538
		public void ChooseHeader(string headerName)
		{
			for (int i = 0; i < this.headers.Length; i++)
			{
				if (this.headers[i].headerName == headerName)
				{
					this.currentHeaderIndex = i;
					this.RebuildHeaders();
					return;
				}
			}
		}

		// Token: 0x06002234 RID: 8756 RVA: 0x000A4380 File Offset: 0x000A2580
		public void ChooseHeaderByButton(MPButton mpButton)
		{
			for (int i = 0; i < this.headers.Length; i++)
			{
				if (this.headers[i].headerButton == mpButton)
				{
					this.currentHeaderIndex = i;
					this.RebuildHeaders();
					return;
				}
			}
		}

		// Token: 0x06002235 RID: 8757 RVA: 0x000A43C8 File Offset: 0x000A25C8
		private void RebuildHeaders()
		{
			for (int i = 0; i < this.headers.Length; i++)
			{
				HeaderNavigationController.Header header = this.headers[i];
				if (i == this.currentHeaderIndex)
				{
					if (header.headerRoot)
					{
						header.headerRoot.SetActive(true);
					}
					if (header.headerButton && this.buttonSelectionRoot)
					{
						this.buttonSelectionRoot.transform.parent = header.headerButton.transform;
						this.buttonSelectionRoot.SetActive(false);
						this.buttonSelectionRoot.SetActive(true);
						RectTransform component = this.buttonSelectionRoot.GetComponent<RectTransform>();
						component.offsetMin = Vector2.zero;
						component.offsetMax = Vector2.zero;
						component.localScale = Vector3.one;
					}
				}
				else if (header.headerRoot)
				{
					header.headerRoot.SetActive(false);
				}
			}
		}

		// Token: 0x06002236 RID: 8758 RVA: 0x00018E41 File Offset: 0x00017041
		private HeaderNavigationController.Header GetCurrentHeader()
		{
			return this.headers[this.currentHeaderIndex];
		}

		// Token: 0x040024B6 RID: 9398
		public HeaderNavigationController.Header[] headers;

		// Token: 0x040024B7 RID: 9399
		public GameObject buttonSelectionRoot;

		// Token: 0x040024B8 RID: 9400
		public int currentHeaderIndex;

		// Token: 0x020005EE RID: 1518
		[Serializable]
		public struct Header
		{
			// Token: 0x040024B9 RID: 9401
			public MPButton headerButton;

			// Token: 0x040024BA RID: 9402
			public string headerName;

			// Token: 0x040024BB RID: 9403
			public TextMeshProUGUI tmpHeaderText;

			// Token: 0x040024BC RID: 9404
			public GameObject headerRoot;
		}
	}
}
