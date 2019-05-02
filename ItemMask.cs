using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000453 RID: 1107
	[Serializable]
	public struct ItemMask
	{
		// Token: 0x060018C6 RID: 6342 RVA: 0x00012956 File Offset: 0x00010B56
		public bool HasItem(ItemIndex itemIndex)
		{
			if (itemIndex < ItemIndex.Syringe || itemIndex >= ItemIndex.Count)
			{
				return false;
			}
			if (itemIndex < ItemIndex.FireRing)
			{
				return (this.a & 1UL << (int)itemIndex) > 0UL;
			}
			return (this.b & 1UL << itemIndex - ItemIndex.FireRing) > 0UL;
		}

		// Token: 0x060018C7 RID: 6343 RVA: 0x00012990 File Offset: 0x00010B90
		public void AddItem(ItemIndex itemIndex)
		{
			if (itemIndex < ItemIndex.Syringe || itemIndex >= ItemIndex.Count)
			{
				return;
			}
			if (itemIndex < ItemIndex.FireRing)
			{
				this.a |= 1UL << (int)itemIndex;
				return;
			}
			this.b |= 1UL << itemIndex - ItemIndex.FireRing;
		}

		// Token: 0x060018C8 RID: 6344 RVA: 0x000129CD File Offset: 0x00010BCD
		public void RemoveItem(ItemIndex itemIndex)
		{
			if (itemIndex < ItemIndex.Syringe || itemIndex >= ItemIndex.Count)
			{
				return;
			}
			if (itemIndex < ItemIndex.FireRing)
			{
				this.a &= ~(1UL << (int)itemIndex);
				return;
			}
			this.b &= ~(1UL << itemIndex - ItemIndex.FireRing);
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x0007F4F8 File Offset: 0x0007D6F8
		public static ItemMask operator &(ItemMask mask1, ItemMask mask2)
		{
			return new ItemMask
			{
				a = (mask1.a & mask2.a),
				b = (mask1.b & mask2.b)
			};
		}

		// Token: 0x060018CA RID: 6346 RVA: 0x0007F538 File Offset: 0x0007D738
		static ItemMask()
		{
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				ItemMask.all.AddItem(itemIndex);
			}
		}

		// Token: 0x04001C1E RID: 7198
		[SerializeField]
		public ulong a;

		// Token: 0x04001C1F RID: 7199
		[SerializeField]
		public ulong b;

		// Token: 0x04001C20 RID: 7200
		public static readonly ItemMask none;

		// Token: 0x04001C21 RID: 7201
		public static readonly ItemMask all = default(ItemMask);
	}
}
