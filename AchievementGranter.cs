using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200024A RID: 586
	public class AchievementGranter : NetworkBehaviour
	{
		// Token: 0x06000AFD RID: 2813 RVA: 0x0004A750 File Offset: 0x00048950
		[ClientRpc]
		public void RpcGrantAchievement(string achievementName)
		{
			foreach (LocalUser user in LocalUserManager.readOnlyLocalUsersList)
			{
				AchievementManager.GetUserAchievementManager(user).GrantAchievement(AchievementManager.GetAchievementDef(achievementName));
			}
		}

		// Token: 0x06000AFF RID: 2815 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06000B00 RID: 2816 RVA: 0x00008DD3 File Offset: 0x00006FD3
		protected static void InvokeRpcRpcGrantAchievement(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcGrantAchievement called on server.");
				return;
			}
			((AchievementGranter)obj).RpcGrantAchievement(reader.ReadString());
		}

		// Token: 0x06000B01 RID: 2817 RVA: 0x0004A7A4 File Offset: 0x000489A4
		public void CallRpcGrantAchievement(string achievementName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcGrantAchievement called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)AchievementGranter.kRpcRpcGrantAchievement);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(achievementName);
			this.SendRPCInternal(networkWriter, 0, "RpcGrantAchievement");
		}

		// Token: 0x06000B02 RID: 2818 RVA: 0x00008DFC File Offset: 0x00006FFC
		static AchievementGranter()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(AchievementGranter), AchievementGranter.kRpcRpcGrantAchievement, new NetworkBehaviour.CmdDelegate(AchievementGranter.InvokeRpcRpcGrantAchievement));
			NetworkCRC.RegisterBehaviour("AchievementGranter", 0);
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04000EEB RID: 3819
		private static int kRpcRpcGrantAchievement = -180752285;
	}
}
