using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005FC RID: 1532
	[RequireComponent(typeof(RectTransform))]
	public class ItemInventoryDisplay : UIBehaviour
	{
		// Token: 0x0600228C RID: 8844 RVA: 0x000A60A0 File Offset: 0x000A42A0
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

		// Token: 0x0600228D RID: 8845 RVA: 0x000A6134 File Offset: 0x000A4334
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

		// Token: 0x0600228E RID: 8846 RVA: 0x000A61B8 File Offset: 0x000A43B8
		private static bool ItemIsVisible(ItemIndex itemIndex)
		{
			ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
			return itemDef != null && !itemDef.hidden;
		}

		// Token: 0x0600228F RID: 8847 RVA: 0x000A61DC File Offset: 0x000A43DC
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

		// Token: 0x06002290 RID: 8848 RVA: 0x000A6290 File Offset: 0x000A4490
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

		// Token: 0x06002291 RID: 8849 RVA: 0x000A6314 File Offset: 0x000A4514
		private void OnIconCountChanged()
		{
			float num = this.CalculateIconScale(this.itemIcons.Count);
			if (num != this.currentIconScale)
			{
				this.currentIconScale = num;
				this.OnIconScaleChanged();
			}
		}

		// Token: 0x06002292 RID: 8850 RVA: 0x000192CE File Offset: 0x000174CE
		private void OnIconScaleChanged()
		{
			this.LayoutAllIcons();
		}

		// Token: 0x06002293 RID: 8851 RVA: 0x000A634C File Offset: 0x000A454C
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

		// Token: 0x06002294 RID: 8852 RVA: 0x000A641C File Offset: 0x000A461C
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

		// Token: 0x06002295 RID: 8853 RVA: 0x000A64E4 File Offset: 0x000A46E4
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

		// Token: 0x06002296 RID: 8854 RVA: 0x000192D6 File Offset: 0x000174D6
		protected override void Awake()
		{
			base.Awake();
			this.rectTransform = (RectTransform)base.transform;
			this.itemStacks = ItemCatalog.RequestItemStackArray();
			this.itemOrder = ItemCatalog.RequestItemOrderBuffer();
		}

		// Token: 0x06002297 RID: 8855 RVA: 0x00019305 File Offset: 0x00017505
		protected override void OnDestroy()
		{
			this.SetSubscribedInventory(null);
			base.OnDestroy();
		}

		// Token: 0x06002298 RID: 8856 RVA: 0x00019314 File Offset: 0x00017514
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

		// Token: 0x06002299 RID: 8857 RVA: 0x0001933B File Offset: 0x0001753B
		private void RequestUpdateDisplay()
		{
			if (!this.updateRequestPending)
			{
				this.updateRequestPending = true;
				RoR2Application.onNextUpdate += this.UpdateDisplay;
			}
		}

		// Token: 0x0600229A RID: 8858 RVA: 0x000A6560 File Offset: 0x000A4760
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

		// Token: 0x0600229B RID: 8859 RVA: 0x000A65F8 File Offset: 0x000A47F8
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

		// Token: 0x0600229C RID: 8860 RVA: 0x0001935D File Offset: 0x0001755D
		public void SetItems(ItemIndex[] newItemOrder, int newItemOrderCount, int[] newItemStacks)
		{
			this.itemOrderCount = newItemOrderCount;
			Array.Copy(newItemOrder, this.itemOrder, this.itemOrderCount);
			Array.Copy(newItemStacks, this.itemStacks, this.itemStacks.Length);
			this.RequestUpdateDisplay();
		}

		// Token: 0x0600229D RID: 8861 RVA: 0x00019392 File Offset: 0x00017592
		public void ResetItems()
		{
			this.itemOrderCount = 0;
			Array.Clear(this.itemStacks, 0, this.itemStacks.Length);
			this.RequestUpdateDisplay();
		}

		// Token: 0x0600229E RID: 8862 RVA: 0x000A664C File Offset: 0x000A484C
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

		// Token: 0x0400254D RID: 9549
		private RectTransform rectTransform;

		// Token: 0x0400254E RID: 9550
		public GameObject itemIconPrefab;

		// Token: 0x0400254F RID: 9551
		public float iconWidth = 64f;

		// Token: 0x04002550 RID: 9552
		public float maxHeight = 128f;

		// Token: 0x04002551 RID: 9553
		[HideInInspector]
		[SerializeField]
		private List<ItemIcon> itemIcons;

		// Token: 0x04002552 RID: 9554
		private ItemIndex[] itemOrder;

		// Token: 0x04002553 RID: 9555
		private int itemOrderCount;

		// Token: 0x04002554 RID: 9556
		private int[] itemStacks;

		// Token: 0x04002555 RID: 9557
		private float currentIconScale = 1f;

		// Token: 0x04002556 RID: 9558
		private float previousWidth;

		// Token: 0x04002557 RID: 9559
		private bool updateRequestPending;

		// Token: 0x04002558 RID: 9560
		private Inventory inventory;

		// Token: 0x04002559 RID: 9561
		private bool inventoryWasValid;

		// Token: 0x020005FD RID: 1533
		private struct LayoutValues
		{
			// Token: 0x0400255A RID: 9562
			public float width;

			// Token: 0x0400255B RID: 9563
			public float iconSize;

			// Token: 0x0400255C RID: 9564
			public int iconsPerRow;

			// Token: 0x0400255D RID: 9565
			public float rowWidth;

			// Token: 0x0400255E RID: 9566
			public int rowCount;

			// Token: 0x0400255F RID: 9567
			public Vector3 iconLocalScale;

			// Token: 0x04002560 RID: 9568
			public Vector3 topLeftCorner;
		}
	}
}
