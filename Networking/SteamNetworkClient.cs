using System;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x0200059F RID: 1439
	public class SteamNetworkClient : NetworkClient
	{
		// Token: 0x170002DC RID: 732
		// (get) Token: 0x0600209F RID: 8351 RVA: 0x00017CC9 File Offset: 0x00015EC9
		public SteamNetworkConnection steamConnection
		{
			get
			{
				return (SteamNetworkConnection)base.connection;
			}
		}

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x060020A0 RID: 8352 RVA: 0x00017CD6 File Offset: 0x00015ED6
		public string status
		{
			get
			{
				return this.m_AsyncConnect.ToString();
			}
		}

		// Token: 0x060020A1 RID: 8353 RVA: 0x00017CE9 File Offset: 0x00015EE9
		public void Connect()
		{
			base.Connect("localhost", 0);
			this.m_AsyncConnect = NetworkClient.ConnectState.Connected;
			base.connection.ForceInitialize(base.hostTopology);
		}

		// Token: 0x060020A2 RID: 8354 RVA: 0x00017D0F File Offset: 0x00015F0F
		public SteamNetworkClient(NetworkConnection conn) : base(conn)
		{
			base.SetNetworkConnectionClass<SteamNetworkConnection>();
		}
	}
}
