using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200044B RID: 1099
	public class ItemDef
	{
		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06001876 RID: 6262 RVA: 0x00012535 File Offset: 0x00010735
		public bool inDroppableTier
		{
			get
			{
				return this.tier != ItemTier.NoTier;
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06001877 RID: 6263 RVA: 0x00012543 File Offset: 0x00010743
		public Texture pickupIconTexture
		{
			get
			{
				return Resources.Load<Texture>(this.pickupIconPath);
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06001878 RID: 6264 RVA: 0x0007ED88 File Offset: 0x0007CF88
		public Texture bgIconTexture
		{
			get
			{
				switch (this.tier)
				{
				case ItemTier.Tier1:
					return Resources.Load<Texture>("Textures/ItemIcons/BG/texTier1BGIcon");
				case ItemTier.Tier2:
					return Resources.Load<Texture>("Textures/ItemIcons/BG/texTier2BGIcon");
				case ItemTier.Tier3:
					return Resources.Load<Texture>("Textures/ItemIcons/BG/texTier3BGIcon");
				case ItemTier.Lunar:
					return Resources.Load<Texture>("Textures/ItemIcons/BG/texLunarBGIcon");
				case ItemTier.Boss:
					return Resources.Load<Texture>("Textures/ItemIcons/BG/texBossBGIcon");
				default:
					return null;
				}
			}
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06001879 RID: 6265 RVA: 0x00012550 File Offset: 0x00010750
		public Sprite pickupIconSprite
		{
			get
			{
				return Resources.Load<Sprite>(this.pickupIconPath);
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x0600187A RID: 6266 RVA: 0x0007EDF0 File Offset: 0x0007CFF0
		public ColorCatalog.ColorIndex colorIndex
		{
			get
			{
				switch (this.tier)
				{
				case ItemTier.Tier1:
					return ColorCatalog.ColorIndex.Tier1Item;
				case ItemTier.Tier2:
					return ColorCatalog.ColorIndex.Tier2Item;
				case ItemTier.Tier3:
					return ColorCatalog.ColorIndex.Tier3Item;
				case ItemTier.Lunar:
					return ColorCatalog.ColorIndex.LunarItem;
				case ItemTier.Boss:
					return ColorCatalog.ColorIndex.BossItem;
				default:
					return ColorCatalog.ColorIndex.None;
				}
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x0600187B RID: 6267 RVA: 0x0007EE2C File Offset: 0x0007D02C
		public ColorCatalog.ColorIndex darkColorIndex
		{
			get
			{
				switch (this.tier)
				{
				case ItemTier.Tier1:
					return ColorCatalog.ColorIndex.Tier1ItemDark;
				case ItemTier.Tier2:
					return ColorCatalog.ColorIndex.Tier2ItemDark;
				case ItemTier.Tier3:
					return ColorCatalog.ColorIndex.Tier3ItemDark;
				case ItemTier.Lunar:
					return ColorCatalog.ColorIndex.LunarItemDark;
				case ItemTier.Boss:
					return ColorCatalog.ColorIndex.BossItem;
				default:
					return ColorCatalog.ColorIndex.None;
				}
			}
		}

		// Token: 0x04001BF7 RID: 7159
		public ItemIndex itemIndex;

		// Token: 0x04001BF8 RID: 7160
		public ItemTier tier;

		// Token: 0x04001BF9 RID: 7161
		public string pickupModelPath;

		// Token: 0x04001BFA RID: 7162
		public string nameToken;

		// Token: 0x04001BFB RID: 7163
		public string pickupToken;

		// Token: 0x04001BFC RID: 7164
		public string descriptionToken;

		// Token: 0x04001BFD RID: 7165
		public string loreToken;

		// Token: 0x04001BFE RID: 7166
		public string addressToken;

		// Token: 0x04001BFF RID: 7167
		public string pickupIconPath;

		// Token: 0x04001C00 RID: 7168
		public string unlockableName = "";

		// Token: 0x04001C01 RID: 7169
		public bool hidden;

		// Token: 0x04001C02 RID: 7170
		public bool canRemove = true;

		// Token: 0x04001C03 RID: 7171
		public MageElement mageElement;
	}
}
