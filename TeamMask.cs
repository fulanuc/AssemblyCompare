using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004BF RID: 1215
	[Serializable]
	public struct TeamMask
	{
		// Token: 0x06001B6A RID: 7018 RVA: 0x000144B4 File Offset: 0x000126B4
		public bool HasTeam(TeamIndex teamIndex)
		{
			return teamIndex >= TeamIndex.Neutral && teamIndex < TeamIndex.Count && ((ulong)this.a & 1UL << (int)teamIndex) > 0UL;
		}

		// Token: 0x06001B6B RID: 7019 RVA: 0x000144D3 File Offset: 0x000126D3
		public void AddTeam(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return;
			}
			this.a |= (byte)(1 << (int)teamIndex);
		}

		// Token: 0x06001B6C RID: 7020 RVA: 0x000144F3 File Offset: 0x000126F3
		public void RemoveTeam(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return;
			}
			this.a &= (byte)(~(byte)(1 << (int)teamIndex));
		}

		// Token: 0x06001B6D RID: 7021 RVA: 0x00088980 File Offset: 0x00086B80
		static TeamMask()
		{
			for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
			{
				TeamMask.all.AddTeam(teamIndex);
			}
			TeamMask.allButNeutral = TeamMask.all;
			TeamMask.allButNeutral.RemoveTeam(TeamIndex.Neutral);
		}

		// Token: 0x04001DF5 RID: 7669
		[SerializeField]
		public byte a;

		// Token: 0x04001DF6 RID: 7670
		public static readonly TeamMask none;

		// Token: 0x04001DF7 RID: 7671
		public static readonly TeamMask allButNeutral;

		// Token: 0x04001DF8 RID: 7672
		public static readonly TeamMask all = default(TeamMask);
	}
}
