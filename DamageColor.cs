using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000235 RID: 565
	public static class DamageColor
	{
		// Token: 0x06000ABE RID: 2750 RVA: 0x0004906C File Offset: 0x0004726C
		static DamageColor()
		{
			DamageColor.colors[0] = Color.white;
			DamageColor.colors[1] = new Color(0.3647059f, 0.8156863f, 0.149019614f);
			DamageColor.colors[2] = new Color(0.796078444f, 0.1882353f, 0.1882353f);
			DamageColor.colors[3] = new Color(0.827451f, 0.7490196f, 0.3137255f);
			DamageColor.colors[4] = new Color(0.68235296f, 0.9490196f, 0.298039228f);
			DamageColor.colors[5] = new Color(0.9372549f, 0.5176471f, 0.203921571f);
		}

		// Token: 0x06000ABF RID: 2751 RVA: 0x00008AB1 File Offset: 0x00006CB1
		public static Color FindColor(DamageColorIndex colorIndex)
		{
			if (colorIndex < DamageColorIndex.Default || colorIndex >= DamageColorIndex.Count)
			{
				return Color.white;
			}
			return DamageColor.colors[(int)colorIndex];
		}

		// Token: 0x04000E6E RID: 3694
		private static Color[] colors = new Color[7];
	}
}
