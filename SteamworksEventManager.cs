using System;
using Facepunch.Steamworks;

namespace RoR2
{
	// Token: 0x020004AB RID: 1195
	public static class SteamworksEventManager
	{
		// Token: 0x06001AB9 RID: 6841 RVA: 0x00013D2F File Offset: 0x00011F2F
		public static void Init(Client client)
		{
			SteamworksLobbyManager.SetupCallbacks(client);
		}
	}
}
