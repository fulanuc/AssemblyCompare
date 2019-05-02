using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000200 RID: 512
	[Serializable]
	public struct ArtifactMask
	{
		// Token: 0x06000A02 RID: 2562 RVA: 0x0000810E File Offset: 0x0000630E
		public bool HasArtifact(ArtifactIndex artifactIndex)
		{
			return artifactIndex >= ArtifactIndex.Command && artifactIndex < ArtifactIndex.Count && ((int)this.a & 1 << (int)artifactIndex) != 0;
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x0000812A File Offset: 0x0000632A
		public void AddArtifact(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return;
			}
			this.a |= (ushort)(1 << (int)artifactIndex);
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x0000814A File Offset: 0x0000634A
		public void ToggleArtifact(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return;
			}
			this.a ^= (ushort)(1 << (int)artifactIndex);
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x0000816A File Offset: 0x0000636A
		public void RemoveArtifact(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return;
			}
			this.a &= (ushort)(~(ushort)(1 << (int)artifactIndex));
		}

		// Token: 0x06000A06 RID: 2566 RVA: 0x000467EC File Offset: 0x000449EC
		public static ArtifactMask operator &(ArtifactMask mask1, ArtifactMask mask2)
		{
			return new ArtifactMask
			{
				a = (mask1.a & mask2.a)
			};
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x00046818 File Offset: 0x00044A18
		static ArtifactMask()
		{
			for (ArtifactIndex artifactIndex = ArtifactIndex.Command; artifactIndex < ArtifactIndex.Count; artifactIndex++)
			{
				ArtifactMask.all.AddArtifact(artifactIndex);
			}
		}

		// Token: 0x04000D44 RID: 3396
		[SerializeField]
		public ushort a;

		// Token: 0x04000D45 RID: 3397
		public static readonly ArtifactMask none;

		// Token: 0x04000D46 RID: 3398
		public static readonly ArtifactMask all = default(ArtifactMask);
	}
}
