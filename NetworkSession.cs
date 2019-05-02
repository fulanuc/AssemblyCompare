using System;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200036F RID: 879
	public class NetworkSession : NetworkBehaviour
	{
		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06001226 RID: 4646 RVA: 0x0000DDC7 File Offset: 0x0000BFC7
		// (set) Token: 0x06001227 RID: 4647 RVA: 0x0000DDCE File Offset: 0x0000BFCE
		public static NetworkSession instance { get; private set; }

		// Token: 0x06001228 RID: 4648 RVA: 0x0000DDD6 File Offset: 0x0000BFD6
		private void OnSyncSteamId(ulong newValue)
		{
			this.NetworksteamId = newValue;
			this.SteamworksAdvertiseGame();
		}

		// Token: 0x06001229 RID: 4649 RVA: 0x000684F4 File Offset: 0x000666F4
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

		// Token: 0x0600122A RID: 4650 RVA: 0x0000DDE5 File Offset: 0x0000BFE5
		private void OnEnable()
		{
			NetworkSession.instance = SingletonHelper.Assign<NetworkSession>(NetworkSession.instance, this);
		}

		// Token: 0x0600122B RID: 4651 RVA: 0x0000DDF7 File Offset: 0x0000BFF7
		private void OnDisable()
		{
			NetworkSession.instance = SingletonHelper.Unassign<NetworkSession>(NetworkSession.instance, this);
		}

		// Token: 0x0600122C RID: 4652 RVA: 0x0000DE09 File Offset: 0x0000C009
		private void Start()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (NetworkServer.active)
			{
				NetworkServer.Spawn(base.gameObject);
			}
		}

		// Token: 0x0600122D RID: 4653 RVA: 0x0006855C File Offset: 0x0006675C
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

		// Token: 0x0600122E RID: 4654 RVA: 0x0000DE28 File Offset: 0x0000C028
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

		// Token: 0x06001233 RID: 4659 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06001234 RID: 4660 RVA: 0x000685F4 File Offset: 0x000667F4
		// (set) Token: 0x06001235 RID: 4661 RVA: 0x0000DE88 File Offset: 0x0000C088
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

		// Token: 0x06001236 RID: 4662 RVA: 0x00068608 File Offset: 0x00066808
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

		// Token: 0x06001237 RID: 4663 RVA: 0x00068674 File Offset: 0x00066874
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

		// Token: 0x04001613 RID: 5651
		[SyncVar(hook = "OnSyncSteamId")]
		private ulong steamId;
	}
}
