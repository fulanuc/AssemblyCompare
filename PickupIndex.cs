using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000472 RID: 1138
	[Serializable]
	public struct PickupIndex : IEquatable<PickupIndex>
	{
		// Token: 0x1700025E RID: 606
		// (get) Token: 0x06001990 RID: 6544 RVA: 0x00013113 File Offset: 0x00011313
		public bool isValid
		{
			get
			{
				return this.value < 106;
			}
		}

		// Token: 0x06001991 RID: 6545 RVA: 0x0001311F File Offset: 0x0001131F
		private PickupIndex(int value)
		{
			this.value = ((value < 0) ? -1 : value);
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x0001311F File Offset: 0x0001131F
		public PickupIndex(ItemIndex itemIndex)
		{
			this.value = (int)((itemIndex < ItemIndex.Syringe) ? ItemIndex.None : itemIndex);
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x0001312F File Offset: 0x0001132F
		public PickupIndex(EquipmentIndex equipmentIndex)
		{
			this.value = (int)((equipmentIndex < EquipmentIndex.CommandMissile) ? EquipmentIndex.None : (78 + equipmentIndex));
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x00013142 File Offset: 0x00011342
		public GameObject GetHiddenPickupDisplayPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");
		}

		// Token: 0x06001995 RID: 6549 RVA: 0x000839DC File Offset: 0x00081BDC
		public GameObject GetPickupDisplayPrefab()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					return Resources.Load<GameObject>(ItemCatalog.GetItemDef((ItemIndex)this.value).pickupModelPath);
				}
				if (this.value < 105)
				{
					return Resources.Load<GameObject>(EquipmentCatalog.GetEquipmentDef((EquipmentIndex)(this.value - 78)).pickupModelPath);
				}
				if (this.value < 106)
				{
					return Resources.Load<GameObject>("Prefabs/PickupModels/PickupLunarCoin");
				}
			}
			return null;
		}

		// Token: 0x06001996 RID: 6550 RVA: 0x00083A4C File Offset: 0x00081C4C
		public GameObject GetPickupDropletDisplayPrefab()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					ItemDef itemDef = ItemCatalog.GetItemDef((ItemIndex)this.value);
					string path = null;
					switch (itemDef.tier)
					{
					case ItemTier.Tier1:
						path = "Prefabs/ItemPickups/Tier1Orb";
						break;
					case ItemTier.Tier2:
						path = "Prefabs/ItemPickups/Tier2Orb";
						break;
					case ItemTier.Tier3:
						path = "Prefabs/ItemPickups/Tier3Orb";
						break;
					case ItemTier.Lunar:
						path = "Prefabs/ItemPickups/LunarOrb";
						break;
					}
					if (!string.IsNullOrEmpty(path))
					{
						return Resources.Load<GameObject>(path);
					}
					return null;
				}
				else
				{
					if (this.value < 105)
					{
						return Resources.Load<GameObject>("Prefabs/ItemPickups/EquipmentOrb");
					}
					if (this.value < 106)
					{
						return Resources.Load<GameObject>("Prefabs/ItemPickups/LunarOrb");
					}
				}
			}
			return null;
		}

		// Token: 0x06001997 RID: 6551 RVA: 0x00083AF4 File Offset: 0x00081CF4
		public Color GetPickupColor()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					return ColorCatalog.GetColor(ItemCatalog.GetItemDef((ItemIndex)this.value).colorIndex);
				}
				if (this.value < 105)
				{
					return ColorCatalog.GetColor(EquipmentCatalog.GetEquipmentDef((EquipmentIndex)(this.value - 78)).colorIndex);
				}
				if (this.value < 106)
				{
					return ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItem);
				}
			}
			return Color.black;
		}

		// Token: 0x06001998 RID: 6552 RVA: 0x00083B74 File Offset: 0x00081D74
		public Color GetPickupColorDark()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					return ColorCatalog.GetColor(ItemCatalog.GetItemDef((ItemIndex)this.value).darkColorIndex);
				}
				if (this.value < 105)
				{
					return ColorCatalog.GetColor(EquipmentCatalog.GetEquipmentDef((EquipmentIndex)(this.value - 78)).colorIndex);
				}
				if (this.value < 106)
				{
					return ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItem);
				}
			}
			return Color.black;
		}

		// Token: 0x06001999 RID: 6553 RVA: 0x00083BF4 File Offset: 0x00081DF4
		public string GetPickupNameToken()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					return ItemCatalog.GetItemDef((ItemIndex)this.value).nameToken;
				}
				if (this.value < 105)
				{
					return EquipmentCatalog.GetEquipmentDef((EquipmentIndex)(this.value - 78)).nameToken;
				}
				if (this.value < 106)
				{
					return "PICKUP_LUNAR_COIN";
				}
			}
			return "???";
		}

		// Token: 0x0600199A RID: 6554 RVA: 0x00083C58 File Offset: 0x00081E58
		public string GetUnlockableName()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					return ItemCatalog.GetItemDef((ItemIndex)this.value).unlockableName;
				}
				if (this.value < 105)
				{
					return EquipmentCatalog.GetEquipmentDef((EquipmentIndex)(this.value - 78)).unlockableName;
				}
			}
			return "";
		}

		// Token: 0x0600199B RID: 6555 RVA: 0x00083CAC File Offset: 0x00081EAC
		public bool IsLunar()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					return ItemCatalog.GetItemDef((ItemIndex)this.value).tier == ItemTier.Lunar;
				}
				if (this.value < 105)
				{
					return EquipmentCatalog.GetEquipmentDef((EquipmentIndex)(this.value - 78)).isLunar;
				}
			}
			return false;
		}

		// Token: 0x0600199C RID: 6556 RVA: 0x00083D00 File Offset: 0x00081F00
		public bool IsBoss()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					return ItemCatalog.GetItemDef((ItemIndex)this.value).tier == ItemTier.Boss;
				}
				if (this.value < 105)
				{
					return EquipmentCatalog.GetEquipmentDef((EquipmentIndex)(this.value - 78)).isBoss;
				}
			}
			return false;
		}

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x0600199D RID: 6557 RVA: 0x0001314E File Offset: 0x0001134E
		public ItemIndex itemIndex
		{
			get
			{
				if (this.value < 0 || this.value >= 78)
				{
					return ItemIndex.None;
				}
				return (ItemIndex)this.value;
			}
		}

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x0600199E RID: 6558 RVA: 0x0001316B File Offset: 0x0001136B
		public EquipmentIndex equipmentIndex
		{
			get
			{
				if (this.value < 78 || this.value >= 105)
				{
					return EquipmentIndex.None;
				}
				return (EquipmentIndex)(this.value - 78);
			}
		}

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x0600199F RID: 6559 RVA: 0x0001318C File Offset: 0x0001138C
		public int coinIndex
		{
			get
			{
				if (this.value < 105 || this.value >= 106)
				{
					return -1;
				}
				return this.value - 105;
			}
		}

		// Token: 0x060019A0 RID: 6560 RVA: 0x000131AD File Offset: 0x000113AD
		public static bool operator ==(PickupIndex a, PickupIndex b)
		{
			return a.value == b.value;
		}

		// Token: 0x060019A1 RID: 6561 RVA: 0x000131BD File Offset: 0x000113BD
		public static bool operator !=(PickupIndex a, PickupIndex b)
		{
			return a.value != b.value;
		}

		// Token: 0x060019A2 RID: 6562 RVA: 0x000131D0 File Offset: 0x000113D0
		public static bool operator <(PickupIndex a, PickupIndex b)
		{
			return a.value < b.value;
		}

		// Token: 0x060019A3 RID: 6563 RVA: 0x000131E0 File Offset: 0x000113E0
		public static bool operator >(PickupIndex a, PickupIndex b)
		{
			return a.value > b.value;
		}

		// Token: 0x060019A4 RID: 6564 RVA: 0x000131F0 File Offset: 0x000113F0
		public static bool operator <=(PickupIndex a, PickupIndex b)
		{
			return a.value >= b.value;
		}

		// Token: 0x060019A5 RID: 6565 RVA: 0x00013203 File Offset: 0x00011403
		public static bool operator >=(PickupIndex a, PickupIndex b)
		{
			return a.value <= b.value;
		}

		// Token: 0x060019A6 RID: 6566 RVA: 0x00013216 File Offset: 0x00011416
		public static PickupIndex operator ++(PickupIndex a)
		{
			return new PickupIndex(a.value + 1);
		}

		// Token: 0x060019A7 RID: 6567 RVA: 0x00013225 File Offset: 0x00011425
		public static PickupIndex operator --(PickupIndex a)
		{
			return new PickupIndex(a.value - 1);
		}

		// Token: 0x060019A8 RID: 6568 RVA: 0x00013234 File Offset: 0x00011434
		public override bool Equals(object obj)
		{
			return obj is PickupIndex && this == (PickupIndex)obj;
		}

		// Token: 0x060019A9 RID: 6569 RVA: 0x000131AD File Offset: 0x000113AD
		public bool Equals(PickupIndex other)
		{
			return this.value == other.value;
		}

		// Token: 0x060019AA RID: 6570 RVA: 0x00083D54 File Offset: 0x00081F54
		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		// Token: 0x060019AB RID: 6571 RVA: 0x00013251 File Offset: 0x00011451
		public static void WriteToNetworkWriter(NetworkWriter writer, PickupIndex value)
		{
			writer.Write((byte)(value.value + 1));
		}

		// Token: 0x060019AC RID: 6572 RVA: 0x00013262 File Offset: 0x00011462
		public static PickupIndex ReadFromNetworkReader(NetworkReader reader)
		{
			return new PickupIndex((int)(reader.ReadByte() - 1));
		}

		// Token: 0x060019AD RID: 6573 RVA: 0x00083D70 File Offset: 0x00081F70
		static PickupIndex()
		{
			PickupIndex.allPickupNames[0] = "None";
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				PickupIndex.allPickupNames[(int)(1 + itemIndex)] = "ItemIndex." + itemIndex.ToString();
			}
			for (EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile; equipmentIndex < EquipmentIndex.Count; equipmentIndex++)
			{
				PickupIndex.allPickupNames[(int)(79 + equipmentIndex)] = "EquipmentIndex." + equipmentIndex.ToString();
			}
			for (int i = 105; i < 106; i++)
			{
				PickupIndex.allPickupNames[1 + i] = "LunarCoin.Coin" + (i - 105);
			}
			PickupIndex.stringToPickupIndexTable = new Dictionary<string, PickupIndex>();
			for (int j = 0; j < PickupIndex.allPickupNames.Length; j++)
			{
				PickupIndex.stringToPickupIndexTable.Add(PickupIndex.allPickupNames[j], new PickupIndex(j - 1));
			}
		}

		// Token: 0x060019AE RID: 6574 RVA: 0x00083E8C File Offset: 0x0008208C
		public override string ToString()
		{
			int num = this.value + 1;
			if (num > -1 && num < PickupIndex.allPickupNames.Length)
			{
				return PickupIndex.allPickupNames[num];
			}
			return string.Format("BadPickupIndex{0}", this.value);
		}

		// Token: 0x060019AF RID: 6575 RVA: 0x00083ED0 File Offset: 0x000820D0
		public static PickupIndex Find(string name)
		{
			PickupIndex result;
			if (PickupIndex.stringToPickupIndexTable.TryGetValue(name, out result))
			{
				return result;
			}
			return PickupIndex.none;
		}

		// Token: 0x060019B0 RID: 6576 RVA: 0x00083EF4 File Offset: 0x000820F4
		public static PickupIndex.Enumerator GetEnumerator()
		{
			return default(PickupIndex.Enumerator);
		}

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x060019B1 RID: 6577 RVA: 0x00083F0C File Offset: 0x0008210C
		public static GenericStaticEnumerable<PickupIndex, PickupIndex.Enumerator> allPickups
		{
			get
			{
				return default(GenericStaticEnumerable<PickupIndex, PickupIndex.Enumerator>);
			}
		}

		// Token: 0x04001CD8 RID: 7384
		public static readonly PickupIndex none = new PickupIndex(-1);

		// Token: 0x04001CD9 RID: 7385
		[SerializeField]
		public readonly int value;

		// Token: 0x04001CDA RID: 7386
		private const int pickupsStart = -1;

		// Token: 0x04001CDB RID: 7387
		private const int itemStart = 0;

		// Token: 0x04001CDC RID: 7388
		private const int itemEnd = 78;

		// Token: 0x04001CDD RID: 7389
		private const int equipmentStart = 78;

		// Token: 0x04001CDE RID: 7390
		private const int equipmentEnd = 105;

		// Token: 0x04001CDF RID: 7391
		private const int coinsStart = 105;

		// Token: 0x04001CE0 RID: 7392
		private const int coinsCount = 1;

		// Token: 0x04001CE1 RID: 7393
		private const int coinsEnd = 106;

		// Token: 0x04001CE2 RID: 7394
		public static readonly PickupIndex lunarCoin1 = new PickupIndex(105);

		// Token: 0x04001CE3 RID: 7395
		private const int pickupsEnd = 106;

		// Token: 0x04001CE4 RID: 7396
		public static readonly PickupIndex first = new PickupIndex(0);

		// Token: 0x04001CE5 RID: 7397
		public static readonly PickupIndex last = new PickupIndex(105);

		// Token: 0x04001CE6 RID: 7398
		public static readonly PickupIndex end = new PickupIndex(106);

		// Token: 0x04001CE7 RID: 7399
		public const int count = 106;

		// Token: 0x04001CE8 RID: 7400
		public static readonly string[] allPickupNames = new string[107];

		// Token: 0x04001CE9 RID: 7401
		private static readonly Dictionary<string, PickupIndex> stringToPickupIndexTable;

		// Token: 0x02000473 RID: 1139
		public struct Enumerator : IEnumerator<PickupIndex>, IEnumerator, IDisposable
		{
			// Token: 0x060019B2 RID: 6578 RVA: 0x00013271 File Offset: 0x00011471
			public bool MoveNext()
			{
				this.position = ++this.position;
				return this.position < PickupIndex.end;
			}

			// Token: 0x060019B3 RID: 6579 RVA: 0x00013294 File Offset: 0x00011494
			public void Reset()
			{
				this.position = PickupIndex.none;
			}

			// Token: 0x17000263 RID: 611
			// (get) Token: 0x060019B4 RID: 6580 RVA: 0x000132A1 File Offset: 0x000114A1
			public PickupIndex Current
			{
				get
				{
					return this.position;
				}
			}

			// Token: 0x17000264 RID: 612
			// (get) Token: 0x060019B5 RID: 6581 RVA: 0x000132A9 File Offset: 0x000114A9
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			// Token: 0x060019B6 RID: 6582 RVA: 0x000025DA File Offset: 0x000007DA
			void IDisposable.Dispose()
			{
			}

			// Token: 0x04001CEA RID: 7402
			private PickupIndex position;
		}
	}
}
