using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200045C RID: 1116
	[Serializable]
	public struct NetworkUserId : IEquatable<NetworkUserId>
	{
		// Token: 0x06001900 RID: 6400 RVA: 0x0001297F File Offset: 0x00010B7F
		private NetworkUserId(ulong value, byte subId)
		{
			this.value = value;
			this.subId = subId;
		}

		// Token: 0x06001901 RID: 6401 RVA: 0x0001298F File Offset: 0x00010B8F
		public static NetworkUserId FromIp(string ip, byte subId)
		{
			return new NetworkUserId((ulong)((long)ip.GetHashCode()), subId);
		}

		// Token: 0x06001902 RID: 6402 RVA: 0x0001299E File Offset: 0x00010B9E
		public static NetworkUserId FromSteamId(ulong steamId, byte subId)
		{
			return new NetworkUserId(steamId, subId);
		}

		// Token: 0x06001903 RID: 6403 RVA: 0x000129A7 File Offset: 0x00010BA7
		public bool Equals(NetworkUserId other)
		{
			return this.value == other.value && this.subId == other.subId;
		}

		// Token: 0x06001904 RID: 6404 RVA: 0x000129C7 File Offset: 0x00010BC7
		public override bool Equals(object obj)
		{
			return obj != null && obj is NetworkUserId && this.Equals((NetworkUserId)obj);
		}

		// Token: 0x06001905 RID: 6405 RVA: 0x000820BC File Offset: 0x000802BC
		public override int GetHashCode()
		{
			return this.value.GetHashCode() * 397 ^ this.subId.GetHashCode();
		}

		// Token: 0x04001C60 RID: 7264
		[SerializeField]
		public readonly ulong value;

		// Token: 0x04001C61 RID: 7265
		[SerializeField]
		public readonly byte subId;
	}
}
