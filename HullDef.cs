using System;

namespace RoR2
{
	// Token: 0x0200043C RID: 1084
	internal struct HullDef
	{
		// Token: 0x06001824 RID: 6180 RVA: 0x0007DEC0 File Offset: 0x0007C0C0
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

		// Token: 0x06001825 RID: 6181 RVA: 0x0001209B File Offset: 0x0001029B
		public static HullDef Find(HullClassification hullClassification)
		{
			return HullDef.hullDefs[(int)hullClassification];
		}

		// Token: 0x04001B72 RID: 7026
		public float height;

		// Token: 0x04001B73 RID: 7027
		public float radius;

		// Token: 0x04001B74 RID: 7028
		private static HullDef[] hullDefs = new HullDef[3];
	}
}
