using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000455 RID: 1109
	public class ItemDef
	{
		// Token: 0x1700023F RID: 575
		// (get) Token: 0x060018CB RID: 6347 RVA: 0x00012A0C File Offset: 0x00010C0C
		public bool inDroppableTier
		{
			get
			{
				return this.tier != ItemTier.NoTier;
			}
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x060018CC RID: 6348 RVA: 0x00012A1A File Offset: 0x00010C1A
		public Texture pickupIconTexture
		{
			get
			{
				return Resources.Load<Texture>(this.pickupIconPath);
			}
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x060018CD RID: 6349 RVA: 0x0007F568 File Offset: 0x0007D768
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

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x060018CE RID: 6350 RVA: 0x00012A27 File Offset: 0x00010C27
		public Sprite pickupIconSprite
		{
			get
			{
				return Resources.Load<Sprite>(this.pickupIconPath);
			}
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x060018CF RID: 6351 RVA: 0x0007F5D0 File Offset: 0x0007D7D0
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

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x060018D0 RID: 6352 RVA: 0x0007F60C File Offset: 0x0007D80C
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

		// Token: 0x04001C29 RID: 7209
		public ItemIndex itemIndex;

		// Token: 0x04001C2A RID: 7210
		public ItemTier tier;

		// Token: 0x04001C2B RID: 7211
		public string pickupModelPath;

		// Token: 0x04001C2C RID: 7212
		public string nameToken;

		// Token: 0x04001C2D RID: 7213
		public string pickupToken;

		// Token: 0x04001C2E RID: 7214
		public string descriptionToken;

		// Token: 0x04001C2F RID: 7215
		public string loreToken;

		// Token: 0x04001C30 RID: 7216
		public string addressToken;

		// Token: 0x04001C31 RID: 7217
		public string pickupIconPath;

		// Token: 0x04001C32 RID: 7218
		public string unlockableName = "";

		// Token: 0x04001C33 RID: 7219
		public bool hidden;

		// Token: 0x04001C34 RID: 7220
		public bool canRemove = true;

		// Token: 0x04001C35 RID: 7221
		public MageElement mageElement;
	}
}
