using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001EE RID: 494
	public struct ServerAchievementIndex : IEquatable<ServerAchievementIndex>
	{
		// Token: 0x0600098D RID: 2445 RVA: 0x00007C53 File Offset: 0x00005E53
		public bool Equals(ServerAchievementIndex other)
		{
			return this.intValue == other.intValue;
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x00007C63 File Offset: 0x00005E63
		public override bool Equals(object obj)
		{
			return obj != null && obj is ServerAchievementIndex && this.Equals((ServerAchievementIndex)obj);
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x00007C80 File Offset: 0x00005E80
		public override int GetHashCode()
		{
			return this.intValue.GetHashCode();
		}

		// Token: 0x06000990 RID: 2448 RVA: 0x00007C8D File Offset: 0x00005E8D
		public static ServerAchievementIndex operator ++(ServerAchievementIndex achievementIndex)
		{
			achievementIndex.intValue++;
			return achievementIndex;
		}

		// Token: 0x06000991 RID: 2449 RVA: 0x00007C53 File Offset: 0x00005E53
		public static bool operator ==(ServerAchievementIndex a, ServerAchievementIndex b)
		{
			return a.intValue == b.intValue;
		}

		// Token: 0x06000992 RID: 2450 RVA: 0x00007C9C File Offset: 0x00005E9C
		public static bool operator !=(ServerAchievementIndex a, ServerAchievementIndex b)
		{
			return a.intValue != b.intValue;
		}

		// Token: 0x04000CF3 RID: 3315
		[SerializeField]
		public int intValue;
	}
}
