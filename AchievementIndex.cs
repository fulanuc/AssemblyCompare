using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001EF RID: 495
	public struct AchievementIndex
	{
		// Token: 0x06000996 RID: 2454 RVA: 0x00007C93 File Offset: 0x00005E93
		public static AchievementIndex operator ++(AchievementIndex achievementIndex)
		{
			achievementIndex.intValue++;
			return achievementIndex;
		}

		// Token: 0x04000CF9 RID: 3321
		[SerializeField]
		public int intValue;
	}
}
