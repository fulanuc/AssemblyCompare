using System;
using Facepunch.Steamworks;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000469 RID: 1129
	public struct NetworkPlayerName
	{
		// Token: 0x06001967 RID: 6503 RVA: 0x00012F0D File Offset: 0x0001110D
		public void Deserialize(NetworkReader reader)
		{
			if (reader.ReadBoolean())
			{
				this.steamId = CSteamID.nil;
				this.nameOverride = reader.ReadString();
				return;
			}
			this.steamId = new CSteamID(reader.ReadUInt64());
			this.nameOverride = null;
		}

		// Token: 0x06001968 RID: 6504 RVA: 0x00082B34 File Offset: 0x00080D34
		public void Serialize(NetworkWriter writer)
		{
			bool flag = this.nameOverride != null;
			writer.Write(flag);
			if (flag)
			{
				writer.Write(this.nameOverride);
				return;
			}
			writer.Write(this.steamId.value);
		}

		// Token: 0x06001969 RID: 6505 RVA: 0x00082B74 File Offset: 0x00080D74
		public string GetResolvedName()
		{
			if (this.nameOverride != null)
			{
				return this.nameOverride;
			}
			Client instance = Client.Instance;
			if (instance != null)
			{
				return instance.Friends.GetName(this.steamId.value);
			}
			return "???";
		}

		// Token: 0x04001C99 RID: 7321
		public CSteamID steamId;

		// Token: 0x04001C9A RID: 7322
		public string nameOverride;
	}
}
