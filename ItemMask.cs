using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000449 RID: 1097
	[Serializable]
	public struct ItemMask
	{
		// Token: 0x06001871 RID: 6257 RVA: 0x0001247F File Offset: 0x0001067F
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

		// Token: 0x06001872 RID: 6258 RVA: 0x000124B9 File Offset: 0x000106B9
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

		// Token: 0x06001873 RID: 6259 RVA: 0x000124F6 File Offset: 0x000106F6
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

		// Token: 0x06001874 RID: 6260 RVA: 0x0007ED18 File Offset: 0x0007CF18
		public static ItemMask operator &(ItemMask mask1, ItemMask mask2)
		{
			return new ItemMask
			{
				a = (mask1.a & mask2.a),
				b = (mask1.b & mask2.b)
			};
		}

		// Token: 0x06001875 RID: 6261 RVA: 0x0007ED58 File Offset: 0x0007CF58
		static ItemMask()
		{
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				ItemMask.all.AddItem(itemIndex);
			}
		}

		// Token: 0x04001BEC RID: 7148
		[SerializeField]
		public ulong a;

		// Token: 0x04001BED RID: 7149
		[SerializeField]
		public ulong b;

		// Token: 0x04001BEE RID: 7150
		public static readonly ItemMask none;

		// Token: 0x04001BEF RID: 7151
		public static readonly ItemMask all = default(ItemMask);
	}
}
