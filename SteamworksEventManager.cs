using System;
using Facepunch.Steamworks;

namespace RoR2
{
	// Token: 0x0200049E RID: 1182
	public static class SteamworksEventManager
	{
		// Token: 0x06001A55 RID: 6741 RVA: 0x00013819 File Offset: 0x00011A19
		public static void Init(Client client)
		{
			SteamworksLobbyManager.SetupCallbacks(client);
		}
	}
}
