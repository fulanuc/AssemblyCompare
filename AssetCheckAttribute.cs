using System;

namespace RoR2
{
	// Token: 0x0200046F RID: 1135
	[AttributeUsage(AttributeTargets.Method)]
	public class AssetCheckAttribute : Attribute
	{
		// Token: 0x06001968 RID: 6504 RVA: 0x00012E58 File Offset: 0x00011058
		public AssetCheckAttribute(Type assetType)
		{
			this.assetType = assetType;
		}

		// Token: 0x04001CCC RID: 7372
		public Type assetType;
	}
}
