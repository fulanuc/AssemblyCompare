using System;

namespace RoR2
{
	// Token: 0x02000444 RID: 1092
	internal struct HullDef
	{
		// Token: 0x06001871 RID: 6257 RVA: 0x0007E67C File Offset: 0x0007C87C
		static HullDef()
		{
			HullDef.hullDefs[0] = new HullDef
			{
				height = 2f,
				radius = 0.5f
			};
			HullDef.hullDefs[1] = new HullDef
			{
				height = 8f,
				radius = 1.8f
			};
			HullDef.hullDefs[2] = new HullDef
			{
				height = 20f,
				radius = 5f
			};
		}

		// Token: 0x06001872 RID: 6258 RVA: 0x0001250F File Offset: 0x0001070F
		public static HullDef Find(HullClassification hullClassification)
		{
			return HullDef.hullDefs[(int)hullClassification];
		}

		// Token: 0x04001BA2 RID: 7074
		public float height;

		// Token: 0x04001BA3 RID: 7075
		public float radius;

		// Token: 0x04001BA4 RID: 7076
		private static HullDef[] hullDefs = new HullDef[3];
	}
}
