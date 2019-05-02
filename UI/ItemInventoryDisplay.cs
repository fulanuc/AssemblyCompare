using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005EA RID: 1514
	[RequireComponent(typeof(RectTransform))]
	public class ItemInventoryDisplay : UIBehaviour
	{
		// Token: 0x060021FB RID: 8699 RVA: 0x000A4AEC File Offset: 0x000A2CEC
		public void SetSubscribedInventory([CanBeNull] Inventory newInventory)
		{
			if (this.inventory == newInventory && this.inventory == this.inventoryWasValid)
			{
				return;
			}
			if (this.inventory != null)
			{
				this.inventory.onInventoryChanged -= this.OnInventoryChanged;
				this.inventory = null;
			}
			this.inventory = newInventory;
			this.inventoryWasValid = this.inventory;
			if (this.inventory)
			{
				this.inventory.onInventoryChanged += this.OnInventoryChanged;
			}
			this.OnInventoryChanged();
		}

		// Token: 0x060021FC RID: 8700 RVA: 0x000A4B80 File Offset: 0x000A2D80
		private void OnInventoryChanged()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.inventory)
			{
				this.inventory.WriteItemStacks(this.itemStacks);
				this.inventory.itemAcquisitionOrder.CopyTo(this.itemOrder);
				this.itemOrderCount = this.inventory.itemAcquisitionOrder.Count;
			}
			else
			{
				Array.Clear(this.itemStacks, 0, this.itemStacks.Length);
				this.itemOrderCount = 0;
			}
			this.RequestUpdateDisplay();
		}

		// Token: 0x060021FD RID: 8701 RVA: 0x000A4C04 File Offset: 0x000A2E04
		private static bool ItemIsVisible(ItemIndex itemIndex)
		{
			ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
			return itemDef != null && !itemDef.hidden;
		}

		// Token: 0x060021FE RID: 8702 RVA: 0x000A4C28 File Offset: 0x000A2E28
		private void AllocateIcons(int desiredItemCount)
		{
			if (desiredItemCount != this.itemIcons.Count)
			{
				while (this.itemIcons.Count > desiredItemCount)
				{
					UnityEngine.Object.Destroy(this.itemIcons[this.itemIcons.Count - 1].gameObject);
					this.itemIcons.RemoveAt(this.itemIcons.Count - 1);
				}
				while (this.itemIcons.Count < desiredItemCount)
				{
					ItemIcon component = UnityEngine.Object.Instantiate<GameObject>(this.itemIconPrefab, this.rectTransform).GetComponent<ItemIcon>();
					this.itemIcons.Add(component);
					this.LayoutIndividualIcon(this.itemIcons.Count - 1);
				}
			}
			this.OnIconCountChanged();
		}

		// Token: 0x060021FF RID: 8703 RVA: 0x000A4CDC File Offset: 0x000A2EDC
		private float CalculateIconScale(int iconCount)
		{
			int num = (int)this.rectTransform.rect.width;
			int num2 = (int)this.maxHeight;
			int num3 = (int)this.iconWidth;
			int num4 = num3;
			int num5 = num3 / 8;
			int num6 = Math.Max(num / num4, 1);
			int num7 = HGMath.IntDivCeil(iconCount, num6);
			while (num4 * num7 > num2)
			{
				num6++;
				num4 = Math.Min(num / num6, num3);
				num7 = HGMath.IntDivCeil(iconCount, num6);
				if (num4 <= num5)
				{
					num4 = num5;
					break;
				}
			}
			return (float)num4 / (float)num3;
		}

		// Token: 0x06002200 RID: 8704 RVA: 0x000A4D60 File Offset: 0x000A2F60
		private void OnIconCountChanged()
		{
			float num = this.CalculateIconScale(this.itemIcons.Count);
			if (num != this.currentIconScale)
			{
				this.currentIconScale = num;
				this.OnIconScaleChanged();
			}
		}

		// Token: 0x06002201 RID: 8705 RVA: 0x00018BD4 File Offset: 0x00016DD4
		private void OnIconScaleChanged()
		{
			this.LayoutAllIcons();
		}

		// Token: 0x06002202 RID: 8706 RVA: 0x000A4D98 File Offset: 0x000A2F98
		private void CalculateLayoutValues(out ItemInventoryDisplay.LayoutValues v)
		{
			Rect rect = this.rectTransform.rect;
			v.width = rect.width;
			v.iconSize = this.iconWidth * this.currentIconScale;
			v.iconsPerRow = Math.Max((int)v.width / (int)v.iconSize, 1);
			v.rowWidth = (float)v.iconsPerRow * v.iconSize;
			float num = (v.width - v.rowWidth) * 0.5f;
			v.rowCount = HGMath.IntDivCeil(this.itemIcons.Count, v.iconsPerRow);
			v.iconLocalScale = new Vector3(this.currentIconScale, this.currentIconScale, 1f);
			v.topLeftCorner = new Vector3(rect.xMin + num, rect.yMax);
		}

		// Token: 0x06002203 RID: 8707 RVA: 0x000A4E68 File Offset: 0x000A3068
		private void LayoutAllIcons()
		{
			ItemInventoryDisplay.LayoutValues layoutValues;
			this.CalculateLayoutValues(out layoutValues);
			int num = this.itemIcons.Count - (layoutValues.rowCount - 1) * layoutValues.iconsPerRow;
			int i = 0;
			int num2 = 0;
			while (i < layoutValues.rowCount)
			{
				Vector3 topLeftCorner = layoutValues.topLeftCorner;
				topLeftCorner.y += (float)i * -layoutValues.iconSize;
				int num3 = layoutValues.iconsPerRow;
				if (i == layoutValues.rowCount - 1)
				{
					num3 = num;
				}
				int j = 0;
				while (j < num3)
				{
					RectTransform rectTransform = this.itemIcons[num2].rectTransform;
					rectTransform.localScale = layoutValues.iconLocalScale;
					rectTransform.localPosition = topLeftCorner;
					topLeftCorner.x += layoutValues.iconSize;
					j++;
					num2++;
				}
				i++;
			}
		}

		// Token: 0x06002204 RID: 8708 RVA: 0x000A4F30 File Offset: 0x000A3130
		private void LayoutIndividualIcon(int i)
		{
			ItemInventoryDisplay.LayoutValues layoutValues;
			this.CalculateLayoutValues(out layoutValues);
			int num = i / layoutValues.iconsPerRow;
			int num2 = i - num * layoutValues.iconsPerRow;
			Vector3 topLeftCorner = layoutValues.topLeftCorner;
			topLeftCorner.x += (float)num2 * layoutValues.iconSize;
			topLeftCorner.y += (float)num * -layoutValues.iconSize;
			RectTransform rectTransform = this.itemIcons[i].rectTransform;
			rectTransform.localPosition = topLeftCorner;
			rectTransform.localScale = layoutValues.iconLocalScale;
		}

		// Token: 0x06002205 RID: 8709 RVA: 0x00018BDC File Offset: 0x00016DDC
		protected override void Awake()
		{
			base.Awake();
			this.rectTransform = (RectTransform)base.transform;
			this.itemStacks = ItemCatalog.RequestItemStackArray();
			this.itemOrder = ItemCatalog.RequestItemOrderBuffer();
		}

		// Token: 0x06002206 RID: 8710 RVA: 0x00018C0B File Offset: 0x00016E0B
		protected override void OnDestroy()
		{
			this.SetSubscribedInventory(null);
			base.OnDestroy();
		}

		// Token: 0x06002207 RID: 8711 RVA: 0x00018C1A File Offset: 0x00016E1A
		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.inventory)
			{
				this.OnInventoryChanged();
			}
			this.RequestUpdateDisplay();
			this.LayoutAllIcons();
		}

		// Token: 0x06002208 RID: 8712 RVA: 0x00018C41 File Offset: 0x00016E41
		private void RequestUpdateDisplay()
		{
			if (!this.updateRequestPending)
			{
				this.updateRequestPending = true;
				RoR2Application.onNextUpdate += this.UpdateDisplay;
			}
		}

		// Token: 0x06002209 RID: 8713 RVA: 0x000A4FAC File Offset: 0x000A31AC
		public void UpdateDisplay()
		{
			this.updateRequestPending = false;
			if (!this || !base.isActiveAndEnabled)
			{
				return;
			}
			ItemIndex[] array = ItemCatalog.RequestItemOrderBuffer();
			int num = 0;
			for (int i = 0; i < this.itemOrderCount; i++)
			{
				if (ItemInventoryDisplay.ItemIsVisible(this.itemOrder[i]))
				{
					array[num++] = this.itemOrder[i];
				}
			}
			this.AllocateIcons(num);
			for (int j = 0; j < num; j++)
			{
				ItemIndex itemIndex = array[j];
				this.itemIcons[j].SetItemIndex(itemIndex, this.itemStacks[(int)itemIndex]);
			}
			ItemCatalog.ReturnItemOrderBuffer(array);
		}

		// Token: 0x0600220A RID: 8714 RVA: 0x000A5044 File Offset: 0x000A3244
		public void SetItems(List<ItemIndex> newItemOrder, uint[] newItemStacks)
		{
			this.itemOrderCount = newItemOrder.Count;
			for (int i = 0; i < this.itemOrderCount; i++)
			{
				this.itemOrder[i] = newItemOrder[i];
			}
			Array.Copy(newItemStacks, this.itemStacks, this.itemStacks.Length);
			this.RequestUpdateDisplay();
		}

		// Token: 0x0600220B RID: 8715 RVA: 0x00018C63 File Offset: 0x00016E63
		public void SetItems(ItemIndex[] newItemOrder, int newItemOrderCount, int[] newItemStacks)
		{
			this.itemOrderCount = newItemOrderCount;
			Array.Copy(newItemOrder, this.itemOrder, this.itemOrderCount);
			Array.Copy(newItemStacks, this.itemStacks, this.itemStacks.Length);
			this.RequestUpdateDisplay();
		}

		// Token: 0x0600220C RID: 8716 RVA: 0x00018C98 File Offset: 0x00016E98
		public void ResetItems()
		{
			this.itemOrderCount = 0;
			Array.Clear(this.itemStacks, 0, this.itemStacks.Length);
			this.RequestUpdateDisplay();
		}

		// Token: 0x0600220D RID: 8717 RVA: 0x000A5098 File Offset: 0x000A3298
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			if (this.rectTransform)
			{
				float width = this.rectTransform.rect.width;
				if (width != this.previousWidth)
				{
					this.previousWidth = width;
					this.LayoutAllIcons();
				}
			}
		}

		// Token: 0x040024F8 RID: 9464
		private RectTransform rectTransform;

		// Token: 0x040024F9 RID: 9465
		public GameObject itemIconPrefab;

		// Token: 0x040024FA RID: 9466
		public float iconWidth = 64f;

		// Token: 0x040024FB RID: 9467
		public float maxHeight = 128f;

		// Token: 0x040024FC RID: 9468
		[SerializeField]
		[HideInInspector]
		private List<ItemIcon> itemIcons;

		// Token: 0x040024FD RID: 9469
		private ItemIndex[] itemOrder;

		// Token: 0x040024FE RID: 9470
		private int itemOrderCount;

		// Token: 0x040024FF RID: 9471
		private int[] itemStacks;

		// Token: 0x04002500 RID: 9472
		private float currentIconScale = 1f;

		// Token: 0x04002501 RID: 9473
		private float previousWidth;

		// Token: 0x04002502 RID: 9474
		private bool updateRequestPending;

		// Token: 0x04002503 RID: 9475
		private Inventory inventory;

		// Token: 0x04002504 RID: 9476
		private bool inventoryWasValid;

		// Token: 0x020005EB RID: 1515
		private struct LayoutValues
		{
			// Token: 0x04002505 RID: 9477
			public float width;

			// Token: 0x04002506 RID: 9478
			public float iconSize;

			// Token: 0x04002507 RID: 9479
			public int iconsPerRow;

			// Token: 0x04002508 RID: 9480
			public float rowWidth;

			// Token: 0x04002509 RID: 9481
			public int rowCount;

			// Token: 0x0400250A RID: 9482
			public Vector3 iconLocalScale;

			// Token: 0x0400250B RID: 9483
			public Vector3 topLeftCorner;
		}
	}
}
