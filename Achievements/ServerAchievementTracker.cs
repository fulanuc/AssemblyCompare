using System;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Achievements
{
	// Token: 0x020006C9 RID: 1737
	[RequireComponent(typeof(NetworkUser))]
	public class ServerAchievementTracker : NetworkBehaviour
	{
		// Token: 0x1700033A RID: 826
		// (get) Token: 0x060026BA RID: 9914 RVA: 0x0001C65A File Offset: 0x0001A85A
		// (set) Token: 0x060026BB RID: 9915 RVA: 0x0001C662 File Offset: 0x0001A862
		public NetworkUser networkUser { get; private set; }

		// Token: 0x060026BC RID: 9916 RVA: 0x000B237C File Offset: 0x000B057C
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

		// Token: 0x060026BD RID: 9917 RVA: 0x0001C66B File Offset: 0x0001A86B
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

		// Token: 0x060026BE RID: 9918 RVA: 0x000B23DC File Offset: 0x000B05DC
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

		// Token: 0x060026BF RID: 9919 RVA: 0x000B241C File Offset: 0x000B061C
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

		// Token: 0x060026C0 RID: 9920 RVA: 0x000B247C File Offset: 0x000B067C
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

		// Token: 0x060026C1 RID: 9921 RVA: 0x000B24D4 File Offset: 0x000B06D4
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

		// Token: 0x060026C2 RID: 9922 RVA: 0x000B2534 File Offset: 0x000B0734
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

		// Token: 0x060026C4 RID: 9924 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x060026C5 RID: 9925 RVA: 0x0001C694 File Offset: 0x0001A894
		protected static void InvokeCmdCmdSetAchievementTrackerRequests(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSetAchievementTrackerRequests called on client.");
				return;
			}
			((ServerAchievementTracker)obj).CmdSetAchievementTrackerRequests(reader.ReadBytesAndSize());
		}

		// Token: 0x060026C6 RID: 9926 RVA: 0x000B2564 File Offset: 0x000B0764
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

		// Token: 0x060026C7 RID: 9927 RVA: 0x0001C6BD File Offset: 0x0001A8BD
		protected static void InvokeRpcRpcGrantAchievement(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcGrantAchievement called on server.");
				return;
			}
			((ServerAchievementTracker)obj).RpcGrantAchievement(GeneratedNetworkCode._ReadServerAchievementIndex_None(reader));
		}

		// Token: 0x060026C8 RID: 9928 RVA: 0x000B25F0 File Offset: 0x000B07F0
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

		// Token: 0x060026C9 RID: 9929 RVA: 0x000B2664 File Offset: 0x000B0864
		static ServerAchievementTracker()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(ServerAchievementTracker), ServerAchievementTracker.kCmdCmdSetAchievementTrackerRequests, new NetworkBehaviour.CmdDelegate(ServerAchievementTracker.InvokeCmdCmdSetAchievementTrackerRequests));
			ServerAchievementTracker.kRpcRpcGrantAchievement = -1713740939;
			NetworkBehaviour.RegisterRpcDelegate(typeof(ServerAchievementTracker), ServerAchievementTracker.kRpcRpcGrantAchievement, new NetworkBehaviour.CmdDelegate(ServerAchievementTracker.InvokeRpcRpcGrantAchievement));
			NetworkCRC.RegisterBehaviour("ServerAchievementTracker", 0);
		}

		// Token: 0x060026CA RID: 9930 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060026CB RID: 9931 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040028B6 RID: 10422
		private BaseServerAchievement[] achievementTrackers;

		// Token: 0x040028B7 RID: 10423
		private SerializableBitArray maskBitArrayConverter;

		// Token: 0x040028B8 RID: 10424
		private byte[] maskBuffer;

		// Token: 0x040028B9 RID: 10425
		private static int kCmdCmdSetAchievementTrackerRequests = 387052099;

		// Token: 0x040028BA RID: 10426
		private static int kRpcRpcGrantAchievement;
	}
}
