using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004CD RID: 1229
	[Serializable]
	public struct TeamMask
	{
		// Token: 0x06001BCE RID: 7118 RVA: 0x00014981 File Offset: 0x00012B81
		public bool HasTeam(TeamIndex teamIndex)
		{
			return teamIndex >= TeamIndex.Neutral && teamIndex < TeamIndex.Count && ((ulong)this.a & 1UL << (int)teamIndex) > 0UL;
		}

		// Token: 0x06001BCF RID: 7119 RVA: 0x000149A0 File Offset: 0x00012BA0
		public void AddTeam(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return;
			}
			this.a |= (byte)(1 << (int)teamIndex);
		}

		// Token: 0x06001BD0 RID: 7120 RVA: 0x000149C0 File Offset: 0x00012BC0
		public void RemoveTeam(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return;
			}
			this.a &= (byte)(~(byte)(1 << (int)teamIndex));
		}

		// Token: 0x06001BD1 RID: 7121 RVA: 0x000894F8 File Offset: 0x000876F8
		static TeamMask()
		{
			for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
			{
				TeamMask.all.AddTeam(teamIndex);
			}
			TeamMask.allButNeutral = TeamMask.all;
			TeamMask.allButNeutral.RemoveTeam(TeamIndex.Neutral);
		}

		// Token: 0x04001E2F RID: 7727
		[SerializeField]
		public byte a;

		// Token: 0x04001E30 RID: 7728
		public static readonly TeamMask none;

		// Token: 0x04001E31 RID: 7729
		public static readonly TeamMask allButNeutral;

		// Token: 0x04001E32 RID: 7730
		public static readonly TeamMask all = default(TeamMask);
	}
}
