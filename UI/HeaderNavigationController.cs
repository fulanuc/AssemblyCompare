using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005DB RID: 1499
	public class HeaderNavigationController : MonoBehaviour
	{
		// Token: 0x060021A0 RID: 8608 RVA: 0x0001873F File Offset: 0x0001693F
		private void Start()
		{
			this.RebuildHeaders();
		}

		// Token: 0x060021A1 RID: 8609 RVA: 0x000A2D0C File Offset: 0x000A0F0C
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

		// Token: 0x060021A2 RID: 8610 RVA: 0x000A2D64 File Offset: 0x000A0F64
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

		// Token: 0x060021A3 RID: 8611 RVA: 0x000A2DAC File Offset: 0x000A0FAC
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

		// Token: 0x060021A4 RID: 8612 RVA: 0x000A2DF4 File Offset: 0x000A0FF4
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

		// Token: 0x060021A5 RID: 8613 RVA: 0x00018747 File Offset: 0x00016947
		private HeaderNavigationController.Header GetCurrentHeader()
		{
			return this.headers[this.currentHeaderIndex];
		}

		// Token: 0x04002462 RID: 9314
		public HeaderNavigationController.Header[] headers;

		// Token: 0x04002463 RID: 9315
		public GameObject buttonSelectionRoot;

		// Token: 0x04002464 RID: 9316
		public int currentHeaderIndex;

		// Token: 0x020005DC RID: 1500
		[Serializable]
		public struct Header
		{
			// Token: 0x04002465 RID: 9317
			public MPButton headerButton;

			// Token: 0x04002466 RID: 9318
			public string headerName;

			// Token: 0x04002467 RID: 9319
			public TextMeshProUGUI tmpHeaderText;

			// Token: 0x04002468 RID: 9320
			public GameObject headerRoot;
		}
	}
}
