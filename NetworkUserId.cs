using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000467 RID: 1127
	[Serializable]
	public struct NetworkUserId : IEquatable<NetworkUserId>
	{
		// Token: 0x0600195C RID: 6492 RVA: 0x00012E8C File Offset: 0x0001108C
		private NetworkUserId(ulong value, byte subId)
		{
			this.value = value;
			this.subId = subId;
		}

		// Token: 0x0600195D RID: 6493 RVA: 0x00012E9C File Offset: 0x0001109C
		public static NetworkUserId FromIp(string ip, byte subId)
		{
			return new NetworkUserId((ulong)((long)ip.GetHashCode()), subId);
		}

		// Token: 0x0600195E RID: 6494 RVA: 0x00012EAB File Offset: 0x000110AB
		public static NetworkUserId FromSteamId(ulong steamId, byte subId)
		{
			return new NetworkUserId(steamId, subId);
		}

		// Token: 0x0600195F RID: 6495 RVA: 0x00012EB4 File Offset: 0x000110B4
		public bool Equals(NetworkUserId other)
		{
			return this.value == other.value && this.subId == other.subId;
		}

		// Token: 0x06001960 RID: 6496 RVA: 0x00012ED4 File Offset: 0x000110D4
		public override bool Equals(object obj)
		{
			return obj != null && obj is NetworkUserId && this.Equals((NetworkUserId)obj);
		}

		// Token: 0x06001961 RID: 6497 RVA: 0x00082A64 File Offset: 0x00080C64
		public override int GetHashCode()
		{
			return this.value.GetHashCode() * 397 ^ this.subId.GetHashCode();
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06001962 RID: 6498 RVA: 0x00012EF1 File Offset: 0x000110F1
		public CSteamID steamId
		{
			get
			{
				return new CSteamID(this.value);
			}
		}

		// Token: 0x04001C94 RID: 7316
		[SerializeField]
		public readonly ulong value;

		// Token: 0x04001C95 RID: 7317
		[SerializeField]
		public readonly byte subId;
	}
}
