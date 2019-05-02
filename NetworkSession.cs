using System;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000372 RID: 882
	public class NetworkSession : NetworkBehaviour
	{
		// Token: 0x17000194 RID: 404
		// (get) Token: 0x0600123D RID: 4669 RVA: 0x0000DEB0 File Offset: 0x0000C0B0
		// (set) Token: 0x0600123E RID: 4670 RVA: 0x0000DEB7 File Offset: 0x0000C0B7
		public static NetworkSession instance { get; private set; }

		// Token: 0x0600123F RID: 4671 RVA: 0x0000DEBF File Offset: 0x0000C0BF
		private void OnSyncSteamId(ulong newValue)
		{
			this.NetworksteamId = newValue;
			this.SteamworksAdvertiseGame();
		}

		// Token: 0x06001240 RID: 4672 RVA: 0x0006882C File Offset: 0x00066A2C
		private void SteamworksAdvertiseGame()
		{
			if (RoR2Application.instance.steamworksClient != null)
			{
				ulong num = this.steamId;
				uint num2 = 0u;
				ushort num3 = 0;
				NetworkSession.<SteamworksAdvertiseGame>g__CallMethod|6_1(NetworkSession.<SteamworksAdvertiseGame>g__GetField|6_2(NetworkSession.<SteamworksAdvertiseGame>g__GetField|6_2(Client.Instance, "native"), "user"), "AdvertiseGame", new object[]
				{
					num,
					num2,
					num3
				});
			}
		}

		// Token: 0x06001241 RID: 4673 RVA: 0x0000DECE File Offset: 0x0000C0CE
		private void OnEnable()
		{
			NetworkSession.instance = SingletonHelper.Assign<NetworkSession>(NetworkSession.instance, this);
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x0000DEE0 File Offset: 0x0000C0E0
		private void OnDisable()
		{
			NetworkSession.instance = SingletonHelper.Unassign<NetworkSession>(NetworkSession.instance, this);
		}

		// Token: 0x06001243 RID: 4675 RVA: 0x0000DEF2 File Offset: 0x0000C0F2
		private void Start()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (NetworkServer.active)
			{
				NetworkServer.Spawn(base.gameObject);
			}
		}

		// Token: 0x06001244 RID: 4676 RVA: 0x00068894 File Offset: 0x00066A94
		[Server]
		public Run BeginRun(Run runPrefabComponent)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.Run RoR2.NetworkSession::BeginRun(RoR2.Run)' called on client");
				return null;
			}
			if (!Run.instance)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(runPrefabComponent.gameObject);
				NetworkServer.Spawn(gameObject);
				return gameObject.GetComponent<Run>();
			}
			return null;
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x0000DF11 File Offset: 0x0000C111
		[Server]
		public void EndRun()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.NetworkSession::EndRun()' called on client");
				return;
			}
			if (Run.instance)
			{
				UnityEngine.Object.Destroy(Run.instance.gameObject);
			}
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x0600124B RID: 4683 RVA: 0x0006892C File Offset: 0x00066B2C
		// (set) Token: 0x0600124C RID: 4684 RVA: 0x0000DF71 File Offset: 0x0000C171
		public ulong NetworksteamId
		{
			get
			{
				return this.steamId;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncSteamId(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<ulong>(value, ref this.steamId, dirtyBit);
			}
		}

		// Token: 0x0600124D RID: 4685 RVA: 0x00068940 File Offset: 0x00066B40
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt64(this.steamId);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt64(this.steamId);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x0600124E RID: 4686 RVA: 0x000689AC File Offset: 0x00066BAC
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.steamId = reader.ReadPackedUInt64();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncSteamId(reader.ReadPackedUInt64());
			}
		}

		// Token: 0x0400162C RID: 5676
		[SyncVar(hook = "OnSyncSteamId")]
		private ulong steamId;
	}
}
