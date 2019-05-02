using System;
using Facepunch.Steamworks;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x020005A0 RID: 1440
	public class SteamNetworkConnection : NetworkConnection
	{
		// Token: 0x060020A3 RID: 8355 RVA: 0x00017D1E File Offset: 0x00015F1E
		public SteamNetworkConnection()
		{
		}

		// Token: 0x060020A4 RID: 8356 RVA: 0x00017D26 File Offset: 0x00015F26
		public SteamNetworkConnection(CSteamID steamId)
		{
			this.steamId = steamId;
			Client.Instance.Networking.CloseSession(steamId.value);
		}

		// Token: 0x060020A5 RID: 8357 RVA: 0x0009D3EC File Offset: 0x0009B5EC
		public override bool TransportSend(byte[] bytes, int numBytes, int channelId, out byte error)
		{
			if (this.ignore)
			{
				error = 0;
				return true;
			}
			this.logNetworkMessages = SteamNetworkConnection.cvNetP2PLogMessages.value;
			Client instance = Client.Instance;
			if (this.steamId.value == instance.SteamId)
			{
				this.TransportReceive(bytes, numBytes, channelId);
				error = 0;
				if (SteamNetworkConnection.cvNetP2PDebugTransport.value)
				{
					Debug.LogFormat("SteamNetworkConnection.TransportSend steamId=self numBytes={1} channelId={2}", new object[]
					{
						numBytes,
						channelId
					});
				}
				return true;
			}
			Networking.SendType eP2PSendType = Networking.SendType.Reliable;
			QosType qos = GameNetworkManager.singleton.connectionConfig.Channels[channelId].QOS;
			if (qos == QosType.Unreliable || qos == QosType.UnreliableFragmented || qos == QosType.UnreliableSequenced)
			{
				eP2PSendType = Networking.SendType.Unreliable;
			}
			if (instance.Networking.SendP2PPacket(this.steamId.value, bytes, numBytes, eP2PSendType, 0))
			{
				error = 0;
				if (SteamNetworkConnection.cvNetP2PDebugTransport.value)
				{
					Debug.LogFormat("SteamNetworkConnection.TransportSend steamId={0} numBytes={1} channelId={2} error={3}", new object[]
					{
						this.steamId.value,
						numBytes,
						channelId,
						error
					});
				}
				return true;
			}
			error = 1;
			if (SteamNetworkConnection.cvNetP2PDebugTransport.value)
			{
				Debug.LogFormat("SteamNetworkConnection.TransportSend steamId={0} numBytes={1} channelId={2} error={3}", new object[]
				{
					this.steamId.value,
					numBytes,
					channelId,
					error
				});
			}
			return false;
		}

		// Token: 0x060020A6 RID: 8358 RVA: 0x00017D4B File Offset: 0x00015F4B
		public override void TransportReceive(byte[] bytes, int numBytes, int channelId)
		{
			if (this.ignore)
			{
				return;
			}
			this.logNetworkMessages = SteamNetworkConnection.cvNetP2PLogMessages.value;
			base.TransportReceive(bytes, numBytes, channelId);
		}

		// Token: 0x060020A7 RID: 8359 RVA: 0x0009D558 File Offset: 0x0009B758
		protected override void Dispose(bool disposing)
		{
			if (Client.Instance != null && this.steamId.value != 0UL)
			{
				Client.Instance.Networking.CloseSession(this.steamId.value);
				this.steamId = CSteamID.nil;
			}
			base.Dispose(disposing);
		}

		// Token: 0x0400227D RID: 8829
		public CSteamID steamId;

		// Token: 0x0400227E RID: 8830
		public bool ignore;

		// Token: 0x0400227F RID: 8831
		public uint rtt;

		// Token: 0x04002280 RID: 8832
		public static BoolConVar cvNetP2PDebugTransport = new BoolConVar("net_p2p_debug_transport", ConVarFlags.None, "0", "Allows p2p transport information to print to the console.");

		// Token: 0x04002281 RID: 8833
		private static BoolConVar cvNetP2PLogMessages = new BoolConVar("net_p2p_log_messages", ConVarFlags.None, "0", "Enables logging of network messages.");
	}
}
