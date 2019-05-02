using System;

namespace RoR2.Social
{
	// Token: 0x02000517 RID: 1303
	public struct SocialUserId
	{
		// Token: 0x06001DA3 RID: 7587 RVA: 0x00015B7B File Offset: 0x00013D7B
		public SocialUserId(CSteamID steamId)
		{
			this.steamId = steamId;
		}

		// Token: 0x04001F84 RID: 8068
		public readonly CSteamID steamId;
	}
}
