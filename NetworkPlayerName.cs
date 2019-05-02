using System;
using Facepunch.Steamworks;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200045E RID: 1118
	public struct NetworkPlayerName
	{
		// Token: 0x0600190A RID: 6410 RVA: 0x000129F3 File Offset: 0x00010BF3
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

		// Token: 0x0600190B RID: 6411 RVA: 0x0008218C File Offset: 0x0008038C
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

		// Token: 0x0600190C RID: 6412 RVA: 0x000821CC File Offset: 0x000803CC
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

		// Token: 0x04001C65 RID: 7269
		public CSteamID steamId;

		// Token: 0x04001C66 RID: 7270
		public string nameOverride;
	}
}
