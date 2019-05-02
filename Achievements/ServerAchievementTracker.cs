using System;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Achievements
{
	// Token: 0x020006B7 RID: 1719
	[RequireComponent(typeof(NetworkUser))]
	public class ServerAchievementTracker : NetworkBehaviour
	{
		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06002623 RID: 9763 RVA: 0x0001BF1F File Offset: 0x0001A11F
		// (set) Token: 0x06002624 RID: 9764 RVA: 0x0001BF27 File Offset: 0x0001A127
		public NetworkUser networkUser { get; private set; }

		// Token: 0x06002625 RID: 9765 RVA: 0x000B0C7C File Offset: 0x000AEE7C
		private void Awake()
		{
			this.networkUser = base.GetComponent<NetworkUser>();
			this.maskBitArrayConverter = new SerializableBitArray(AchievementManager.serverAchievementCount);
			if (NetworkServer.active)
			{
				this.achievementTrackers = new BaseServerAchievement[AchievementManager.serverAchievementCount];
			}
			if (NetworkClient.active)
			{
				this.maskBuffer = new byte[this.maskBitArrayConverter.byteCount];
			}
		}

		// Token: 0x06002626 RID: 9766 RVA: 0x0001BF30 File Offset: 0x0001A130
		private void Start()
		{
			if (this.networkUser.localUser != null)
			{
				UserAchievementManager userAchievementManager = AchievementManager.GetUserAchievementManager(this.networkUser.localUser);
				if (userAchievementManager == null)
				{
					return;
				}
				userAchievementManager.TransmitAchievementRequestsToServer();
			}
		}

		// Token: 0x06002627 RID: 9767 RVA: 0x000B0CDC File Offset: 0x000AEEDC
		private void OnDestroy()
		{
			if (this.achievementTrackers != null)
			{
				int serverAchievementCount = AchievementManager.serverAchievementCount;
				for (int i = 0; i < serverAchievementCount; i++)
				{
					this.SetAchievementTracked(new ServerAchievementIndex
					{
						intValue = i
					}, false);
				}
			}
		}

		// Token: 0x06002628 RID: 9768 RVA: 0x000B0D1C File Offset: 0x000AEF1C
		[Client]
		public void SendAchievementTrackerRequestsMaskToServer(bool[] serverAchievementsToTrackMask)
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.Achievements.ServerAchievementTracker::SendAchievementTrackerRequestsMaskToServer(System.Boolean[])' called on server");
				return;
			}
			int serverAchievementCount = AchievementManager.serverAchievementCount;
			for (int i = 0; i < serverAchievementCount; i++)
			{
				this.maskBitArrayConverter[i] = serverAchievementsToTrackMask[i];
			}
			this.maskBitArrayConverter.GetBytes(this.maskBuffer);
			this.CallCmdSetAchievementTrackerRequests(this.maskBuffer);
		}

		// Token: 0x06002629 RID: 9769 RVA: 0x000B0D7C File Offset: 0x000AEF7C
		[Command]
		private void CmdSetAchievementTrackerRequests(byte[] packedServerAchievementsToTrackMask)
		{
			int serverAchievementCount = AchievementManager.serverAchievementCount;
			if (packedServerAchievementsToTrackMask.Length << 3 < serverAchievementCount)
			{
				return;
			}
			for (int i = 0; i < serverAchievementCount; i++)
			{
				int num = i >> 3;
				int num2 = i & 7;
				this.SetAchievementTracked(new ServerAchievementIndex
				{
					intValue = i
				}, (packedServerAchievementsToTrackMask[num] >> num2 & 1) != 0);
			}
		}

		// Token: 0x0600262A RID: 9770 RVA: 0x000B0DD4 File Offset: 0x000AEFD4
		private void SetAchievementTracked(ServerAchievementIndex serverAchievementIndex, bool shouldTrack)
		{
			BaseServerAchievement baseServerAchievement = this.achievementTrackers[serverAchievementIndex.intValue];
			if (shouldTrack == (baseServerAchievement != null))
			{
				return;
			}
			if (shouldTrack)
			{
				BaseServerAchievement baseServerAchievement2 = BaseServerAchievement.Instantiate(serverAchievementIndex);
				baseServerAchievement2.serverAchievementTracker = this;
				this.achievementTrackers[serverAchievementIndex.intValue] = baseServerAchievement2;
				baseServerAchievement2.OnInstall();
				return;
			}
			baseServerAchievement.OnUninstall();
			this.achievementTrackers[serverAchievementIndex.intValue] = null;
		}

		// Token: 0x0600262B RID: 9771 RVA: 0x000B0E34 File Offset: 0x000AF034
		[ClientRpc]
		public void RpcGrantAchievement(ServerAchievementIndex serverAchievementIndex)
		{
			LocalUser localUser = this.networkUser.localUser;
			if (localUser != null)
			{
				UserAchievementManager userAchievementManager = AchievementManager.GetUserAchievementManager(localUser);
				if (userAchievementManager == null)
				{
					return;
				}
				userAchievementManager.HandleServerAchievementCompleted(serverAchievementIndex);
			}
		}

		// Token: 0x0600262D RID: 9773 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x0600262E RID: 9774 RVA: 0x0001BF59 File Offset: 0x0001A159
		protected static void InvokeCmdCmdSetAchievementTrackerRequests(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSetAchievementTrackerRequests called on client.");
				return;
			}
			((ServerAchievementTracker)obj).CmdSetAchievementTrackerRequests(reader.ReadBytesAndSize());
		}

		// Token: 0x0600262F RID: 9775 RVA: 0x000B0E64 File Offset: 0x000AF064
		public void CallCmdSetAchievementTrackerRequests(byte[] packedServerAchievementsToTrackMask)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdSetAchievementTrackerRequests called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdSetAchievementTrackerRequests(packedServerAchievementsToTrackMask);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)ServerAchievementTracker.kCmdCmdSetAchievementTrackerRequests);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.WriteBytesFull(packedServerAchievementsToTrackMask);
			base.SendCommandInternal(networkWriter, 0, "CmdSetAchievementTrackerRequests");
		}

		// Token: 0x06002630 RID: 9776 RVA: 0x0001BF82 File Offset: 0x0001A182
		protected static void InvokeRpcRpcGrantAchievement(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcGrantAchievement called on server.");
				return;
			}
			((ServerAchievementTracker)obj).RpcGrantAchievement(GeneratedNetworkCode._ReadServerAchievementIndex_None(reader));
		}

		// Token: 0x06002631 RID: 9777 RVA: 0x000B0EF0 File Offset: 0x000AF0F0
		public void CallRpcGrantAchievement(ServerAchievementIndex serverAchievementIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcGrantAchievement called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)ServerAchievementTracker.kRpcRpcGrantAchievement);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			GeneratedNetworkCode._WriteServerAchievementIndex_None(networkWriter, serverAchievementIndex);
			this.SendRPCInternal(networkWriter, 0, "RpcGrantAchievement");
		}

		// Token: 0x06002632 RID: 9778 RVA: 0x000B0F64 File Offset: 0x000AF164
		static ServerAchievementTracker()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(ServerAchievementTracker), ServerAchievementTracker.kCmdCmdSetAchievementTrackerRequests, new NetworkBehaviour.CmdDelegate(ServerAchievementTracker.InvokeCmdCmdSetAchievementTrackerRequests));
			ServerAchievementTracker.kRpcRpcGrantAchievement = -1713740939;
			NetworkBehaviour.RegisterRpcDelegate(typeof(ServerAchievementTracker), ServerAchievementTracker.kRpcRpcGrantAchievement, new NetworkBehaviour.CmdDelegate(ServerAchievementTracker.InvokeRpcRpcGrantAchievement));
			NetworkCRC.RegisterBehaviour("ServerAchievementTracker", 0);
		}

		// Token: 0x06002633 RID: 9779 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06002634 RID: 9780 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x0400285A RID: 10330
		private BaseServerAchievement[] achievementTrackers;

		// Token: 0x0400285B RID: 10331
		private SerializableBitArray maskBitArrayConverter;

		// Token: 0x0400285C RID: 10332
		private byte[] maskBuffer;

		// Token: 0x0400285D RID: 10333
		private static int kCmdCmdSetAchievementTrackerRequests = 387052099;

		// Token: 0x0400285E RID: 10334
		private static int kRpcRpcGrantAchievement;
	}
}
