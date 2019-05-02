using System;
using JetBrains.Annotations;

namespace RoR2
{
	// Token: 0x0200047A RID: 1146
	[AttributeUsage(AttributeTargets.Method)]
	[MeansImplicitUse]
	public class AssetCheckAttribute : Attribute
	{
		// Token: 0x060019C5 RID: 6597 RVA: 0x00013372 File Offset: 0x00011572
		public AssetCheckAttribute(Type assetType)
		{
			this.assetType = assetType;
		}

		// Token: 0x04001D00 RID: 7424
		public Type assetType;
	}
}
