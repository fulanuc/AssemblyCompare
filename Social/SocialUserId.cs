using System;

namespace RoR2.Social
{
	// Token: 0x02000508 RID: 1288
	public struct SocialUserId
	{
		// Token: 0x06001D3B RID: 7483 RVA: 0x000156D2 File Offset: 0x000138D2
		public SocialUserId(CSteamID steamId)
		{
			this.steamId = steamId;
		}

		// Token: 0x04001F46 RID: 8006
		public readonly CSteamID steamId;
	}
}
