using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001F0 RID: 496
	public struct ServerAchievementIndex : IEquatable<ServerAchievementIndex>
	{
		// Token: 0x06000997 RID: 2455 RVA: 0x00007CA2 File Offset: 0x00005EA2
		public bool Equals(ServerAchievementIndex other)
		{
			return this.intValue == other.intValue;
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x00007CB2 File Offset: 0x00005EB2
		public override bool Equals(object obj)
		{
			return obj != null && obj is ServerAchievementIndex && this.Equals((ServerAchievementIndex)obj);
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x00007CCF File Offset: 0x00005ECF
		public override int GetHashCode()
		{
			return this.intValue.GetHashCode();
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x00007CDC File Offset: 0x00005EDC
		public static ServerAchievementIndex operator ++(ServerAchievementIndex achievementIndex)
		{
			achievementIndex.intValue++;
			return achievementIndex;
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x00007CA2 File Offset: 0x00005EA2
		public static bool operator ==(ServerAchievementIndex a, ServerAchievementIndex b)
		{
			return a.intValue == b.intValue;
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x00007CEB File Offset: 0x00005EEB
		public static bool operator !=(ServerAchievementIndex a, ServerAchievementIndex b)
		{
			return a.intValue != b.intValue;
		}

		// Token: 0x04000CFA RID: 3322
		[SerializeField]
		public int intValue;
	}
}
