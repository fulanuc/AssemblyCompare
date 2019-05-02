using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005FB RID: 1531
	[RequireComponent(typeof(RectTransform))]
	public class ItemIcon : MonoBehaviour
	{
		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06002286 RID: 8838 RVA: 0x0001929A File Offset: 0x0001749A
		// (set) Token: 0x06002287 RID: 8839 RVA: 0x000192A2 File Offset: 0x000174A2
		public RectTransform rectTransform { get; private set; }

		// Token: 0x06002288 RID: 8840 RVA: 0x000192AB File Offset: 0x000174AB
		private void Awake()
		{
			this.CacheRectTransform();
		}

		// Token: 0x06002289 RID: 8841 RVA: 0x000192B3 File Offset: 0x000174B3
		public void CacheRectTransform()
		{
			if (this.rectTransform == null)
			{
				this.rectTransform = (RectTransform)base.transform;
			}
		}

		// Token: 0x0600228A RID: 8842 RVA: 0x000A5F44 File Offset: 0x000A4144
		public void SetItemIndex(ItemIndex newItemIndex, int newItemCount)
		{
			if (this.itemIndex == newItemIndex && this.itemCount == newItemCount)
			{
				return;
			}
			this.itemIndex = newItemIndex;
			this.itemCount = newItemCount;
			string titleToken = "";
			string bodyToken = "";
			Color color = Color.white;
			Color bodyColor = new Color(0.6f, 0.6f, 0.6f, 1f);
			ItemDef itemDef = ItemCatalog.GetItemDef(this.itemIndex);
			if (itemDef != null)
			{
				this.image.texture = Resources.Load<Texture>(itemDef.pickupIconPath);
				if (this.itemCount > 1)
				{
					this.stackText.enabled = true;
					this.stackText.text = string.Format("x{0}", this.itemCount);
				}
				else
				{
					this.stackText.enabled = false;
				}
				titleToken = itemDef.nameToken;
				bodyToken = itemDef.pickupToken;
				color = ColorCatalog.GetColor(itemDef.darkColorIndex);
			}
			if (this.glowImage)
			{
				this.glowImage.color = new Color(color.r, color.g, color.b, 0.75f);
			}
			if (this.tooltipProvider)
			{
				this.tooltipProvider.titleToken = titleToken;
				this.tooltipProvider.bodyToken = bodyToken;
				this.tooltipProvider.titleColor = color;
				this.tooltipProvider.bodyColor = bodyColor;
			}
		}

		// Token: 0x04002547 RID: 9543
		public RawImage glowImage;

		// Token: 0x04002548 RID: 9544
		public RawImage image;

		// Token: 0x04002549 RID: 9545
		public TextMeshProUGUI stackText;

		// Token: 0x0400254A RID: 9546
		public TooltipProvider tooltipProvider;

		// Token: 0x0400254B RID: 9547
		private ItemIndex itemIndex;

		// Token: 0x0400254C RID: 9548
		private int itemCount;
	}
}
