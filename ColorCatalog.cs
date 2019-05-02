using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200021F RID: 543
	public class ColorCatalog
	{
		// Token: 0x06000A9E RID: 2718 RVA: 0x000487F0 File Offset: 0x000469F0
		static ColorCatalog()
		{
			ColorCatalog.indexToColor32[1] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			ColorCatalog.indexToColor32[2] = new Color32(119, byte.MaxValue, 23, byte.MaxValue);
			ColorCatalog.indexToColor32[3] = new Color32(231, 84, 58, byte.MaxValue);
			ColorCatalog.indexToColor32[4] = new Color32(48, 127, byte.MaxValue, byte.MaxValue);
			ColorCatalog.indexToColor32[5] = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);
			ColorCatalog.indexToColor32[6] = new Color32(235, 232, 122, byte.MaxValue);
			ColorCatalog.indexToColor32[7] = new Color32(231, 84, 58, byte.MaxValue);
			ColorCatalog.indexToColor32[8] = new Color32(126, 152, 9, byte.MaxValue);
			ColorCatalog.indexToColor32[10] = new Color32(100, 100, 100, byte.MaxValue);
			ColorCatalog.indexToColor32[11] = Color32.Lerp(new Color32(142, 56, 206, byte.MaxValue), new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), 0.575f);
			ColorCatalog.indexToColor32[12] = new Color32(198, 173, 250, byte.MaxValue);
			ColorCatalog.indexToColor32[13] = Color.yellow;
			ColorCatalog.indexToColor32[14] = new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);
			ColorCatalog.indexToColor32[15] = new Color32(106, 170, 95, byte.MaxValue);
			ColorCatalog.indexToColor32[16] = new Color32(173, 117, 80, byte.MaxValue);
			ColorCatalog.indexToColor32[17] = new Color32(142, 49, 49, byte.MaxValue);
			ColorCatalog.indexToColor32[18] = new Color32(193, 193, 193, byte.MaxValue);
			ColorCatalog.indexToColor32[19] = new Color32(88, 149, 88, byte.MaxValue);
			ColorCatalog.indexToColor32[20] = new Color32(142, 49, 49, byte.MaxValue);
			ColorCatalog.indexToColor32[21] = new Color32(76, 84, 144, byte.MaxValue);
			ColorCatalog.indexToColor32[22] = new Color32(200, 80, 0, byte.MaxValue);
			for (ColorCatalog.ColorIndex colorIndex = ColorCatalog.ColorIndex.None; colorIndex < ColorCatalog.ColorIndex.Count; colorIndex++)
			{
				ColorCatalog.indexToHexString[(int)colorIndex] = Util.RGBToHex(ColorCatalog.indexToColor32[(int)colorIndex]);
			}
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x0000891F File Offset: 0x00006B1F
		public static Color32 GetColor(ColorCatalog.ColorIndex colorIndex)
		{
			if (colorIndex < ColorCatalog.ColorIndex.None || colorIndex >= ColorCatalog.ColorIndex.Count)
			{
				colorIndex = ColorCatalog.ColorIndex.Error;
			}
			return ColorCatalog.indexToColor32[(int)colorIndex];
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x00008939 File Offset: 0x00006B39
		public static string GetColorHexString(ColorCatalog.ColorIndex colorIndex)
		{
			if (colorIndex < ColorCatalog.ColorIndex.None || colorIndex >= ColorCatalog.ColorIndex.Count)
			{
				colorIndex = ColorCatalog.ColorIndex.Error;
			}
			return ColorCatalog.indexToHexString[(int)colorIndex];
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x0000894F File Offset: 0x00006B4F
		public static Color GetMultiplayerColor(int playerSlot)
		{
			if (playerSlot >= 0 && playerSlot < ColorCatalog.multiplayerColors.Length)
			{
				return ColorCatalog.multiplayerColors[playerSlot];
			}
			return Color.black;
		}

		// Token: 0x04000DE7 RID: 3559
		private static readonly Color32[] indexToColor32 = new Color32[23];

		// Token: 0x04000DE8 RID: 3560
		private static readonly string[] indexToHexString = new string[23];

		// Token: 0x04000DE9 RID: 3561
		private static readonly Color[] multiplayerColors = new Color[]
		{
			new Color32(252, 62, 62, byte.MaxValue),
			new Color32(62, 109, 252, byte.MaxValue),
			new Color32(129, 252, 62, byte.MaxValue),
			new Color32(252, 241, 62, byte.MaxValue)
		};

		// Token: 0x02000220 RID: 544
		public enum ColorIndex
		{
			// Token: 0x04000DEB RID: 3563
			None,
			// Token: 0x04000DEC RID: 3564
			Tier1Item,
			// Token: 0x04000DED RID: 3565
			Tier2Item,
			// Token: 0x04000DEE RID: 3566
			Tier3Item,
			// Token: 0x04000DEF RID: 3567
			LunarItem,
			// Token: 0x04000DF0 RID: 3568
			Equipment,
			// Token: 0x04000DF1 RID: 3569
			Interactable,
			// Token: 0x04000DF2 RID: 3570
			Teleporter,
			// Token: 0x04000DF3 RID: 3571
			Money,
			// Token: 0x04000DF4 RID: 3572
			Blood,
			// Token: 0x04000DF5 RID: 3573
			Unaffordable,
			// Token: 0x04000DF6 RID: 3574
			Unlockable,
			// Token: 0x04000DF7 RID: 3575
			LunarCoin,
			// Token: 0x04000DF8 RID: 3576
			BossItem,
			// Token: 0x04000DF9 RID: 3577
			Error,
			// Token: 0x04000DFA RID: 3578
			EasyDifficulty,
			// Token: 0x04000DFB RID: 3579
			NormalDifficulty,
			// Token: 0x04000DFC RID: 3580
			HardDifficulty,
			// Token: 0x04000DFD RID: 3581
			Tier1ItemDark,
			// Token: 0x04000DFE RID: 3582
			Tier2ItemDark,
			// Token: 0x04000DFF RID: 3583
			Tier3ItemDark,
			// Token: 0x04000E00 RID: 3584
			LunarItemDark,
			// Token: 0x04000E01 RID: 3585
			WIP,
			// Token: 0x04000E02 RID: 3586
			Count
		}
	}
}
