using System;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x0200058C RID: 1420
	public class SteamNetworkClient : NetworkClient
	{
		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x0600200E RID: 8206 RVA: 0x000175B9 File Offset: 0x000157B9
		public SteamNetworkConnection steamConnection
		{
			get
			{
				return (SteamNetworkConnection)base.connection;
			}
		}

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x0600200F RID: 8207 RVA: 0x000175C6 File Offset: 0x000157C6
		public string status
		{
			get
			{
				return this.m_AsyncConnect.ToString();
			}
		}

		// Token: 0x06002010 RID: 8208 RVA: 0x000175D9 File Offset: 0x000157D9
		public void Connect()
		{
			base.Connect("localhost", 0);
			this.m_AsyncConnect = NetworkClient.ConnectState.Connected;
			base.connection.ForceInitialize(base.hostTopology);
		}

		// Token: 0x06002011 RID: 8209 RVA: 0x000175FF File Offset: 0x000157FF
		public SteamNetworkClient(NetworkConnection conn) : base(conn)
		{
			base.SetNetworkConnectionClass<SteamNetworkConnection>();
		}
	}
}
