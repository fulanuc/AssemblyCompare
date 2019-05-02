using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000467 RID: 1127
	[Serializable]
	public struct PickupIndex : IEquatable<PickupIndex>
	{
		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06001933 RID: 6451 RVA: 0x00012BF9 File Offset: 0x00010DF9
		public bool isValid
		{
			get
			{
				return this.value < 106;
			}
		}

		// Token: 0x06001934 RID: 6452 RVA: 0x00012C05 File Offset: 0x00010E05
		private PickupIndex(int value)
		{
			this.value = ((value < 0) ? -1 : value);
		}

		// Token: 0x06001935 RID: 6453 RVA: 0x00012C05 File Offset: 0x00010E05
		public PickupIndex(ItemIndex itemIndex)
		{
			this.value = (int)((itemIndex < ItemIndex.Syringe) ? ItemIndex.None : itemIndex);
		}

		// Token: 0x06001936 RID: 6454 RVA: 0x00012C15 File Offset: 0x00010E15
		public PickupIndex(EquipmentIndex equipmentIndex)
		{
			this.value = (int)((equipmentIndex < EquipmentIndex.CommandMissile) ? EquipmentIndex.None : (78 + equipmentIndex));
		}

		// Token: 0x06001937 RID: 6455 RVA: 0x00012C28 File Offset: 0x00010E28
		public GameObject GetHiddenPickupDisplayPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");
		}

		// Token: 0x06001938 RID: 6456 RVA: 0x00083034 File Offset: 0x00081234
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

		// Token: 0x06001939 RID: 6457 RVA: 0x000830A4 File Offset: 0x000812A4
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

		// Token: 0x0600193A RID: 6458 RVA: 0x0008314C File Offset: 0x0008134C
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

		// Token: 0x0600193B RID: 6459 RVA: 0x000831CC File Offset: 0x000813CC
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

		// Token: 0x0600193C RID: 6460 RVA: 0x0008324C File Offset: 0x0008144C
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

		// Token: 0x0600193D RID: 6461 RVA: 0x000832B0 File Offset: 0x000814B0
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

		// Token: 0x0600193E RID: 6462 RVA: 0x00083304 File Offset: 0x00081504
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

		// Token: 0x0600193F RID: 6463 RVA: 0x00083358 File Offset: 0x00081558
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

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06001940 RID: 6464 RVA: 0x00012C34 File Offset: 0x00010E34
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

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06001941 RID: 6465 RVA: 0x00012C51 File Offset: 0x00010E51
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

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06001942 RID: 6466 RVA: 0x00012C72 File Offset: 0x00010E72
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

		// Token: 0x06001943 RID: 6467 RVA: 0x00012C93 File Offset: 0x00010E93
		public static bool operator ==(PickupIndex a, PickupIndex b)
		{
			return a.value == b.value;
		}

		// Token: 0x06001944 RID: 6468 RVA: 0x00012CA3 File Offset: 0x00010EA3
		public static bool operator !=(PickupIndex a, PickupIndex b)
		{
			return a.value != b.value;
		}

		// Token: 0x06001945 RID: 6469 RVA: 0x00012CB6 File Offset: 0x00010EB6
		public static bool operator <(PickupIndex a, PickupIndex b)
		{
			return a.value < b.value;
		}

		// Token: 0x06001946 RID: 6470 RVA: 0x00012CC6 File Offset: 0x00010EC6
		public static bool operator >(PickupIndex a, PickupIndex b)
		{
			return a.value > b.value;
		}

		// Token: 0x06001947 RID: 6471 RVA: 0x00012CD6 File Offset: 0x00010ED6
		public static bool operator <=(PickupIndex a, PickupIndex b)
		{
			return a.value >= b.value;
		}

		// Token: 0x06001948 RID: 6472 RVA: 0x00012CE9 File Offset: 0x00010EE9
		public static bool operator >=(PickupIndex a, PickupIndex b)
		{
			return a.value <= b.value;
		}

		// Token: 0x06001949 RID: 6473 RVA: 0x00012CFC File Offset: 0x00010EFC
		public static PickupIndex operator ++(PickupIndex a)
		{
			return new PickupIndex(a.value + 1);
		}

		// Token: 0x0600194A RID: 6474 RVA: 0x00012D0B File Offset: 0x00010F0B
		public static PickupIndex operator --(PickupIndex a)
		{
			return new PickupIndex(a.value - 1);
		}

		// Token: 0x0600194B RID: 6475 RVA: 0x00012D1A File Offset: 0x00010F1A
		public override bool Equals(object obj)
		{
			return obj is PickupIndex && this == (PickupIndex)obj;
		}

		// Token: 0x0600194C RID: 6476 RVA: 0x00012C93 File Offset: 0x00010E93
		public bool Equals(PickupIndex other)
		{
			return this.value == other.value;
		}

		// Token: 0x0600194D RID: 6477 RVA: 0x000833AC File Offset: 0x000815AC
		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		// Token: 0x0600194E RID: 6478 RVA: 0x00012D37 File Offset: 0x00010F37
		public static void WriteToNetworkWriter(NetworkWriter writer, PickupIndex value)
		{
			writer.Write((byte)(value.value + 1));
		}

		// Token: 0x0600194F RID: 6479 RVA: 0x00012D48 File Offset: 0x00010F48
		public static PickupIndex ReadFromNetworkReader(NetworkReader reader)
		{
			return new PickupIndex((int)(reader.ReadByte() - 1));
		}

		// Token: 0x06001950 RID: 6480 RVA: 0x000833C8 File Offset: 0x000815C8
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

		// Token: 0x06001951 RID: 6481 RVA: 0x000834E4 File Offset: 0x000816E4
		public override string ToString()
		{
			int num = this.value + 1;
			if (num > -1 && num < PickupIndex.allPickupNames.Length)
			{
				return PickupIndex.allPickupNames[num];
			}
			return string.Format("BadPickupIndex{0}", this.value);
		}

		// Token: 0x06001952 RID: 6482 RVA: 0x00083528 File Offset: 0x00081728
		public static PickupIndex Find(string name)
		{
			PickupIndex result;
			if (PickupIndex.stringToPickupIndexTable.TryGetValue(name, out result))
			{
				return result;
			}
			return PickupIndex.none;
		}

		// Token: 0x06001953 RID: 6483 RVA: 0x0008354C File Offset: 0x0008174C
		public static PickupIndex.Enumerator GetEnumerator()
		{
			return default(PickupIndex.Enumerator);
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06001954 RID: 6484 RVA: 0x00083564 File Offset: 0x00081764
		public static GenericStaticEnumerable<PickupIndex, PickupIndex.Enumerator> allPickups
		{
			get
			{
				return default(GenericStaticEnumerable<PickupIndex, PickupIndex.Enumerator>);
			}
		}

		// Token: 0x04001CA4 RID: 7332
		public static readonly PickupIndex none = new PickupIndex(-1);

		// Token: 0x04001CA5 RID: 7333
		[SerializeField]
		public readonly int value;

		// Token: 0x04001CA6 RID: 7334
		private const int pickupsStart = -1;

		// Token: 0x04001CA7 RID: 7335
		private const int itemStart = 0;

		// Token: 0x04001CA8 RID: 7336
		private const int itemEnd = 78;

		// Token: 0x04001CA9 RID: 7337
		private const int equipmentStart = 78;

		// Token: 0x04001CAA RID: 7338
		private const int equipmentEnd = 105;

		// Token: 0x04001CAB RID: 7339
		private const int coinsStart = 105;

		// Token: 0x04001CAC RID: 7340
		private const int coinsCount = 1;

		// Token: 0x04001CAD RID: 7341
		private const int coinsEnd = 106;

		// Token: 0x04001CAE RID: 7342
		public static readonly PickupIndex lunarCoin1 = new PickupIndex(105);

		// Token: 0x04001CAF RID: 7343
		private const int pickupsEnd = 106;

		// Token: 0x04001CB0 RID: 7344
		public static readonly PickupIndex first = new PickupIndex(0);

		// Token: 0x04001CB1 RID: 7345
		public static readonly PickupIndex last = new PickupIndex(105);

		// Token: 0x04001CB2 RID: 7346
		public static readonly PickupIndex end = new PickupIndex(106);

		// Token: 0x04001CB3 RID: 7347
		public const int count = 106;

		// Token: 0x04001CB4 RID: 7348
		public static readonly string[] allPickupNames = new string[107];

		// Token: 0x04001CB5 RID: 7349
		private static readonly Dictionary<string, PickupIndex> stringToPickupIndexTable;

		// Token: 0x02000468 RID: 1128
		public struct Enumerator : IEnumerator<PickupIndex>, IEnumerator, IDisposable
		{
			// Token: 0x06001955 RID: 6485 RVA: 0x00012D57 File Offset: 0x00010F57
			public bool MoveNext()
			{
				this.position = ++this.position;
				return this.position < PickupIndex.end;
			}

			// Token: 0x06001956 RID: 6486 RVA: 0x00012D7A File Offset: 0x00010F7A
			public void Reset()
			{
				this.position = PickupIndex.none;
			}

			// Token: 0x17000257 RID: 599
			// (get) Token: 0x06001957 RID: 6487 RVA: 0x00012D87 File Offset: 0x00010F87
			public PickupIndex Current
			{
				get
				{
					return this.position;
				}
			}

			// Token: 0x17000258 RID: 600
			// (get) Token: 0x06001958 RID: 6488 RVA: 0x00012D8F File Offset: 0x00010F8F
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			// Token: 0x06001959 RID: 6489 RVA: 0x000025F6 File Offset: 0x000007F6
			void IDisposable.Dispose()
			{
			}

			// Token: 0x04001CB6 RID: 7350
			private PickupIndex position;
		}
	}
}
