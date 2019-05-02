using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005E9 RID: 1513
	[RequireComponent(typeof(RectTransform))]
	public class ItemIcon : MonoBehaviour
	{
		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x060021F5 RID: 8693 RVA: 0x00018BA0 File Offset: 0x00016DA0
		// (set) Token: 0x060021F6 RID: 8694 RVA: 0x00018BA8 File Offset: 0x00016DA8
		public RectTransform rectTransform { get; private set; }

		// Token: 0x060021F7 RID: 8695 RVA: 0x00018BB1 File Offset: 0x00016DB1
		private void Awake()
		{
			this.CacheRectTransform();
		}

		// Token: 0x060021F8 RID: 8696 RVA: 0x00018BB9 File Offset: 0x00016DB9
		public void CacheRectTransform()
		{
			if (this.rectTransform == null)
			{
				this.rectTransform = (RectTransform)base.transform;
			}
		}

		// Token: 0x060021F9 RID: 8697 RVA: 0x000A4990 File Offset: 0x000A2B90
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

		// Token: 0x040024F2 RID: 9458
		public RawImage glowImage;

		// Token: 0x040024F3 RID: 9459
		public RawImage image;

		// Token: 0x040024F4 RID: 9460
		public TextMeshProUGUI stackText;

		// Token: 0x040024F5 RID: 9461
		public TooltipProvider tooltipProvider;

		// Token: 0x040024F6 RID: 9462
		private ItemIndex itemIndex;

		// Token: 0x040024F7 RID: 9463
		private int itemCount;
	}
}
