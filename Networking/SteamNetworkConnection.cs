using System;
using Facepunch.Steamworks;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x0200058D RID: 1421
	public class SteamNetworkConnection : NetworkConnection
	{
		// Token: 0x06002012 RID: 8210 RVA: 0x0001760E File Offset: 0x0001580E
		public SteamNetworkConnection()
		{
		}

		// Token: 0x06002013 RID: 8211 RVA: 0x00017616 File Offset: 0x00015816
		public SteamNetworkConnection(CSteamID steamId)
		{
			this.steamId = steamId;
		}

		// Token: 0x06002014 RID: 8212 RVA: 0x0009BED0 File Offset: 0x0009A0D0
		public override bool TransportSend(byte[] bytes, int numBytes, int channelId, out byte error)
		{
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

		// Token: 0x06002015 RID: 8213 RVA: 0x00017625 File Offset: 0x00015825
		public override void TransportReceive(byte[] bytes, int numBytes, int channelId)
		{
			this.logNetworkMessages = SteamNetworkConnection.cvNetP2PLogMessages.value;
			base.TransportReceive(bytes, numBytes, channelId);
		}

		// Token: 0x06002016 RID: 8214 RVA: 0x0009C02C File Offset: 0x0009A22C
		protected override void Dispose(bool disposing)
		{
			if (Client.Instance != null && this.steamId.value != 0UL)
			{
				Client.Instance.Networking.CloseSession(this.steamId.value);
				this.steamId = CSteamID.nil;
			}
			base.Dispose(disposing);
		}

		// Token: 0x04002226 RID: 8742
		public CSteamID steamId;

		// Token: 0x04002227 RID: 8743
		public uint rtt;

		// Token: 0x04002228 RID: 8744
		public static BoolConVar cvNetP2PDebugTransport = new BoolConVar("net_p2p_debug_transport", ConVarFlags.None, "0", "Allows p2p transport information to print to the console.");

		// Token: 0x04002229 RID: 8745
		private static BoolConVar cvNetP2PLogMessages = new BoolConVar("net_p2p_log_messages", ConVarFlags.None, "0", "Enables logging of network messages.");
	}
}
